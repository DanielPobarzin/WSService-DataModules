using Domain.Settings.SignalRServer;

namespace Application.Interfaces.Repositories
{
	public interface IServerConfigRepositoryAsync : IGenericRepositoryAsync<ServerSettings>
	{
		Task <DBSettings> GetByIdDataBaseSettingsAsync(Guid id);
		Task <HostSettings> GetByIdHostSettingsAsync(Guid id);
		Task <HubSettings> GetByIdHubSettingsAsync(Guid id);
	}
}