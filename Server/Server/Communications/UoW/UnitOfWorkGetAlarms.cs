using Entities.Entities;
using Microsoft.Extensions.Configuration;
using Repositories.Alarms;
using Repositories.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
			db = new AlarmsDbContext(this.configuration);
		}
		public void GetAllAlarms(CancellationToken cancellationToken)
		{
			ReceivedAlarmsList = new List<Alarm>();

			while (!cancellationToken.IsCancellationRequested)
			{
				var alarms = Alarms.GetAllList();
				foreach (var alarm in alarms)
				{
					if (!ReceivedAlarmsList.Contains(alarm))
					{
						ReceivedAlarmsList.Add(alarm);
					}
				}
				Thread.Sleep(Convert.ToInt32(1000));
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