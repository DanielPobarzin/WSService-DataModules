using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Settings.SignalRClient
{
	public class ConnectSettings : BaseConfig
	{
		public string NotifyUrl { get; set; }
		public string AlarmUrl { get; set; }
	}
}
