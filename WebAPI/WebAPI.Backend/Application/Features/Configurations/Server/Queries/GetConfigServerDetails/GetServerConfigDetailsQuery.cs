using Application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Configurations.Server.Queries.GetConfigServerDetails
{
	public class GetServerConfigDetailsQuery : IRequest<Response<ServerConfigDetailsViewModel>>
	{
		public Guid Id { get; set; }
	}
}
