using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Domain.Common;

namespace WebAPI.Domain.Entities
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
		public ConnectionsStatus Status { get; set; }
		public DateTime TimeStampOpenConnection { get; set; }
		public DateTime TimeStampCloseConnection { get; set; }
		public TimeSpan Session { get; set; }
	}
}
