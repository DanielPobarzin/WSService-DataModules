using Communications.Connections;
using Communications.DTO;
using Communications.Helpers;
using Communications.UoW;
using Entities.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using NSwag.Annotations;
using Serilog;
using Shared.Common;
using Shared.Services;
using Shared.Share.KafkaMessage;
using SignalRSwaggerGen.Attributes;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Communications.Hubs
{
	/// <summary>
	/// WebSocket Notification hub for real-time communication.
	/// This hub is responsible for sending notifications messages to connected clients.
	/// </summary>
	[SignalRHub]
	public class NotificationHub : Hub
	{
		private ConcurrentConnections<NotificationHub> connections;
		private readonly List<Notification>? _notifications;
		private readonly IConfiguration _configuration;
		private IMemoryCache memoryCache;
		private ProducerService producerService;

		/// <summary>
		/// Initializes a new instance of the <see cref="NotificationHub"/> class.
		/// </summary>
		/// <param name="notifications">The list of notifications to be monitored.</param>
		/// <param name="connections">The connection manager for handling client connections.</param>
		/// <param name="configuration">Application configuration settings.</param>
		/// <param name="memoryCache">Memory cache for storing alarm states.</param>
		/// <param name="producerService">Producer for Kafka.</param>
		public NotificationHub (List<Notification>? notifications,
			   ConcurrentConnections<NotificationHub> connections,
			   IMemoryCache memoryCache,
			   IConfiguration configuration,
				ProducerService producerService)
		{
			_configuration = configuration;
			_notifications = notifications;
			this.connections = connections;
			this.memoryCache = memoryCache;
			this.producerService = producerService;
		}

		/// <summary>
		/// Sends the message with notification to the specified client.
		/// This method continuously checks for new notifications and sends them to the client until the connection is closed.
		/// </summary>
		/// <param name="clientId">The ID (GUID) of the client that accesses the method.</param>
		/// <returns>A task that represents the asynchronous operation. 
		/// The task result contains an <see cref="MessageServerDTO"/> object.</returns>
		[SignalRMethod("Send")]
		[SwaggerResponse(HttpStatusCode.OK, typeof(MessageServerDTO), Description = "The notification was successfully sent to the client.")]
		[SwaggerResponse(HttpStatusCode.BadRequest, typeof(BadRequest), Description = "The request was invalid.")]
		[SwaggerResponse(HttpStatusCode.InternalServerError, typeof(string), Description = "An error occurred while processing the request.")]
		public async Task Send(Guid clientId)
		{
			var serverid = Guid.Parse(_configuration["HubSettings:ServerId"]);
	
			while (connections.GetConnection(Context.ConnectionId) != null)
			{
				try
				{
					ConnectionsHandler.Instance.AddConnection(connections.GetConnection(Context.ConnectionId), serverid, clientId);
					var connection = ConnectionsHandler.Instance.GetConnection(Context.ConnectionId);
					await producerService.PutMessageProducerProcessAsync("connections", JsonSerializer.Serialize(connection, new JsonSerializerOptions
					{
						WriteIndented = true
					}), "open-connection");

					foreach (var notification in _notifications)
					{
						var CompositKey = $"{clientId}_{notification.Id}";
						
						if (!memoryCache.TryGetValue(CompositKey, out Notification? Notification))
						{
							var notificationDTO = await TransformToDTOHelper.TransformToNotificationDTO(notification, serverid);

							{
								KafkaMessageMetrics.Instance.TotalCountMessages += 1;
								KafkaMessageMetrics.Instance.TotalMessagesSize += Encoding.UTF8.GetBytes(JsonSerializer.Serialize(notificationDTO)).Length;
								KafkaMessageMetrics.Instance.CountAlarms += 1;
								KafkaMessageMetrics.Instance.Latency = notificationDTO.DateAndTimeSendDataByServer - notificationDTO.Notification.CreationDateTime;
							}

							await Clients.Client(Context.ConnectionId).SendAsync(_configuration["HubSettings:Notify:HubMethod"], notificationDTO);

							Log.Information($"Notification {notificationDTO.Notification.Id} has been sent."
										+ "\nSender:\t" + $" Server - {notificationDTO.ServerId}"
										+ "\nRecipient:\t" + $" Client - {clientId}");

							memoryCache.Set(CompositKey, notification);
						}
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
			KafkaMessageMetrics.Instance.ConnectionStatus = Interactors.Enums.ConnectionStatus.Opened;
			KafkaMessageMetrics.Instance.CountListeners += 1;
			connections.AddConnection(Context.ConnectionId, Context.ConnectionId);
			Log.Information("New connection: {@userId}", Context.ConnectionId);
			await Groups.AddToGroupAsync(Context.ConnectionId, "Notify");
			await Clients.Others.SendAsync("Notify", $"{Context.ConnectionId} is connected. Type of message : notification.");
			await Clients.Others.SendAsync("Notify", $"{Context.ConnectionId} joined the Notify group.");
			await Clients.Client(Context.ConnectionId).SendAsync("Notify", $"You have joined the Notify group.");
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
			KafkaMessageMetrics.Instance.ConnectionStatus = Interactors.Enums.ConnectionStatus.Opened;
			KafkaMessageMetrics.Instance.CountListeners -= 1;
			connections.RemoveConnection(Context.ConnectionId);
			ConnectionsHandler.Instance.RemoveConnection(connections.GetConnection(Context.ConnectionId));
			var connection = ConnectionsHandler.Instance.GetConnection(Context.ConnectionId);
			await producerService.PutMessageProducerProcessAsync("connections", JsonSerializer.Serialize(connection, new JsonSerializerOptions
			{
				WriteIndented = true
			}), "close-connection");
			Log.Information("Disconnecting: {ConnectionId}", Context.ConnectionId);
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, "User");
			await Clients.Others.SendAsync("Notify", $"{Context.ConnectionId} is disconnected from the notify hub.");
			await base.OnDisconnectedAsync(exception);
		}

		/// <summary>
		/// Invoked when a client reconnects to the hub.
		/// Notifies other clients about the reconnection and resends notifications to the reconnected client.
		/// </summary>
		/// <param name="clientId">The ID of the client that is reconnecting.</param>
		/// <returns>A task that represents the asynchronous operation.</returns>
		public async Task OnReconnectedAsync(Guid clientId)
		{
			KafkaMessageMetrics.Instance.CountListeners += 1;
			Log.Information($"Reconnecting client {clientId}: {Context.ConnectionId}");
			await Clients.Others.SendAsync("Notify", $"{Context.ConnectionId} is reconnected.");
			await Send(clientId);
		}
	}
}




