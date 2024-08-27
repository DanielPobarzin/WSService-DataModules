using Application.Wrappers;
using MediatR;

namespace Application.Features.Configurations.Server.Queries.GetConfigServerDetails
{
	public class GetServerConfigDetailsQuery : IRequest<Response<ServerConfigDetailsViewModel>>
	{
		public Guid Id { get; set; }
	}
}
