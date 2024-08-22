namespace Entities.Settings
{
	public class DBSettings 
	{
		public string DataBase { get; set; }
		public AlarmConnection Alarm { get; set; }
		public NotifyConnection Notify { get; set; }
	}
	public class KafkaSettings
	{
		public ConsumerConnection Consumer { get; set; }
		public ProducerConnection Producer { get; set; }
	}
	public class AlarmConnection 
	{
		public string ConnectionString { get; set; }
	}
	public class ConsumerConnection 
	{
		public string BootstrapServers { get; set; }
	}
	public class ProducerConnection 
	{
		public string BootstrapServers { get; set; }
	}

	public class NotifyConnection 
	{
		public string ConnectionString { get; set; }
	}
	public class HubSettings 
	{
		public Guid ServerId { get; set; }
		public NotifyHubSettings Notify { get; set; }
		public AlarmHubSettings Alarm { get; set; }
	}
	public class NotifyHubSettings 
	{
		public int DelayMilliseconds { get; set; }
		public string HubMethod { get; set; }
		public string TargetClients { get; set; }
	}
	public class AlarmHubSettings 
	{
		public int DelayMilliseconds { get; set; }
		public string HubMethod { get; set; }
		public string TargetClients { get; set; }
	}
	public class HostSettings 
	{
		public int Port { get; set; }
		public string Urls { get; set; }
		public string PolicyName { get; set; }
		public string AllowedOrigins { get; set; }
		public string RouteNotify { get; set; }
		public string RouteAlarm { get; set; }
	}
}
