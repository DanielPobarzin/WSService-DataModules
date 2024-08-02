using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communications.DTO
{
	public abstract class ServerDTO
	{
		public Guid ServerId { get; set; }
		public DateTime DateAndTimeSendDataByServer { get; set; }
	}
}
