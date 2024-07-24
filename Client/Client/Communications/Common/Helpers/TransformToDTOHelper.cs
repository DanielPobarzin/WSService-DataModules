using Communications.DTO;
using Entities.Entities;
using Repositories.DO;

namespace Communications.Helpers
{
	public class TransformToDOHelper
	{
		public async Task<DomainObjectNotification> TransformToDomainObjectNotification(MessageServerDTO Message, Guid ClientId)
		{
			DomainObjectNotification notification = new DomainObjectNotification
			{
				RecipientId = ClientId,
				SenderId = Message.ServerId,
				MessageId = Message.Notification.Id,
				Notification = Message.Notification,
				DateAndTimeSendDataByServer = Message.DateAndTimeSendDataByServer,
				DateAndTimeRecievedDataFromServer = DateTime.Now
			};

			return await Task.FromResult(notification);
		}
		public async Task<DomainObjectAlarm> TransformToDomainObjectAlarm(AlarmServerDTO Message, Guid ClientId)
		{
			DomainObjectAlarm alarm = new DomainObjectAlarm
			{
				RecipientId = ClientId,
				SenderId = Message.ServerId,
				MessageId = Message.Alarm.Id,
				Alarm = Message.Alarm,
				DateAndTimeSendDataByServer = Message.DateAndTimeSendDataByServer,
				DateAndTimeRecievedDataFromServer = DateTime.Now
			};

			return await Task.FromResult(alarm);
		}
	}
}