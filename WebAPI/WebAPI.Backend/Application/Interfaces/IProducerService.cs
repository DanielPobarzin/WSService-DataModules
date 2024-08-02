using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
	public interface IProducerService
	{
		Task ProduceMessageProcessAsync(string topic, string message);
	}
}
