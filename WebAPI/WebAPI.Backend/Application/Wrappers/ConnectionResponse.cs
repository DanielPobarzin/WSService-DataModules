using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Backend.Core.Application.Parameters;

namespace WebAPI.Backend.Core.Application.Wrappers
{
	public class ConnectionResponse<T> : Response<T>
	{
		public int ConnectionsOpen { get; set; }
		public int ConncetionTotal { get; set; }

		public ConnectionResponse(T data, ConnectionsCount recordsCount)
		{
			this.Data = data;
			this.Message = null;
			this.ConnectionsOpen = recordsCount.ActiveConnections;
			this.ConncetionTotal = recordsCount.TotalConnections;
			this.Succeeded = true;
			this.Errors = null;
		}
	}
}
