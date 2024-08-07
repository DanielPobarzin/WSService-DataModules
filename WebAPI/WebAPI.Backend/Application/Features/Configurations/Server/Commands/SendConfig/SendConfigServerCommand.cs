using Application.Wrappers;
using MediatR;

namespace Application.Features.Configurations.Server.Commands.SendConfig
{
	public class SendConfigServerCommand : IRequest<Response<Guid>>
	{
		public Guid Id { get; set; }
	}
}

