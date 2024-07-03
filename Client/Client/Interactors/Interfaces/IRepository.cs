using Entities.Entities;


namespace Interactors.Interfaces
{
    public interface IRepository<T> where T : class
	{
		Task PublishMessage(T entity);
	}
}
