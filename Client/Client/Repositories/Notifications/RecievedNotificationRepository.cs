using Interactors.Interfaces;
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
