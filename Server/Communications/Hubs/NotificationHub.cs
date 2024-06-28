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
		private Connections<NotificationHub> _connections;
		private readonly List<Notification>? _notifications;
		private readonly IConfiguration _configuration;
		private readonly IMemoryCache _memoryCache;

		
		
		public NotificationHub (List<Notification>? notifications, 
			   Connections<NotificationHub> connections, 
			   IConfiguration configuration, 
			   IMemoryCache memoryCache)
		{
			_configuration = configuration;
			_notifications = notifications;
			_memoryCache = memoryCache;
			_connections = connections;
		}

		public async Task Send(Guid clientId)
		{
			TransformToDTOHelper _TransformToDTOHelper = new TransformToDTOHelper();
			JsonCacheHelper _jsonCacheHelper = new JsonCacheHelper();
			var cacheNotification = await _jsonCacheHelper.ReadFromFileCache(clientId);

			if (cacheNotification.Any())
			{ 
				foreach (var notification in cacheNotification)
						_memoryCache.Set($"{Context.ConnectionId}_{notification.Id}", notification);
			}
			while (_connections.All.ContainsKey(Context.ConnectionId))
			{
				if (_notifications.Any())
				{
					try
					{
						foreach (var notification in _notifications)
						{
							var notificationDTO = await _TransformToDTOHelper.
														TransformToNotificationDTO(notification, 
														Guid.Parse(_configuration["NotificationsHubSettings:ServerId"]));
							
							_memoryCache.TryGetValue($"{Context.ConnectionId}_{notification.Id}", out Notification? cachedNotification);
							if (cachedNotification == null)
							{
								_memoryCache.Set($"{Context.ConnectionId}_{notification.Id}", notification);
								await Clients.Client(Context.ConnectionId).SendAsync("ReceiveNotification", notificationDTO);
								Log.Information($"The notification {notificationDTO.Notification.Id} with message <<{notificationDTO.Notification.Content}>> " +
												$"has been sent to client {clientId} by server {notificationDTO.ServerId}. ");
							}
						}
					}
					catch (Exception ex)
					{
						Log.Error($"Exception with cache: {ex.Message}");
					}
				}
				await Task.Delay(Convert.ToInt32(_configuration["NotificationsHubSettings:DelayMilliseconds"]));
			}
			await _jsonCacheHelper.WriteToFileCache(_notifications, clientId);
		}
		public override async Task OnConnectedAsync()
		{
			var cancellationTokenSource = new CancellationTokenSource();
			_connections.All.TryAdd(Context.ConnectionId, cancellationTokenSource);

			Log.Information("New connection: {@userId}", Context.ConnectionId);
			await Groups.AddToGroupAsync(Context.ConnectionId, "Notify");
			await Clients.Others.SendAsync("Notify", $"{Context.ConnectionId} is connected.");
			await Clients.Others.SendAsync("Notify", $"{Context.ConnectionId} joined the Notify group.");
			await Clients.Caller.SendAsync("Notify", $"You have joined the Notify group.");
			await base.OnConnectedAsync();
		}

		public override async Task OnDisconnectedAsync(Exception exception)
		{
			if (_connections.All.TryRemove(Context.ConnectionId, out var cancellationTokenSource))
			{
				cancellationTokenSource.Cancel();
				cancellationTokenSource.Dispose();
			}
			Log.Information("Disconnecting: {ConnectionId}", Context.ConnectionId);
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, "User");
			await Clients.Others.SendAsync("Notify", $"{Context.ConnectionId} is disconnected.");
			await base.OnDisconnectedAsync(exception);
		}

		public async Task OnReconnectedAsync()
		{
			Log.Information("Reconnecting: {ConnectionId}", Context.ConnectionId);
			await Clients.Others.SendAsync("Notify", $"{Context.ConnectionId} is reconnected.");
		}

	}
}




