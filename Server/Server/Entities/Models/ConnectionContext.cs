using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
	public class ConnectionContext
	{
		public Guid ServerId { get; set; }
		public string ConnectionId { get; set; }
		public string HubRoute { get; set; }
		public Guid ClientId {  get; set; }
		public DateTime? StartConnection {  get; set; }
		public DateTime? EndConnection { get; set; }
		public TimeSpan? Session { get; set; }


	}
}
