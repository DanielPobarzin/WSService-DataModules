
using Entities.Entities;
using Entities.Enums;

namespace Entities.Settings
{
	public class DBSettings 
    {
        public string DataBase { get; set; }
        public AlarmDataBase Alarm { get; set; }
        public NotifyDataBase Notify { get; set; }
    }
    public class Kafka
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

    public class CLientSettings
	{
        public Guid Id { get; set; }
        public bool UseCache { get; set; }
        public ConnectionMode Mode { get; set; }
    }
}
