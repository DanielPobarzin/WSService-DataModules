using Domain.Common;

namespace Domain.Settings.SignalRClient
{
	public class ClientSettings : BaseConfig
	{
		public ConnectSettings ConnectSettings { get; set; }
		public DBSettings DBSettings { get; set; }
		public ModeSettings ModeSettings { get; set; }
		public KafkaSettings KafkaSettings { get; set; }
	}
}
