using Entities.Entities;
using Microsoft.Extensions.Configuration;
using Repositories.Notifications;

namespace Communications.UoW
{
	/// <summary>
	/// Represents a unit of work for managing and retrieving notifications.
	/// Implements the <see cref="IDisposable"/> interface to manage resources.
	/// </summary>
	public class UnitOfWorkGetNotifications : IDisposable
	{
		/// <summary>
		/// Gets the list of received notifications.
		/// </summary>
		public List<Notification> ReceivedNotificationsList;

		private IConfiguration configuration;
		private NotificationsDbContext db;
		private NotificationRepository notificationRepository;
		private bool disposed = false;

		/// <summary>
		/// Gets the notification repository instance.
		/// </summary>
		public NotificationRepository Notifications
		{
			get
			{
				notificationRepository ??= new NotificationRepository(db);
				return notificationRepository;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UnitOfWorkGetNotifications"/> class.
		/// </summary>
		/// <param name="configuration">The application configuration.</param>
		public UnitOfWorkGetNotifications(IConfiguration configuration)
		{
			this.configuration = configuration;
			db = new NotificationsDbContext(configuration);
		}

		/// <summary>
		/// Continuously retrieves notifications until cancellation is requested.
		/// Adds new notifications to the <see cref="ReceivedNotificationsList"/> if they are not already present.
		/// </summary>
		/// <param name="cancellationToken">A token for cancelling the operation.</param>
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
				Thread.Sleep(Convert.ToInt32(configuration["HubSettings:Notify:DelayMilliseconds"]));
			}
			if (cancellationToken.IsCancellationRequested)
			{
				Dispose();
			}
		}

		/// <summary>
		/// Disposes the resources used by the <see cref="UnitOfWorkGetNotifications"/> class.
		/// </summary>
		/// <param name="disposing">A boolean indicating whether the method was called directly or by the garbage collector.</param>
		public virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
					db.Dispose();
				this.disposed = true;
			}
		}

		/// <summary>
		/// Disposes the resources used by the <see cref="UnitOfWorkGetNotifications"/> class.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}


