using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Settings.SignalRClient;
using Persistance.DbContexts;

namespace Persistance.Repositories
{
	public class ClientConfigRepository : GenericRepositoryAsync<CLientSettings, ClientConfigDbContext>, IClientConfigRepositoryAsync
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
