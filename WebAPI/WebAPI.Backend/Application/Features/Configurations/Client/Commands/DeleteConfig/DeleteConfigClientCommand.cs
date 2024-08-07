using Application.Wrappers;
using MediatR;

namespace Application.Features.Configurations.Client.Commands.DeleteConfig
{
	public class DeleteConfigClientCommand : IRequest<Response<Guid>>
	{
		public Guid Id { get; set; }
	}
}
