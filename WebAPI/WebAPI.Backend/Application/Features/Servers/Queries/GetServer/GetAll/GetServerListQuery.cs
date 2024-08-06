using Application.Parameters;
using Application.Wrappers;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Servers.Queries.GetServer.GetAll
{
	public class GetServerListQuery : IRequest<Response<IEnumerable<ServerLookupDTO>>>
	{

	}
}
