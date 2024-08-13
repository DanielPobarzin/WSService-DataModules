using Application.Wrappers;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Configurations.Client.Commands.UpdateConfig
{
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
