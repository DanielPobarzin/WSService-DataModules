using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Parameters;
using Application.Wrappers;
using Domain.Entities;
using Domain.Common;

namespace Application.Features.Connections.Queries.GetConnectionsList
	{
	public class GetConnectionsListQuery : QueryParameter, IRequest<ConnectionResponse<IEnumerable<Entity>>>
	{
		public ConnectionStatus Status { get; set; }
	}
}
