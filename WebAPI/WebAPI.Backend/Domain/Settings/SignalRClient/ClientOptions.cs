using Domain.Common;
using Domain.Entities;
using Domain.Enums;

namespace Domain.Settings.SignalRClient
{
	public class DBSettings 
	{
		public string DataBase { get; set; }
		public AlarmDataBase Alarm { get; set; }
		public NotifyDataBase Notify { get; set; }
	}
	public class KafkaSettings 
	{
		public ConsumerConnection Consumer { get; set; }
		public ProducerConnection Producer { get; set; }
	}
	public class ConsumerConnection : Entity
	{
		public string BootstrapServers { get; set; }
	}
	public class ProducerConnection : Entity
	{
		public string BootstrapServers { get; set; }
	}

	public class AlarmDataBase : Entity
	{
		public string ConnectionString { get; set; }
	}

	public class NotifyDataBase : Entity
	{
		public string ConnectionString { get; set; }
	}

	public class ConnectSettings 
	{
		public NotifyConnection Notify { get; set; }
		public AlarmConnection Alarm { get; set; }
	}

	public class NotifyConnection : Entity
	{
		public string Url { get; set; }
	}

	public class AlarmConnection : Entity
	{
		public string Url { get; set; }
	}

	public class ModeSettings 
	{
		public Guid ClientId { get; set; }
		public bool UseCache { get; set; }
		public ConnectionMode Mode { get; set; }
	}
}
