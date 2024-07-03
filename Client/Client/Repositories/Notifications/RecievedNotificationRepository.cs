using Entities.Entities;
using Interactors.Interfaces;
using Microsoft.EntityFrameworkCore;
using Repositories.DO;

namespace Repositories.Notifications
{
    public class RecievedNotificationRepository : IRepository<DomainObjectNotification>
	{
		private RecievedNotificationsDbContext db;

		public RecievedNotificationRepository(RecievedNotificationsDbContext context)
		{
			this.db = context;
		}
		public async Task PublishMessage(DomainObjectNotification entity)
		{
			await db.Notifications.AddAsync(entity);
		}
	}
}
