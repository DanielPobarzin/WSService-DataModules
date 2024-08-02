using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Share.KafkaMessage
{
	public abstract class KafkaMessageBase
	{
		public Guid ClientId { get; set; }
	}
}
