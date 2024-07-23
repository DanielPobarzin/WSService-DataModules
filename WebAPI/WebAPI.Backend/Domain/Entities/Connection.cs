using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Entities
{
	public class Connection
	{
		[Required]
		public Guid ServerId { get; set; }
		[Required]
		public Guid ClientId { get; set; }
		[Required]
		public string ConnectionId { get; set; }
		[Required]
		public ConnectionStatus Status { get; set; }
		public DateTime TimeStampOpenConnection { get; set; }
		public DateTime TimeStampCloseConnection { get; set; }
		public TimeSpan Session { get; set; }
	}
}
