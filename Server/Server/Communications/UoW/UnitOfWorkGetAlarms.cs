using Entities.Entities;
using Microsoft.Extensions.Configuration;
using Repositories.Alarms;

namespace Communications.UoW
{
	/// <summary>
	/// Represents a unit of work for retrieving alarms from the database.
	/// Implements the <see cref="IDisposable"/> interface to manage resources.
	/// </summary>
	public class UnitOfWorkGetAlarms : IDisposable
	{
		/// <summary>
		/// Gets the list of received alarms.
		/// </summary>
		public List<Alarm> ReceivedAlarmsList;
		private IConfiguration configuration;
		private AlarmsDbContext db;
		private AlarmRepository alarmRepository;
		private bool disposed = false;

		/// <summary>
		/// Gets the instance of the <see cref="AlarmRepository"/> associated with this unit of work.
		/// </summary>
		public AlarmRepository Alarms
		{
			get
			{
				alarmRepository ??= new AlarmRepository(db);
				return alarmRepository;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UnitOfWorkGetAlarms"/> class.
		/// </summary>
		/// <param name="configuration">The configuration settings for the application.</param>
		public UnitOfWorkGetAlarms(IConfiguration configuration)
		{
			this.configuration = configuration;
			db = new AlarmsDbContext(configuration);
		}

		/// <summary>
		/// Continuously retrieves alarms from the database until cancellation is requested.
		/// </summary>
		/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
		public void GetAllAlarms(CancellationToken cancellationToken)
		{
			ReceivedAlarmsList = new List<Alarm>();

			while (!cancellationToken.IsCancellationRequested)
			{
				foreach (var alarm in Alarms.GetAllList())
				{
					if (!ReceivedAlarmsList.Contains(alarm))
					{
						ReceivedAlarmsList.Add(alarm);
					}
				}
				Thread.Sleep(Convert.ToInt32(configuration["HubSettings:Alarm:DelayMilliseconds"]));
			}
			if (cancellationToken.IsCancellationRequested)
			{
				Dispose();
			}
		}

		/// <summary>
		/// Releases the unmanaged resources used by the <see cref="UnitOfWorkGetAlarms"/> class 
		/// and optionally releases the managed resources.
		/// </summary>
		/// <param name="disposing">If true, managed resources will be disposed; otherwise, only unmanaged resources will be released.</param>
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
		/// Releases all resources used by the <see cref="UnitOfWorkGetAlarms"/> class.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

	}
}