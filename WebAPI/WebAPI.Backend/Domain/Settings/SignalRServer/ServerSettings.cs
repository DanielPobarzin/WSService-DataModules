using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Settings.SignalRServer
{
    public class ServerSettings : BaseConfig
    {
		public DBSettings DB { get; set; }
		public HostSettings Host { get; set; }
        public HubSettings Hub { get; set; }
	}
}
