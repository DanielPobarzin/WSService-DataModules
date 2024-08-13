namespace Entities.Settings
{
    public class Settings 
    {
        public ConnectSettings ConnectSettings { get; set; }
        public DBSettings DBSettings { get; set; }
        public CLientSettings CLientSettings { get; set; }
        public Kafka Kafka { get; set; }
    }
}
