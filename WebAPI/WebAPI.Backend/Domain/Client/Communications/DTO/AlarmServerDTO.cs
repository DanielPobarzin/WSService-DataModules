using Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communications.DTO
{
	public class AlarmServerDTO : ServerDTO
	{
			public Alarm Alarm { get; set; }
	}
}
