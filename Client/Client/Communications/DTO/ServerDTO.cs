namespace Communications.DTO
{
	public abstract class ServerDTO
	{
		public Guid ServerId { get; set; }
		public DateTime DateAndTimeSendDataByServer { get; set; }
	}
}
