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
using SignalRSwaggerGen.Attributes;
using System.Net;

namespace Communications.Hubs
{
	/// <summary>
	/// WebSocket Alarm hub for real-time communication.
	/// </summary>
	[SignalRHub]
	public class AlarmHub : Hub
	{
		private Connections<AlarmHub> connections;
		private readonly List<Alarm>? _alarms;
		private readonly IConfiguration _configuration;
		private IMemoryCache memoryCache;
		private TransformToDTOHelper transformToDTOHelper;

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
	/// Sends the message with alarm to client.
	/// </summary>
	/// <param name="clientId">The ID(guid) of the client that accesses the method.</param>
	/// <returns>Returns AlarmServerDTO</returns>
	[SignalRMethod("Send")]
	[SwaggerResponse(HttpStatusCode.OK, typeof(AlarmServerDTO))]
	[SwaggerResponse(HttpStatusCode.BadRequest, typeof(BadRequest))]
	public async Task Send(Guid clientId)
		{
			var serverid = Guid.Parse(_configuration["HubSettings:ServerId"]);
			var route = _configuration["HostSettings:RouteNotify"];

			while (connections.GetConnection(Context.ConnectionId) != null)
			{
				try
				{
					foreach (var alarm in _alarms)
					{
						memoryCache.TryGetValue($"{clientId}_{alarm.Id}", out Alarm? Alarm);
			
						if (Alarm == null)
						{
							var alarmDTO = await transformToDTOHelper.TransformToAlarmDTO(alarm, serverid);

							await Clients.Client(Context.ConnectionId).SendAsync(_configuration["HubSettings:Notify:HubMethod"], alarmDTO);

							Log.Information($"Alarm {alarmDTO.Alarm.Id} has been sent."
											+ "\nSender:   " + $" Server - {alarmDTO.ServerId}"
											+ "\nRecipient:" + $" Client - {clientId}");
							memoryCache.Set($"{clientId}_{alarm.Id}", alarm);
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
		/// Sends the message with alarms to all clients.
		/// </summary>
		/// <returns>Returns AlarmServerDTO</returns>
		[SignalRMethod("SendAll")]
		[SwaggerResponse(HttpStatusCode.OK, typeof(AlarmServerDTO))]
		[SwaggerResponse(HttpStatusCode.BadRequest, typeof(BadRequest))]
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

		public override async Task OnDisconnectedAsync(Exception exception)
		{
			connections.RemoveConnection(Context.ConnectionId);
			Log.Information("Disconnecting: {ConnectionId}", Context.ConnectionId);
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, "User");
			await Clients.Others.SendAsync("Notify", $"{Context.ConnectionId} is disconnected from the alarm hub.");
			await base.OnDisconnectedAsync(exception);
		}

		public async Task OnReconnectedAsync(Guid clientId)
		{
			Log.Information($"Reconnecting client {clientId}: {Context.ConnectionId}");
			await Clients.Others.SendAsync("Notify", $"{Context.ConnectionId} is reconnected.");
			await Send(clientId);
		}

	}
}
	