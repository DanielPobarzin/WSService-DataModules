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
		public DBSettings ServerDB { get; set;}
		public HostSettings ServerHost { get; set;}
		public HubSettings ServerHub { get; set;}
		public KafkaSettings ServerKafka { get; set; }
	}
}
