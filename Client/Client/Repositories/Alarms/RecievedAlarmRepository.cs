using Interactors.Interfaces;
using Repositories.DO;

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
