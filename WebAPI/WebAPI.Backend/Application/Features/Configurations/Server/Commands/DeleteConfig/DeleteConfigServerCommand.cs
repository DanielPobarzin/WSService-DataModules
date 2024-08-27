using Application.Wrappers;
using MediatR;

namespace Application.Features.Configurations.Server.Commands.DeleteConfig
{
	public class DeleteConfigServerCommand : IRequest<Response<Guid>>
	{
		public Guid Id { get; set; }
	}
}
