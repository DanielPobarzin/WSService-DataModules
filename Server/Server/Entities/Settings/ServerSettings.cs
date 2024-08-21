namespace Entities.Settings
{
	public class ServerSettings 
	{
		public DBSettings DbConnection { get; set; }
		public HostSettings HostSettings { get; set; }
		public HubSettings HubSettings { get; set; }
		public KafkaSettings Kafka { get; set; }
	}
}
