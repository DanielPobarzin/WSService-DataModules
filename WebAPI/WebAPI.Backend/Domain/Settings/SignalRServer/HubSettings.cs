using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Settings.SignalRServer
{
	public class HubSettings : BaseConfig
	{
        public int AlarmDelayMilliseconds { get; set; }
		public int NotifyDelayMilliseconds { get; set; }
		public string NotifyHubMethod { get; set; }
		public string AlarmHubMethod { get; set; }
		public string NotifyTargetClients { get; set; }
		public string AlarmTargetClients { get; set; }
	}
}
