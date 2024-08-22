namespace Interactors.Interfaces
{
	public interface IProducerService
	{
		Task PutMessageProducerProcessAsync(string topic, string message, string Key);
	}
}
