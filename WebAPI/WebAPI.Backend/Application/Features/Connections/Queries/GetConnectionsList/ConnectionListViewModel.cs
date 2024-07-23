using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Connections.Queries.GetConnectionsList
{
	public class ConnectionListViewModel 
	{
		public IList<ConnectionLookupDTO> Connections { get; set; }
	}
}
