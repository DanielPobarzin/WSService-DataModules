using Application.Wrappers;
using Domain.Enums;
using MediatR;

namespace Application.Features.Configurations.Client.Commands.UpdateConfig
{
	/// <summary>
	/// UpdateClientCommand - handles IRequest
	/// BaseRequestParameter - contains the entity of the client configuration
	/// BaseResponseParameter - contains the Id of the client configuration and the result of the execution (successful/unsuccessful)
	/// Тo update a client configuration, use the properties from the body of this class
	/// </summary>
	/// <remarks>
	/// System Id - unique identifier for the client configuration (& client)
	/// DB - the data storage environment or the DBMS used
	/// AlarmDB - connection string to the database with alarm messages 
	/// NotificationDB - connection string to the database with notification messages 
	/// AlarmUrl - The URL of the connection to the alarm hub on the server
	/// NotifyUrl - The URL of the connection to the notification hub on the server
	/// UseCache - the option to use local caching on the client side
	/// Mode - the option of the client's connection mode <see cref="ConnectionMode"/>
	/// ConsumerBootstrapServer - the address and port of connection to Kafka brokers for the consumer
	/// ProducerBootstrapServer - the address and port of connection to Kafka brokers for the producer
	/// </remarks>
	public class UpdateConfigClientCommand : IRequest<Response<Guid>>
	{
		public Guid SystemId { get; set; }
		public string DB { get; set; }
		public string AlarmDB { get; set; }
		public string NotificationDB { get; set; }
		public string NotifyUrl { get; set; }
		public string AlarmUrl { get; set; }
		public bool UseCache { get; set; }
		public ConnectionMode Mode { get; set; }
		public string ConsumerBootstrapServer { get; set; }
		public string ProducerBootstrapServer { get; set; }
	}
}
