using Domain.Entities;

namespace Application.Interfaces.Repositories
{
	public interface IServerRepositoryAsync : IGenericRepositoryAsync<Server>
	{
	}
	public interface IClientRepositoryAsync : IGenericRepositoryAsync<Client>
	{
	}
}
