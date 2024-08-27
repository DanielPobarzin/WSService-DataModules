using Application.Wrappers;
using MediatR;

namespace Application.Features.Configurations.Server.Commands.CreateConfig
{
	/// <summary>
	/// CreateConfigServerCommand - handles IRequest
	/// BaseRequestParameter - contains the entity of the server configuration
	/// BaseResponseParameter - contains the Id of the server configuration and the result of the execution (successful/unsuccessful)
	/// Тo create a new server configuration, use the properties from the body of this class
	/// </summary>
	public class CreateConfigServerCommand : IRequest<Response<Guid>>
	{
		public Guid SystemId { get; set; }
		public int Port { get; set; }
		public string Urls { get; set; }
		public string PolicyName { get; set; }
		public string AllowedOrigins { get; set; }
		public string RouteNotify { get; set; }
		public string RouteAlarm { get; set; }
		public int AlarmDelayMilliseconds { get; set; }
		public int NotifyDelayMilliseconds { get; set; }
		public string NotifyHubMethod { get; set; }
		public string AlarmHubMethod { get; set; }
		public string NotifyTargetClients { get; set; }
		public string AlarmTargetClients { get; set; }
		public string DB { get; set; }
		public string AlarmDB { get; set; }
		public string NotificationDB { get; set; }
		public string ConsumerBootstrapServer { get; set; }
		public string ProducerBootstrapServer { get; set; }
	}
}
