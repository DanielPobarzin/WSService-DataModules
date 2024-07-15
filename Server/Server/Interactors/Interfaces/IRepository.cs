using Entities.Entities;
using Entities.Models;

namespace Interactors.Interfaces
{
	public interface IRepository<T> where T : BaseEntity
	{
		IEnumerable<T> GetAllList();
	}
	public interface IRepositoryPublish<T> where T : class
	{
		Task AddConnection(T entity);
		Task RemoveConnection(string id);
	}
}
