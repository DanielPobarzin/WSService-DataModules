using Application.Exceptions;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Settings.SignalRServer;
using Microsoft.EntityFrameworkCore;
using Persistance.DbContexts;

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
		public async Task<DBSettings> GetByIdDataBaseSettingsAsync(Guid Id)
		{
			var dbSettings = await _dbContext.DbSettings.FirstOrDefaultAsync(e => e.SystemId == Id);
			if (dbSettings == null) { throw new APIException("Configure with database parameters Not Found."); }
			return dbSettings;
		}
		public async Task<HostSettings> GetByIdHostSettingsAsync(Guid Id)
		{
			var hostSettings = await _dbContext.HostSettings.FirstOrDefaultAsync(e => e.SystemId == Id);
			if (hostSettings == null) { throw new APIException("Configure with host parameters Not Found."); }
			return hostSettings;
		}
		public async Task<HubSettings> GetByIdHubSettingsAsync(Guid Id)
		{
			var hubSettings = await _dbContext.HubSettings.FirstOrDefaultAsync(e => e.SystemId == Id);
			if (hubSettings == null) { throw new APIException("Configure with hubs parameters Not Found."); }
			return hubSettings;
		}
	}
}
