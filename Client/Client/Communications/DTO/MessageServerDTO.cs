using Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communications.DTO
{
	public class MessageServerDTO : ServerDTO
	{
		public Notification Notification { get; set; }
	}
}
