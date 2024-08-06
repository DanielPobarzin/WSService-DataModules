using Application.Features.Servers.Queries.GetServer.GetAll;
using Application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Configurations.Server.Queries.GetConfigServerList
{
	public class GetConfigServerListQuery : IRequest<Response<IEnumerable<ConfigServerLookupDTO>>>
	{

	}
}
