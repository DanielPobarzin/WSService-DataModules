using Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communications.DTO
{
	public class AlarmServerDTO
	{
		public Guid ServerId { get; set; }
		public Alarm Signal { get; set; }
		public DateTime DateAndTimeSendDataByServer { get; set; }
	}
}
