using Microsoft.Extensions.Configuration;
using Repositories.Alarms;
using Repositories.DO;

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
			if (await Alarms.FetchByIdMessage(message.MessageId) == null)
				await Alarms.PublishMessage(message);
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


