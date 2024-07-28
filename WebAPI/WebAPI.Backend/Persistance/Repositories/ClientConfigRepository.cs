using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Settings.SignalRClient;
using Persistance.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.Repositories
{
	public class ClientConfigRepository : GenericRepositoryAsync<ClientSettings, ClientConfigDbContext>, IClientConfigRepositoryAsync
	{
		private ClientConfigDbContext _dbContext;
		private IMapper _mapper;
		public ClientConfigRepository(ClientConfigDbContext dbContext, IMapper mapper) : base(dbContext)
		{
			_dbContext = dbContext;
			_mapper = mapper;
		}
	}
}
