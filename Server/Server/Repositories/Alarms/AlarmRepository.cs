using Entities.Entities;
using Interactors.Interfaces;

namespace Repositories.Alarms
{
	public class AlarmRepository : IRepository<Alarm>
	{
		private readonly AlarmsDbContext db;

		public AlarmRepository(AlarmsDbContext context)
		{
			db = context;
		}
		public IEnumerable<Alarm> GetAllList()
		{
			return db.Alarms;
		}
	}
}
