using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Parameters;

namespace Application.Wrappers
{
	public class ConnectionResponse<T> : Response<T>
	{
		public int ConnectionsFiltered { get; set; }
		public int ConnectionTotal { get; set; }

		public ConnectionResponse(T data, ConnectionsCount recordsCount)
		{
			this.Data = data;
			this.Message = null;
			this.ConnectionsFiltered = recordsCount.FilteredConnections;
			this.ConnectionTotal = recordsCount.TotalConnections;
			this.Succeeded = true;
			this.Errors = null;
		}
	}
}
