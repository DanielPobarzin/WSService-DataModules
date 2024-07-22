using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Backend.Core.Application.Features.Connections.Queries.GetConnectionsList;
using WebAPI.Backend.Core.Application.Parameters;
using WebAPI.Backend.Core.Domain.Entities;
using WebAPI.Domain.Entities;

namespace WebAPI.Backend.Core.Application.Interfaces.Repositories
{
	public interface IConnectionRepositoryAsync : IGenericRepositoryAsync<Connection>
	{
		Task <(IEnumerable<Entity> context, ConnectionsCount connectionCount)> GetStateConnectionsResponseAsync(GetConnectionsListQuery requestParameters);
	}
}
