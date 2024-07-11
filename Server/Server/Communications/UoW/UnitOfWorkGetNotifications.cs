using Entities.Entities;
using Microsoft.Extensions.Configuration;
using Repositories.Notifications;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Communications.UoW
{
	public class UnitOfWorkGetNotifications : IDisposable
	{
		public List<Notification> ReceivedNotificationsList;

		private IConfiguration configuration;
		private NotificationsDbContext db;
		private NotificationRepository notificationRepository;
		private bool disposed = false;
		public NotificationRepository Notifications
		{
			get
			{
				notificationRepository ??= new NotificationRepository(db);
				return notificationRepository;
			}
		}
		public UnitOfWorkGetNotifications(IConfiguration configuration)
		{
			this.configuration = configuration;
			db = new NotificationsDbContext(configuration);
		}
		public void GetAllNotifications(CancellationToken cancellationToken)
		{
			ReceivedNotificationsList = new List<Notification>();

			while (!cancellationToken.IsCancellationRequested)
			{
				var notifications = Notifications.GetAllList();
				foreach(var notification in notifications)
				{
					if (!ReceivedNotificationsList.Contains(notification))
					{
						ReceivedNotificationsList.Add(notification);
					}
				}
				Thread.Sleep(Convert.ToInt32(100));
			}
			if (cancellationToken.IsCancellationRequested)
			{
				Dispose();
			}
		}
		public virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
					db.Dispose();
				this.disposed = true;
			}
		}
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}


