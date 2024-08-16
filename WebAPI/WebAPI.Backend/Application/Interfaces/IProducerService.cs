namespace Application.Interfaces
{
	public interface IProducerService
	{
		Task ProduceMessageProcessAsync(string topic, string message, string key);
	}
}
