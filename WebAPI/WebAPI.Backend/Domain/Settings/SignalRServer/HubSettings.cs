using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Settings.SignalRServer
{
	public class HubSettings
    {
        public int DelayMilliseconds { get; set; }
        public string HubMethod { get; set; }
        public string TargetClients { get; set; }
    }
}
