using Entities.Entities;
using Interactors.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Notifications
{
    public class RecievedNotificationRepository : IRepository<RecievedByClientNotification>
	{
		private RecievedNotificationsDbContext db;

		public RecievedNotificationRepository(RecievedNotificationsDbContext context)
		{
			this.db = context;
		}
		public async Task<RecievedByClientNotification> PublishMessage(RecievedByClientNotification entity)
		{
			var addedEntity = await db.Notifications.AddAsync(entity);
			await db.SaveChangesAsync();
			return addedEntity.Entity;
		}
	}
}
