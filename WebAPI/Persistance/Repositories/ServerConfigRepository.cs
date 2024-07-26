using Application.Features.Connections.Queries.GetConnectionsList;
using Application.Interfaces.Repositories;
using Application.Parameters;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistance.DbContexts;
using Persistance.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Settings.SignalRServer;

namespace Persistance.Repositories
{
	public class ServerConfigRepository : GenericRepositoryAsync<ServerSettings, ServerConfigDbContext>, IServerConfigRepositoryAsync
	{
		private ServerConfigDbContext _dbContext;
		private IMapper _mapper;
		public ServerConfigRepository(ServerConfigDbContext dbContext, IMapper mapper) : base(dbContext)
		{
			_dbContext = dbContext;
			_mapper = mapper;
		}
	}
}
