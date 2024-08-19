using Application.Wrappers;
using MediatR;

namespace Application.Features.CLients.Queries.GetClient.GetDetails
{
	public class ClientDetailsQuery : IRequest<Response<ClientDetailsViewModel>>
	{
		public Guid Id { get; set; }
	}
}
