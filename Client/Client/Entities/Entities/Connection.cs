using Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Entities
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
