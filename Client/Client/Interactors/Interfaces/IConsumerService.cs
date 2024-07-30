using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactors.Interfaces
{
	public interface IConsumerService<T> : IHostedService
	{
		void PullMessageConsumerProcess (CancellationToken stoppingToken);
	}
}
