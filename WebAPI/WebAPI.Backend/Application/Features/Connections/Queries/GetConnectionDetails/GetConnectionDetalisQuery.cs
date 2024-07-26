using Application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Connections.Queries.GetConnectionDetails
{
	public class GetConnectionDetalisQuery : IRequest<Response<ConnectionDetailsViewModel>>
	{
		public string ConnectionId { get; set; }
	}
}
