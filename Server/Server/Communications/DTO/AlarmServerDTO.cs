using Entities.Entities;

namespace Communications.DTO
{
	public class AlarmServerDTO
	{
		public Guid ServerId { get; set; }
		public Alarm Alarm { get; set; }
		public DateTime DateAndTimeSendDataByServer { get; set; }
	}
}
