using Communications.DTO;
using Entities.Entities;

namespace Communications.Helpers
{
	public class TransformToDTOHelper
	{
		public async Task<MessageServerDTO> TransformToNotificationDTO(Notification notification, Guid serverId)
		{
			MessageServerDTO messageServerDTOs = new MessageServerDTO
			{
				ServerId = serverId,
				Notification = notification,
				DateAndTimeSendDataByServer = DateTime.Now
			};

			return await Task.FromResult(messageServerDTOs);
		}

		public async Task<AlarmServerDTO> TransformToAlarmDTO(Alarm alarm, Guid serverId)
		{
			AlarmServerDTO alarmServerDTOs = new AlarmServerDTO
			{
				ServerId = serverId,
				Alarm = alarm,
				DateAndTimeSendDataByServer = DateTime.Now
			};

			return await Task.FromResult(alarmServerDTOs);
		}
	}
}
