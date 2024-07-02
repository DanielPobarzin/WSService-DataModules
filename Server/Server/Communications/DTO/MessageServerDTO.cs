using Entities.Entities;

namespace Communications.DTO
{
	public class MessageServerDTO
	{
		public Guid ServerId { get; set; }
		public Notification Notification { get; set; }
		public DateTime DateAndTimeSendDataByServer { get; set; }
	}
}
