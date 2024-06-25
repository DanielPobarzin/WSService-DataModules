using Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communications.DTO
{
	public class MessageServerDTO
	{
		public Guid ServerId { get; set; }
		public Notification Notification { get; set; }
		public DateTime DateAndTimeGetDataByServer { get; set; }
	}
}
