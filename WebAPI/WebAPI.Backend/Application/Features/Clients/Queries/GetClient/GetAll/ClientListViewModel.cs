using Application.Features.Connections.Queries.GetConnectionsList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Clients.Queries.GetClient.GetAll
{
	public class ClientListViewModel
	{
		public IList<ClientLookupDTO> Clients { get; set; }
	}
}
