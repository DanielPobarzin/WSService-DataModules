using Application.Parameters;
using Application.Wrappers;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.Connections.Queries.GetConnectionsList
{
	public class GetConnectionsListQuery : QueryParameter, IRequest<ConnectionResponse<IEnumerable<Entity>>>
	{
		public ConnectionStatus Status { get; set; }
	}
}
