using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using Persistance.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.Repositories
{
	public class ServerRepository : GenericRepositoryAsync<Server, ConnectionDbContext>, IServerRepositoryAsync
	{
		private ConnectionDbContext _dbContext;
		private IMapper _mapper;

		public ServerRepository(ConnectionDbContext dbContext, IMapper mapper) : base(dbContext)
		{
			_dbContext = dbContext;
			_mapper = mapper;
		}
	}
}
