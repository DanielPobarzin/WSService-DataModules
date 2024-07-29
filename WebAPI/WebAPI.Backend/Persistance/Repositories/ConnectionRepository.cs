using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Persistance.DbContexts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Features.Connections.Queries.GetConnectionsList;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Parameters;
using Domain.Entities;
using Domain.Common;
using Application.Exceptions;

namespace Persistance.Repositories
{
	public class ConnectionRepository : GenericRepositoryAsync<Connection,ConnectionDbContext>, IConnectionRepositoryAsync
	{
		private ConnectionDbContext _dbContext;
		private IMapper _mapper;

		public ConnectionRepository(ConnectionDbContext dbContext, IMapper mapper) : base(dbContext)
		{
			_dbContext = dbContext;
			_mapper = mapper;
		}
		public async Task<Connection?> GetByConnectionIdAsync(string? connectionId)
		{
			if (connectionId == null) return null;
			var connection = await _dbContext.Connections.FirstOrDefaultAsync(e => e.ConnectionId == connectionId);
			if (connection == null) { throw new APIException("Connection Not Found."); }
			return connection;
		}
		public async Task<(IEnumerable<Entity> context, ConnectionsCount connectionCount)> GetStateConnectionsResponseAsync(GetConnectionsListQuery requestParameters)
		{
			var status = requestParameters.Status;
			var fields = requestParameters.Fields;
			int connectionsTotal, connectionsFiltered;

			IQueryable<Connection> query = _dbContext.Connections.AsQueryable();

			if (status == ConnectionStatus.Close || status == ConnectionStatus.Open)
			{
				query = query.Where(e => e.Status == status);
			}

			connectionsFiltered = query.Count();
			connectionsTotal = await _dbContext.Connections.CountAsync();

			var connectionsCount = new ConnectionsCount
			{
				FilteredConnections = connectionsFiltered,
				TotalConnections = connectionsTotal
			};

			if (!string.IsNullOrWhiteSpace(fields))
			{
				var fieldsArray = fields.Split(',', StringSplitOptions.RemoveEmptyEntries)
										 .Select(f => f.Trim())
										 .ToArray();
				 
				var projectedQuery = query.ProjectTo<ConnectionLookupDTO>(_mapper.ConfigurationProvider, new { Fields = fieldsArray });
				var context = await projectedQuery.ToListAsync();

				return (context.Select(lookup => _mapper.Map<Entity>(lookup)), connectionsCount);
			}

			var defaultData = await query.ToListAsync();
			return (defaultData.Select(lookup => _mapper.Map<Entity>(lookup)), connectionsCount);

		}
	}
}