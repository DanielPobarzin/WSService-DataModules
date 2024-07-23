using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Features.Connections.Queries.GetConnectionsList;
using Application.Parameters;
using Domain.Entities;

namespace Application.Interfaces.Repositories
{
	public interface IConnectionRepositoryAsync : IGenericRepositoryAsync<Connection>
	{
		Task <(IEnumerable<Entity> context, ConnectionsCount connectionCount)> GetStateConnectionsResponseAsync(GetConnectionsListQuery requestParameters);
	}
}
