using Domain.Enums;

namespace Domain.Entities
{
	public class Client : Entity
	{
		public Guid Id { get; set; }
		public WorkStatus WorkStatus { get; set; }
		public ConnectionStatus ConnectionStatus { get; set; }
		public string? ConnectionId { get; set; }

		public ICollection<Connection> Connections { get; set; }
	}
}
