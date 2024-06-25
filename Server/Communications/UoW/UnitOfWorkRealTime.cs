using Communications.Hubs;
using Entities.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communications.UoW
{
	public class UnitOfWorkRealTime
	{
		private readonly IHubContext<NotificationHub> _hubContext;
		private readonly IHubCallerClients<NotificationHub> _hubCallerClients;
		private readonly IEnumerable<Notification>? _notifications;
		private readonly IConfiguration _configuration;

		private IMemoryCache memoryCache;

		public UnitOfWorkRealTime (IHubContext<NotificationHub> hubContext,
								   IHubCallerClients<NotificationHub> hubCallerClients, 
			                       IEnumerable<Notification>? notifications, 
								   IConfiguration configuration)
		{
			_hubContext = hubContext;
			_hubCallerClients = hubCallerClients;
			_configuration = configuration;
			_notifications = notifications;
		}

		public async void RealTimeNotify(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				if (_notifications != null)
				{
					foreach (var notification in _notifications)
					{
						Console.WriteLine(notification.Content);
						try
						{
							if (memoryCache.Get<Notification>(notification.Id) == null)
							{
								memoryCache.Set(notification.Id, notification);
								_hubContext.Clients.All.SendAsync("R", notification);
							}
						}
						catch (Exception ex) { Console.WriteLine($"{ex}"); }

					}
					await Task.Delay(Convert.ToInt32(_configuration["NotificationsHubSettings:DelayMilliseconds"]));
				}

			}
		}

	}
}
