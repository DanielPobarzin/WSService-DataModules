using Communications.DTO;
using Entities.Entities;
using Repositories.DO;

namespace Communications.Helpers
{
	public class TransformToDOHelper
	{
		public async Task<DomainObjectNotification> TransformToDomainObject(MessageServerDTO Message, Guid ClientId)
		{
			DomainObjectNotification notification = new DomainObjectNotification
			{
				ClientId = ClientId,
				ServerId = Message.ServerId,
				MessageId = Message.Notification.Id,
				Notification = Message.Notification,
				DateAndTimeSendDataByServer = Message.DateAndTimeSendDataByServer,
				DateAndTimeRecievedDataFromServer = DateTime.Now
			};

			return await Task.FromResult(notification);
		}
	}
}