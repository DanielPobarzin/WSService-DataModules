using Entities.Entities;
using Interactors.Interfaces;

namespace Repositories.Notifications
{
	public class NotificationRepository : IRepository<Notification>
	{
		private readonly NotificationsDbContext db;

		public NotificationRepository(NotificationsDbContext context)
		{
			db = context;
		}
		public IEnumerable<Notification> GetAllList()
		{
			return db.Notifications;
		}
	}
}
