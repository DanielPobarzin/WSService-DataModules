using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
	public class Connection : Entity
	{
		public Server Server { get; set; }
		public Client Client { get; set; }
		public string? ConnectionId { get; set; }
		public ConnectionStatus Status { get; set; }
		public DateTime? TimeStampOpenConnection { get; set; }
		public DateTime? TimeStampCloseConnection { get; set; }
		public TimeSpan? Session { get; set; }
	}
}
