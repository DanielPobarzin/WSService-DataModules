using Entities.Entities;
using Microsoft.Extensions.Configuration;
using Repositories.Alarms;
using Repositories.DO;
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
	public class UnitOfWorkPublishAlarms : IDisposable
	{
		private RecievedAlarmsDbContext db;
		private RecievedAlarmsRepository alarmRepository;
		private IConfiguration configuration;
		private bool disposed = false;
		public RecievedAlarmsRepository Alarms
		{
			get
			{
				alarmRepository ??= new RecievedAlarmsRepository(db);
				return alarmRepository;
			}
		}
		public UnitOfWorkPublishAlarms(IConfiguration configuration)
		{
			this.configuration = configuration;
			db = new RecievedAlarmsDbContext(this.configuration);
		}
		public async Task PublishAlarms(DomainObjectAlarm message)
		{
			await Alarms.PublishMessage(message);
		}
		public void Save()
		{
			db.SaveChanges();
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


