using Entities.Entities;
using Interactors.Interfaces;
using Microsoft.EntityFrameworkCore;
using Repositories.DO;

namespace Repositories.Notifications
{
    public class RecievedNotificationsRepository : GenericRepository<DomainObjectNotification, RecievedNotificationsDbContext>, IRepository<DomainObjectNotification>
	{
		public RecievedNotificationsRepository(RecievedNotificationsDbContext context) : base(context)
		{
			DbInitializer<RecievedNotificationsDbContext>.Initialize(context);
		}
	}
}
