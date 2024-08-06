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
	public class ClientRepository : GenericRepositoryAsync<Client, ConnectionDbContext>
	{
		private ConnectionDbContext _dbContext;
		private IMapper _mapper;

		public ClientRepository(ConnectionDbContext dbContext, IMapper mapper) : base(dbContext)
		{
			_dbContext = dbContext;
			_mapper = mapper;
		}
	}
}
