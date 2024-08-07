using Application.Wrappers;
using MediatR;

namespace Application.Features.Configurations.Client.Queries.GetConfigClientDetails
{
	public class GetClientConfigDetailsQuery : IRequest<Response<ClientConfigDetailsViewModel>>
	{
		public Guid Id { get; set; }
	}
}
