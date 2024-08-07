using Application.Wrappers;
using MediatR;

namespace Application.Features.Configurations.Client.Queries.GetConfigClientList
{
	public class GetConfigClientListQuery : IRequest<Response<IEnumerable<ConfigClientLookupDTO>>>
	{
	}
}
