using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactors.Interfaces
{
	public interface IProducerService
	{
		Task PutMessageProducerProcessAsync(string topic, string message, string Key);
	}
}
