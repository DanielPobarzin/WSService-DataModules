using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Backend.Core.Application.Parameters;
using WebAPI.Backend.Core.Application.Wrappers;
using WebAPI.Backend.Core.Domain.Entities;
using WebAPI.Domain.Common;

namespace WebAPI.Backend.Core.Application.Features.Connections.Queries.GetConnectionsList
	{
	public class GetConnectionsListQuery : QueryParameter, IRequest<ConnectionResponse<IEnumerable<Entity>>>
	{
		public ConnectionsStatus Status { get; set; }
	}
}
