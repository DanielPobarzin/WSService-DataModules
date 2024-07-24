using Entities.Entities;
using Microsoft.Extensions.Configuration;
using Repositories.Alarms;

namespace Communications.UoW
{
	public class UnitOfWorkGetAlarms : IDisposable
	{
		public List<Alarm> ReceivedAlarmsList;
		private IConfiguration configuration;
		private AlarmsDbContext db;
		private AlarmRepository alarmRepository;
		private bool disposed = false;

		public AlarmRepository Alarms
		{
			get
			{
				alarmRepository ??= new AlarmRepository(db);
				return alarmRepository;
			}
		}
		public UnitOfWorkGetAlarms(IConfiguration configuration)
		{
			this.configuration = configuration;
			db = new AlarmsDbContext(configuration);
		}
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