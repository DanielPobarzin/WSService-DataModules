using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Backend.Core.Application.Parameters
{
	public class ConnectionsCount
	{
		public int ActiveConnections { get; set; }
		public int TotalConnections { get; set; }
	}
}
