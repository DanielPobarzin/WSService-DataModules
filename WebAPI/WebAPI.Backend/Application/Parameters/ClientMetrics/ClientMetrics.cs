using Domain.Enums;

namespace Application.Parameters.ClientMetrics
{
	public class ClientMetrics
	{
		public Guid ClientId { get; set; }
		public WorkStatus WorkStatus {  get; set; }
		public ConnectionStatus ConnectionStatus { get; set; }
		public long TotalMessagesSize { get; set; }
		public int TotalCountMessages { get; set; }
		public int CountAlarms { get; set; }
		public int CountNotifications { get; set; }
		public long AverageMessageSize { get; set; }
		public long WorkingMemoryUsage { get; set; }
		public long PrivateMemoryUsage { get; set; }
		public TimeSpan Latency { get; set; }
	}
}

