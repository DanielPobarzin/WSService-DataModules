namespace Interactors.Interfaces
{
	public interface IRepository<T> where T : class
	{
		IEnumerable<T> GetAllList();
	}
}