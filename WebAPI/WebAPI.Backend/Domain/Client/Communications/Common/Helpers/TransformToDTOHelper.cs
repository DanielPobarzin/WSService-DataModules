using Communications.DTO;
using Repositories.DO;

namespace Communications.Helpers
{
	/// <summary>
	/// Helper class for transforming server DTOs into domain objects for notifications and alarms.
	/// </summary>
	public class TransformToDOHelper
    {
    /// <summary>
    /// Transforms a <see cref="MessageServerDTO"/> into a <see cref="DomainObjectNotification"/>.
    /// </summary>
    /// <param name="Message">The message data transfer object containing notification details.</param>
    /// <param name="ClientId">The unique identifier of the recipient client.</param>
    /// <returns>A task that represents the asynchronous operation, containing the transformed <see cref="DomainObjectNotification"/>.</returns>
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

    /// <summary>
    /// Transforms an <see cref="AlarmServerDTO"/> into a <see cref="DomainObjectAlarm"/>.
    /// </summary>
    /// <param name="Message">The alarm data transfer object containing alarm details.</param>
    /// <param name="ClientId">The unique identifier of the recipient client.</param>
    /// <returns>A task that represents the asynchronous operation, containing the transformed <see cref="DomainObjectAlarm"/>.</returns>
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