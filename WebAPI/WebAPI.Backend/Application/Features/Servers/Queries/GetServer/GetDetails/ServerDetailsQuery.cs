using Application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Servers.Queries.GetServer.GetDetails
{
	public class ServerDetailsQuery : IRequest<Response<ServerDetailsViewModel>>
	{
		public Guid Id { get; set; }
	}
}
