using Application.Wrappers;
using MediatR;

namespace Application.Features.Configurations.Client.Commands.SendConfig
{
	public class SendConfigClientCommand : IRequest<Response<Guid>>
	{
		public Guid Id { get; set; }
	}
}
