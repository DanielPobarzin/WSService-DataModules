namespace Domain.Settings.SignalRClient
{
	public class CLientSettings 
	{
		public ConnectSettings ConnectionSettings { get; set; }
		public DBSettings DBConnection { get; set; }
		public ClientSettings ClientSettings { get; set; }
		public KafkaSettings Kafka { get; set; }
	}
}
