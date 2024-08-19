using Application.Wrappers;
using MediatR;

namespace Application.Features.Clients.Queries.GetClient.GetAll
{
	public class GetClientListQuery : IRequest<Response<IEnumerable<ClientLookupDTO>>>
	{

	}
}
