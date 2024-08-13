using Domain.Common;
using Domain.Entities;
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
		public AlarmConnection AlarmDB { get; set; }
		public NotifyConnection NotificationDB { get; set; }
	}
	public class KafkaSettings : BaseConfig
	{
		public ConsumerConnection Consumer { get; set; }
		public ProducerConnection Producer { get; set; }
	}
	public class AlarmConnection : Entity
	{
		public string ConnectionString { get; set; }
	}
	public class ConsumerConnection : Entity
	{
		public string BootstrapServers { get; set; }
	}
	public class ProducerConnection : Entity
	{
		public string BootstrapServers { get; set; }
	}

	public class NotifyConnection : Entity
	{
		public string ConnectionString { get; set; }
	}
	public class HubSettings : BaseConfig
	{
		public Guid ServerId { get; set; }
		public NotifyHubSettings Notify { get; set; }
		public AlarmHubSettings Alarm { get; set; }
	}
	public class NotifyHubSettings : Entity
	{
		public int DelayMilliseconds { get; set; }
		public string HubMethod { get; set; }
		public string TargetClients { get; set; }
	}
	public class AlarmHubSettings : Entity
	{
		public int DelayMilliseconds { get; set; }
		public string HubMethod { get; set; }
		public string TargetClients { get; set; }
	}
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
