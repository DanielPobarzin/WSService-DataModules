using Entities.Entities;
using Interactors.Interfaces;
using Microsoft.EntityFrameworkCore;
using Repositories.DO;

namespace Repositories.Alarms
{
    public class RecievedAlarmsRepository : IRepository<DomainObjectAlarm>
	{
		private RecievedAlarmsDbContext db;

		public RecievedAlarmsRepository(RecievedAlarmsDbContext context)
		{
			this.db = context;
		}
		public async Task PublishMessage(DomainObjectAlarm entity)
		{
			await db.Alarms.AddAsync(entity);
		}
	}
}
