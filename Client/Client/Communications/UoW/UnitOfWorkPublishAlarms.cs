using Microsoft.Extensions.Configuration;
using Repositories.Alarms;
using Repositories.DO;

namespace Communications.UoW
{
	/// <summary>
	/// The class of the unit of work for publishing alarms to the database.
	/// </summary>
	/// <remarks>
	/// Implementation of the "Repository" pattern.
	/// </remarks>
	public class UnitOfWorkPublishAlarms : IDisposable
	{
		private RecievedAlarmsDbContext db;
		private RecievedAlarmsRepository alarmRepository;
		private IConfiguration configuration;
		private bool disposed = false;
		/// <summary>
		/// Gets the repository for managing alarms.
		/// </summary>
		/// <value>
		/// An instance of <see cref="RecievedAlarmsRepository"/> used to interact with alarm data.
		/// </value>
		public RecievedAlarmsRepository Alarms
		{
			get
			{
				alarmRepository ??= new RecievedAlarmsRepository(db);
				return alarmRepository;
			}
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="UnitOfWorkPublishAlarms"/> class.
		/// </summary>
		/// <param name="configuration">The configuration settings for the database context.</param>
		/// <remarks>
		/// This constructor creates an instance of the <see cref="RecievedAlarmsDbContext"/>
		/// using the provided configuration.
		/// </remarks>
		public UnitOfWorkPublishAlarms(IConfiguration configuration)
		{
			this.configuration = configuration;
			db = new RecievedAlarmsDbContext(this.configuration);
		}
		/// <summary>
		/// Publishes an alarm message to the database if it does not already exist.
		/// </summary>
		/// <param name="message">The alarm message to be published.</param>
		/// <returns>A task that represents the asynchronous operation.</returns>
		/// <remarks>
		/// This method checks if an alarm with the same message ID already exists.
		/// If it does not exist, it proceeds to publish the message.
		/// </remarks>
		public async Task PublishAlarms(DomainObjectAlarm message)
		{
			if (await Alarms.FetchByIdMessage(message.MessageId) == null)
				await Alarms.PublishMessage(message);
		}
		/// <summary>
		/// Disposes resources used by the <see cref="UnitOfWorkPublishAlarms"/> class.
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
		/// Disposes resources used by the <see cref="UnitOfWorkPublishAlarms"/> class.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}


