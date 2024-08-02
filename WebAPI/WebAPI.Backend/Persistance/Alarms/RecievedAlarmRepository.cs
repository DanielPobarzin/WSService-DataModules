using Entities.Entities;
using Interactors.Interfaces;
using Microsoft.EntityFrameworkCore;
using Repositories.DO;
using Repositories.Notifications;

namespace Repositories.Alarms
{
    public class RecievedAlarmsRepository : GenericRepository<DomainObjectAlarm, RecievedAlarmsDbContext>, IRepository<DomainObjectAlarm>
	{
		public RecievedAlarmsRepository(RecievedAlarmsDbContext context) : base(context)
		{
			DbInitializer<RecievedAlarmsDbContext>.Initialize(context);
		}
	}
}
