using Entities.Entities;
using Interactors.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Notifications
{
    public class NotificationRepository : IRepository<Notification>
	{
		private NotificationsDbContext db;

		public NotificationRepository(NotificationsDbContext context)
		{
			this.db = context;
		}
		public IEnumerable<Notification> GetAllList()
		{
			return db.Notifications;
		}
	}
}
