using Interactors.Enums;

namespace Interactors.Models
{
	public class Connection
	{
		public Guid ServerId { get; set; }
		public Guid ClientId { get; set; }
		public string ConnectionId { get; set; }
		public ConnectionStatus Status { get; set; }
		public DateTime? TimeStampOpenConnection { get; set; }
		public DateTime? TimeStampCloseConnection { get; set; }
		public TimeSpan? Session { get; set; }
	}
}
