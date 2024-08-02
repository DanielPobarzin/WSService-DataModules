using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Settings.SignalRServer
{
	public class DBSettings : BaseConfig
	{
		public string DB { get; set; }
		public string AlarmDB { get; set; }
		public string NotificationDB { get; set; }
	}
}
