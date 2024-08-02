using Microsoft.Extensions.Configuration;
using Repositories.DO;
using Repositories.Notifications;

namespace Communications.UoW
{
	/// <summary>
	/// The class of the unit of work for publishing notifications to the database.
	/// </summary>
	/// <remarks>
	/// Implementation of the "Repository" pattern.
	/// </remarks>
	public class UnitOfWorkPublishNotifications : IDisposable
	{
		private RecievedNotificationsDbContext db;
		private RecievedNotificationsRepository notificationRepository;
		private IConfiguration configuration;
		private bool disposed = false;
		/// <summary>
		/// Gets the repository for managing notifications.
		/// </summary>
		/// <value>
		/// An instance of <see cref="RecievedNotificationsRepository"/> used to interact with notification data.
		/// </value>
		public RecievedNotificationsRepository Notifications
		{
			get
			{
				notificationRepository ??= new RecievedNotificationsRepository(db);
				return notificationRepository;
			}
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="UnitOfWorkPublishNotifications"/> class.
		/// </summary>
		/// <param name="configuration">The configuration settings for the database context.</param>
		/// <remarks>
		/// This constructor creates an instance of the <see cref="RecievedNotificationsDbContext"/>
		/// using the provided configuration.
		/// </remarks>
		public UnitOfWorkPublishNotifications(IConfiguration configuration)
		{
			this.configuration = configuration;
			db = new RecievedNotificationsDbContext(this.configuration);
		}
		/// <summary>
		/// Publishes an notification message to the database if it does not already exist.
		/// </summary>
		/// <param name="message">The notification message to be published.</param>
		/// <returns>A task that represents the asynchronous operation.</returns>
		/// <remarks>
		/// This method checks if an alarm with the same message ID already exists.
		/// If it does not exist, it proceeds to publish the message.
		/// </remarks>
		public async Task PublishNotifications(DomainObjectNotification message)
		{
			if (await Notifications.FetchByIdMessage(message.MessageId) == null)
				await Notifications.PublishMessage(message);
		}
		/// <summary>
		/// Disposes resources used by the <see cref="UnitOfWorkPublishNotifications"/> class.
		/// </summary>
		/// <param name="disposing">A boolean indicating whether the method has been called directly or by the garbage collector.</param>
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
		/// Disposes resources used by the <see cref="UnitOfWorkPublishNotifications"/> class.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}


