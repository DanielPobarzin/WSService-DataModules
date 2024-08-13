using Application.Wrappers;
using Domain.Enums;
using Domain.Settings.SignalRServer;
using MediatR;

namespace Application.Features.Configurations.Client.Commands.CreateConfig
{
	public class CreateConfigClientCommand : IRequest<Response<Guid>>
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
