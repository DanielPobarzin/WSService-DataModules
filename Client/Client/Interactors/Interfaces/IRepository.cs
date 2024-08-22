namespace Interactors.Interfaces
{
    public interface IRepository<T> where T : class
	{
		Task<T> PublishMessage(T entity);
		Task<T> FetchByIdMessage(Guid id);
	}
}
