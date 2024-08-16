using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Parameters.ServerMetrics
{
	public class ServerMetrics
	{
		public Guid ServerId { get; set; }
		public WorkStatus WorkStatus { get; set; }
		public ConnectionStatus ConnectionStatus { get; set; }
		public int CountListeners { get; set; }
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

