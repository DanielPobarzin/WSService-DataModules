using Communications.Connections;
using Communications.DTO;
using Communications.Helpers;
using Communications.UoW;
using Entities.Entities;
using Entities.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
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
	/// WebSocket service for real-time communication.
	/// </summary>
	[SignalRHub]
	public class NotificationHub : Hub
	{
		private Connections<NotificationHub> connections;
		private UnitOfWorkConnections unitOfWorkConnections;
		private readonly List<Notification>? _notifications;
		private readonly IConfiguration _configuration;
		private TransformToDTOHelper transformToDTOHelper;
		private JsonCacheHelper jsonCacheHelper;


		public NotificationHub (List<Notification>? notifications, 
			   Connections<NotificationHub> connections,
			   TransformToDTOHelper transformToDTOHelper,
			   JsonCacheHelper jsonCacheHelper,
			   IConfiguration configuration, 
			   UnitOfWorkConnections unitOfWorkConnections)
		{
			_configuration = configuration;
			_notifications = notifications;
			this.connections = connections;
			this.transformToDTOHelper = transformToDTOHelper;
			this.jsonCacheHelper = jsonCacheHelper;
			this.unitOfWorkConnections = unitOfWorkConnections;
		}

		/// <summary>
		/// Sends the message with notification to client.
		/// </summary>
		/// <param name="clientId">The ID(guid) of the client that accesses the method.</param>
		/// <returns>Returns MessageServerDTO</returns>
		[SignalRMethod("Send")]
		[SwaggerResponse(HttpStatusCode.OK, typeof(MessageServerDTO))]
		[SwaggerResponse(HttpStatusCode.BadRequest, typeof(BadRequest))]
		public async Task Send(Guid clientId)
		{
			var serverid = Guid.Parse(_configuration["HubSettings:ServerId"]);
			var route = _configuration["HostSettings:RouteNotify"];

			await AddContextConnection(clientId, serverid, Context.ConnectionId, route);

			while (connections.GetConnection(Context.ConnectionId) != null)
			{
				try
				{
					foreach (var notification in _notifications)
					{
						var notificationDTO = await transformToDTOHelper.TransformToNotificationDTO(notification, serverid);

						await Clients.Client(Context.ConnectionId).SendAsync(_configuration["HubSettings:Notify:HubMethod"], notificationDTO);

						Log.Information($"Notification {notificationDTO.Notification.Id} has been sent."
											+ "\nSender:\t" + $" Server - {notificationDTO.ServerId}"
											+ "\nRecipient:\t" + $" Client - {clientId}");	
					}
				}
				catch (Exception ex)
				{
					Log.Error($"Exception with data: {ex.Message}");
				}
				await Task.Delay(Convert.ToInt32(_configuration["HubSettings:Notify:DelayMilliseconds"]));
			}
			await jsonCacheHelper.WriteToFileCache(_notifications, clientId);
		}

		/// <summary>
		/// Sends the message with notification to all clients.
		/// </summary>
		/// <returns>Returns MessageServerDTO</returns>
		[SignalRMethod("SendAll")]
		[SwaggerResponse(HttpStatusCode.OK, typeof(MessageServerDTO))]
		[SwaggerResponse(HttpStatusCode.BadRequest, typeof(BadRequest))]
		public async Task SendAll()
		{
			while (connections.GetConnections().Any())
			{
				var serverid = Guid.Parse(_configuration["HubSettings:ServerId"]);
				try
				{
					foreach (var notification in _notifications)
					{
						var notificationDTO = await transformToDTOHelper.TransformToNotificationDTO(notification, serverid);

						await Clients.All.SendAsync(_configuration["HubSettings:Notify:HubMethod"], notificationDTO);

						Log.Information($"Notification {notificationDTO.Notification.Id} has been sent."
											+ "\nSender:\t" + $" Server - {notificationDTO.ServerId}");
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
			await Groups.AddToGroupAsync(Context.ConnectionId, "Notify");
			await Clients.Others.SendAsync("Notify", $"{Context.ConnectionId} is connected. Type of message : notification.");
			await Clients.Others.SendAsync("Notify", $"{Context.ConnectionId} joined the Notify group.");
			await Clients.Client(Context.ConnectionId).SendAsync("Notify", $"You have joined the Notify group.");
			await base.OnConnectedAsync();
		}

		public override async Task OnDisconnectedAsync(Exception exception)
		{
			connections.RemoveConnection(Context.ConnectionId);
			await RemoveContextConnection(Context.ConnectionId);
			Log.Information("Disconnecting: {ConnectionId}", Context.ConnectionId);
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, "User");
			await Clients.Others.SendAsync("Notify", $"{Context.ConnectionId} is disconnected from the notify hub.");
			await base.OnDisconnectedAsync(exception);
		}
		[SignalRHidden]
		public async Task OnReconnectedAsync(Guid clientId)
		{
			Log.Information($"Reconnecting client {clientId}: {Context.ConnectionId}");
			await Clients.Others.SendAsync("Notify", $"{Context.ConnectionId} is reconnected.");
			await Send(clientId);
		}
		public async Task AddContextConnection(Guid clientId, Guid serverid, string connectionId, string route)
		{
			var connection = new ConnectionContext
			{
				ClientId = clientId,
				ServerId = serverid,
				ConnectionId = connectionId,
				StartConnection = DateTime.Now,
				HubRoute = route
			};
			await unitOfWorkConnections.Notifications.AddConnection(connection);
			unitOfWorkConnections.Save();
		}
		public async Task RemoveContextConnection(string connectionId)
		{
			await unitOfWorkConnections.Notifications.RemoveConnection(connectionId);
			unitOfWorkConnections.Save();
		}
	}
}




