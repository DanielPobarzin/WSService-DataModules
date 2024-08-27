using Microsoft.Extensions.Hosting;

namespace Application.Interfaces
{
	public interface IConsumerService : IHostedService
	{
		Task KafkaPullMessageProcess(CancellationToken cancellationToken);
	}
}
