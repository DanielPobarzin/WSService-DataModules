using Entities.Entities;
using Interactors.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Alarms
{
    public class AlarmRepository : IRepository<Alarm>
	{
		private AlarmsDbContext db;

		public AlarmRepository(AlarmsDbContext context)
		{
			this.db = context;
		}
		public IEnumerable<Alarm> GetAllList()
		{
			return db.Alarms;
		}
	}
}
