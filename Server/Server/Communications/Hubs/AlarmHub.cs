using Communications.Connections;
using Communications.DTO;
using Communications.Helpers;
using Entities.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using NSwag.Annotations;
using Serilog;
using Shared.Share.KafkaMessage;
using SignalRSwaggerGen.Attributes;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Communications.Hubs
{
	/// <summary>
	/// WebSocket Alarm hub for real-time communication.
	/// This hub is responsible for sending alarm messages to connected clients.
	/// </summary>
	[SignalRHub]
	public class AlarmHub : Hub
	{
		private Connections<AlarmHub> connections;
		private readonly List<Alarm>? _alarms;
		private readonly IConfiguration _configuration;
		private IMemoryCache memoryCache;
		private TransformToDTOHelper transformToDTOHelper;

		/// <summary>
		/// Initializes a new instance of the <see cref="AlarmHub"/> class.
		/// </summary>
		/// <param name="alarms">The list of alarms to be monitored.</param>
		/// <param name="connections">The connection manager for handling client connections.</param>
		/// <param name="transformToDTOHelper">Helper for transforming alarms to DTOs.</param>
		/// <param name="configuration">Application configuration settings.</param>
		/// <param name="memoryCache">Memory cache for storing alarm states.</param>
		public AlarmHub(List<Alarm>? alarms,
		   Connections<AlarmHub> connections,
		   TransformToDTOHelper transformToDTOHelper,
		   IConfiguration configuration,
		   IMemoryCache memoryCache)
		{
			_alarms = alarms;
			_configuration = configuration;
			this.memoryCache = memoryCache;
			this.connections = connections;
			this.transformToDTOHelper = transformToDTOHelper;
		}

		/// <summary>
		/// Sends the message with alarm to the specified client.
		/// This method continuously checks for new alarms and sends them to the client until the connection is closed.
		/// </summary>
		/// <param name="clientId">The ID (GUID) of the client that accesses the method.</param>
		/// <returns>A task that represents the asynchronous operation. 
		/// The task result contains an <see cref="AlarmServerDTO"/> object.</returns>
		[SignalRMethod("Send")]
		[SwaggerResponse(HttpStatusCode.OK, typeof(AlarmServerDTO), Description = "The alarm was successfully sent to the client.")]
		[SwaggerResponse(HttpStatusCode.BadRequest, typeof(BadRequest), Description = "The request was invalid.")]
		[SwaggerResponse(HttpStatusCode.InternalServerError, typeof(string), Description = "An error occurred while processing the request.")]
		public async Task Send(Guid clientId)
		{
			var serverid = Guid.Parse(_configuration["HubSettings:ServerId"]);
			while (connections.GetConnection(Context.ConnectionId) != null)
			{
				try
				{
					foreach (var alarm in _alarms)
					{
						var CompositKey = $"{clientId}_{alarm.Id}";
						if (!memoryCache.TryGetValue(CompositKey, out Alarm? Alarm))
						{
							var alarmDTO = await transformToDTOHelper.TransformToAlarmDTO(alarm, serverid);
							{
								KafkaMessageMetrics.Instance.TotalCountMessages += 1;
								KafkaMessageMetrics.Instance.TotalMessagesSize += Encoding.UTF8.GetBytes(JsonSerializer.Serialize(alarmDTO)).Length;
								KafkaMessageMetrics.Instance.CountAlarms += 1;
								KafkaMessageMetrics.Instance.Latency = alarmDTO.DateAndTimeSendDataByServer - alarmDTO.Alarm.CreationDateTime;
							}

							await Clients.Client(Context.ConnectionId).SendAsync(_configuration["HubSettings:Notify:HubMethod"], alarmDTO);

							Log.Information($"Alarm {alarmDTO.Alarm.Id} has been sent."
											+ "\nSender:   " + $" Server - {alarmDTO.ServerId}"
											+ "\nRecipient:" + $" Client - {clientId}");
							memoryCache.Set(CompositKey, alarm);
						}
					}
				}
				catch (Exception ex)
				{
					Log.Error($"Exception with data: {ex.Message}");
				}
				await Task.Delay(Convert.ToInt32(_configuration["HubSettings:Alarm:DelayMilliseconds"]));
			}
		}

		/// <summary>
		/// Sends the message with alarms to all connected clients.
		/// This method continuously checks for new alarms and broadcasts them to all clients until there are no active connections.
		/// </summary>
		/// <returns>A task that represents the asynchronous operation.</returns>
		[SignalRMethod("SendAll")]
		[SwaggerResponse(HttpStatusCode.OK, typeof(AlarmServerDTO), Description = "Alarms sent successfully.")]
		[SwaggerResponse(HttpStatusCode.BadRequest, typeof(BadRequest), Description = "Invalid request.")]
		public async Task SendAll()
		{
			while (connections.GetConnections().Any())
			{
				var serverid = Guid.Parse(_configuration["HubSettings:ServerId"]);
				try
				{
					foreach (var alarm in _alarms)
					{
						var alarmDTO = await transformToDTOHelper.TransformToAlarmDTO(alarm, serverid);

						{
							KafkaMessageMetrics.Instance.TotalCountMessages += 1;
							KafkaMessageMetrics.Instance.TotalMessagesSize += Encoding.UTF8.GetBytes(JsonSerializer.Serialize(alarmDTO)).Length;
							KafkaMessageMetrics.Instance.CountAlarms += 1;
							KafkaMessageMetrics.Instance.Latency = alarmDTO.DateAndTimeSendDataByServer - alarmDTO.Alarm.CreationDateTime;
						}

						await Clients.All.SendAsync(_configuration["HubSettings:Alarm:HubMethod"], alarmDTO);

						Log.Information($"Alarm {alarmDTO.Alarm.Id} has been sent."
											+ "\nSender:   " + $" Server - {alarmDTO.ServerId}");
					}
				}
				catch (Exception ex)
				{
					Log.Error($"Exception with data: {ex.Message}");
				}
				await Task.Delay(Convert.ToInt32(_configuration["HubSettings:Notify:DelayMilliseconds"]));
			}
		}

		/// <summary>
		/// Invoked when a new client connects to the hub.
		/// Adds the connection to the connection manager and notifies other clients.
		/// </summary>
		/// <returns>A task that represents the asynchronous operation.</returns>
		public override async Task OnConnectedAsync()
		{
			connections.AddConnection(Context.ConnectionId, Context.ConnectionId);
			Log.Information("New connection: {@userId}", Context.ConnectionId);
			await Groups.AddToGroupAsync(Context.ConnectionId, "Alarm");
			await Clients.Others.SendAsync("Notify", $"{Context.ConnectionId} is connected. Type of message : alarm.");
			await Clients.Others.SendAsync("Notify", $"{Context.ConnectionId} joined the Alarm group.");
			await Clients.Client(Context.ConnectionId).SendAsync("Notify", $"You have joined the Alarm group.");
			await base.OnConnectedAsync();
		}

		/// <summary>
		/// Invoked when a client disconnects from the hub.
		/// Removes the connection from the connection manager and notifies other clients.
		/// </summary>
		/// <param name="exception">The exception that caused the disconnection, if any.</param>
		/// <returns>A task that represents the asynchronous operation.</returns>
		public override async Task OnDisconnectedAsync(Exception exception)
		{
			connections.RemoveConnection(Context.ConnectionId);
			Log.Information("Disconnecting: {ConnectionId}", Context.ConnectionId);
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, "User");
			await Clients.Others.SendAsync("Notify", $"{Context.ConnectionId} is disconnected from the alarm hub.");
			await base.OnDisconnectedAsync(exception);
		}

		/// <summary>
		/// Invoked when a client reconnects to the hub.
		/// Notifies other clients about the reconnection and resends alarms to the reconnected client.
		/// </summary>
		/// <param name="clientId">The ID of the client that is reconnecting.</param>
		/// <returns>A task that represents the asynchronous operation.</returns>
		public async Task OnReconnectedAsync(Guid clientId)
		{
			Log.Information($"Reconnecting client {clientId}: {Context.ConnectionId}");
			await Clients.Others.SendAsync("Notify", $"{Context.ConnectionId} is reconnected.");
			await Send(clientId);
		}

	}
}
	