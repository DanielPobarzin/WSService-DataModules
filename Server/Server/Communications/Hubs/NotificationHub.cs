using Communications.Connections;
using Communications.DTO;
using Communications.Helpers;
using Communications.Hubs;
using Entities.Entities;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Communications.Hubs
{
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

		public async Task Send(Guid clientId)
		{
			var cacheNotification = await jsonCacheHelper.ReadFromFileCache(clientId);
			//if (cacheNotification.Any())
			//{ 
			//	foreach (var notification in cacheNotification)
			//			memoryCache.Set($"{Context.ConnectionId}_{notification.Id}", notification);
			//}

			while (connections.GetConnection(Context.ConnectionId) != null)
			{
				try
				{
					foreach (var notification in _notifications)
					{							
						memoryCache.TryGetValue($"{Context.ConnectionId}_{notification.Id}", out Notification? cachedNotification);
						if (cachedNotification == null)
						{
							var notificationDTO = await transformToDTOHelper.
														TransformToNotificationDTO(notification,
														Guid.Parse(_configuration["NotificationsHubSettings:ServerId"]));

							await Clients.Client(Context.ConnectionId).SendAsync("ReceiveNotification", notificationDTO);
							//memoryCache.Set($"{Context.ConnectionId}_{notification.Id}", notification);
							Log.Information($"The notification {notificationDTO.Notification.Id} with message <<{notificationDTO.Notification.Content}>> " +
											$"has been sent to client {clientId} by server {notificationDTO.ServerId}. ");
						}
					}
				}
				catch (Exception ex)
				{
					Log.Error($"Exception with data: {ex.Message}");
				}
				await Task.Delay(Convert.ToInt32(_configuration["NotificationsHubSettings:DelayMilliseconds"]));
			}
			await jsonCacheHelper.WriteToFileCache(_notifications, clientId);
		}
		public override async Task OnConnectedAsync()
		{
			connections.AddConnection(Context.ConnectionId, Context.ConnectionId);
			Log.Information("New connection: {@userId}", Context.ConnectionId);
			await Groups.AddToGroupAsync(Context.ConnectionId, "Notify");
			await Clients.Others.SendAsync("Notify", $"{Context.ConnectionId} is connected.");
			await Clients.Others.SendAsync("Notify", $"{Context.ConnectionId} joined the Notify group.");
			await Clients.Caller.SendAsync("Notify", $"You have joined the Notify group.");
			await base.OnConnectedAsync();
		}

		public override async Task OnDisconnectedAsync(Exception exception)
		{
			connections.RemoveConnection(Context.ConnectionId);
			Log.Information("Disconnecting: {ConnectionId}", Context.ConnectionId);
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, "User");
			await Clients.Others.SendAsync("Notify", $"{Context.ConnectionId} is disconnected.");
			await base.OnDisconnectedAsync(exception);
		}

		public async Task OnReconnectedAsync(Guid clientId)
		{
			Log.Information($"Reconnecting client {clientId}: {Context.ConnectionId}");
			await Clients.Others.SendAsync("Notify", $"{Context.ConnectionId} is reconnected.");
		}

	}
}




