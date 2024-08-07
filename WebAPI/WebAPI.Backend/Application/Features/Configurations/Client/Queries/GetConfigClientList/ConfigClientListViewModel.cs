using Application.Features.Configurations.Server.Queries.GetConfigServerList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Configurations.Client.Queries.GetConfigClientList
{
	public class ConfigClientListViewModel
	{
		public IList<ConfigClientLookupDTO> ConfigClients { get; set; }
	}
}
