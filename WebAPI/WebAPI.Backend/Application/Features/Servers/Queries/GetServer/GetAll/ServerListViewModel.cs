using Application.Features.Connections.Queries.GetConnectionsList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Servers.Queries.GetServer.GetAll
{
	public class ServerListViewModel
	{
		public IList<ServerLookupDTO> Servers { get; set; }
	}
}
