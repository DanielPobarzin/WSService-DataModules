using Entities.Entities;

namespace Interactors.Interfaces
{
	public interface IRepository<T> where T : BaseEntity
	{
		IEnumerable<T> GetAllList();
	}
}
