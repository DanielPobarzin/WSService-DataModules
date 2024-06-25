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
		public IEnumerable<Notification> NotificationsList;

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
		public async void GetAllNotifications(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				this.NotificationsList = Notifications.GetAllList();
				await Task.Delay(Convert.ToInt32(1000));
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


