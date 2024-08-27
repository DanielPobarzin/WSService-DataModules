using Domain.Enums;

namespace Domain.Entities
{
	public class Connection : Entity
	{
		public Guid ServerId { get; set; }
		public Guid ClientId { get; set; }
		public string? ConnectionId { get; set; }
		public ConnectionStatus Status { get; set; }
		public DateTime? TimeStampOpenConnection { get; set; }
		public DateTime? TimeStampCloseConnection { get; set; }
		public TimeSpan? Session { get; set; }

		public virtual Client Client { get; set; }
		public virtual Server Server { get; set; }
	}
}
