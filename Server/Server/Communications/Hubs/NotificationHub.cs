using Communications.Connections;
using Communications.DTO;
using Communications.Helpers;
using Entities.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Serilog;
using SignalRSwaggerGen.Attributes;
using NSwag.Annotations;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Communications.Hubs
{
	/// <summary>
	/// WebSocket service for real-time communication.
	/// </summary>
	[SignalRHub]
	public class NotificationHub : Hub
	{
		private Connections<NotificationHub> connections;
		private readonly List<Notification>? _notifications;
		private readonly IConfiguration _configuration;
		private IMemoryCache memoryCache;
		private TransformToDTOHelper transformToDTOHelper;
		private JsonCacheHelper jsonCacheHelper;


		public NotificationHub (List<Notification>? notifications, 
			   Connections<NotificationHub> connections,
			   TransformToDTOHelper transformToDTOHelper,
			   JsonCacheHelper jsonCacheHelper,
			   IConfiguration configuration, 
			   IMemoryCache memoryCache)
		{
			_configuration = configuration;
			_notifications = notifications;
			this.memoryCache = memoryCache;
			this.connections = connections;
			this.transformToDTOHelper = transformToDTOHelper;
			this.jsonCacheHelper = jsonCacheHelper;
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
			var cacheNotification = await jsonCacheHelper.ReadFromFileCache<Notification>(clientId);
			if (cacheNotification.Any() && bool.Parse(_configuration["HubSettings:Notify:UseCache"]))
			{
				foreach (var notification in cacheNotification)
					memoryCache.Set($"{Context.ConnectionId}_{notification.Id}", notification);
			}

			while (connections.GetConnection(Context.ConnectionId) != null)
			{
				try
				{
					foreach (var notification in _notifications)
					{
						var notificationDTO = await transformToDTOHelper.TransformToNotificationDTO(notification, Guid.Parse(_configuration["HubSettings:ServerId"]));

						if (bool.Parse(_configuration["HubSettings:Notify:UseCache"]))
						{
							memoryCache.TryGetValue($"{Context.ConnectionId}_{notification.Id}", out Notification? cachedNotification);

							if (cachedNotification == null)
							{
								await Clients.Client(Context.ConnectionId).SendAsync(_configuration["HubSettings:Notify:HubMethod"], notificationDTO);

								memoryCache.Set($"{Context.ConnectionId}_{notification.Id}", notification);

								Log.Information($"Notification {notificationDTO.Notification.Id} has been sent."
											+ "\nSender:\t" + $" Server - {notificationDTO.ServerId}"
											+ "\nRecipient:\t" + $" Client - {clientId}");
							}
						}
						else
						{
							await Clients.Client(Context.ConnectionId).SendAsync(_configuration["HubSettings:Notify:HubMethod"], notificationDTO);

							Log.Information($"Notification {notificationDTO.Notification.Id} has been sent."
										+ "\nSender:\t" + $" Server - {notificationDTO.ServerId}"
										+ "\nRecipient:\t" + $" Client - {clientId}");
						}
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
		[SignalRHidden]
		public override async Task OnDisconnectedAsync(Exception exception)
		{
			connections.RemoveConnection(Context.ConnectionId);
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

	}
}




