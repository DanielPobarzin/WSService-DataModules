using Entities.Entities;
using Interactors.Interfaces;
using Microsoft.EntityFrameworkCore;
using Repositories.DO;

namespace Repositories.Notifications
{
    public class RecievedNotificationsRepository : IRepository<DomainObjectNotification>
	{
		private RecievedNotificationsDbContext db;

		public RecievedNotificationsRepository(RecievedNotificationsDbContext context)
		{
			this.db = context;
		}
		public async Task PublishMessage(DomainObjectNotification entity)
		{
			await db.Notifications.AddAsync(entity);
		}
	}
}
