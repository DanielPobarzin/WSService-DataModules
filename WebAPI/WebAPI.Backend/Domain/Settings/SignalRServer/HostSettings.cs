using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Settings.SignalRServer
{
	public class HostSettings : BaseConfig
	{
        public int Port { get; set; }
        public string Urls { get; set; }
        public string PolicyName { get; set; }
        public string AllowedOrigins { get; set; }
        public string RouteNotify { get; set; }
        public string RouteAlarm { get; set; }
    }
}
