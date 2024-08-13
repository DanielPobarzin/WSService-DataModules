namespace Entities.Settings
{
	public class ServerSettings 
	{
		public DBSettings ServerDB { get; set; }
		public HostSettings ServerHost { get; set; }
		public HubSettings ServerHub { get; set; }
		public KafkaSettings ServerKafka { get; set; }
	}
}
