using Application.Features.Connections.Queries.GetConnectionsList;
using Application.Parameters;
using Domain.Entities;

namespace Application.Interfaces.Repositories
{
	public interface IConnectionRepositoryAsync : IGenericRepositoryAsync<Connection>
	{
		Task<Connection> GetByConnectionIdAsync(string connectionId);
		Task <(IEnumerable<Entity> context, ConnectionsCount connectionCount)> GetStateConnectionsResponseAsync(GetConnectionsListQuery requestParameters);
	}
}
