namespace Repositories.DO
{
	public abstract class DomainObject 
	{
		public virtual Guid RecipientId { get; set; }
		public virtual Guid SenderId { get; set; }
		public Guid MessageId { get; set; }
		public DateTime DateAndTimeSendDataByServer { get; set; }
		public DateTime DateAndTimeRecievedDataFromServer { get; set; }
	}
}
