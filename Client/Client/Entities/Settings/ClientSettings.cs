namespace Entities.Settings
{
    public class ClientSettings 
    {
        public ConnectSettings ConnectionSettings { get; set; }
        public DBSettings DBConnection { get; set; }
        public CLientSettings CLientSettings { get; set; }
        public Kafka Kafka { get; set; }
    }
}
