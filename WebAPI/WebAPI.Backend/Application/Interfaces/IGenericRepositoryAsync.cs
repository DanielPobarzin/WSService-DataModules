namespace Application.Interfaces
{
	public interface IGenericRepositoryAsync<T> where T : class
	{
		Task<T> GetByIdAsync(Guid id);
		Task<(IEnumerable<T>, int)> GetAllAsync();
		Task<T> AddAsync(T entity);
		Task UpdateAsync(T entity);
		Task DeleteAsync(T entity);
		Task GeneralInsertAsync(IEnumerable<T> entities);
	}
}

