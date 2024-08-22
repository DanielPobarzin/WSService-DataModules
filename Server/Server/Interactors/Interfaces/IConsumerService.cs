using Microsoft.Extensions.Hosting;

namespace Interactors.Interfaces
{
	public interface IConsumerService : IHostedService
	{
		void PullMessageConsumerProcess();
	}
}
