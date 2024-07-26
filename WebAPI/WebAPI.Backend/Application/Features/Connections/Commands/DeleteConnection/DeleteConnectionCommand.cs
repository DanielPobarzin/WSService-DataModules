using Application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Connections.Commands.DeleteConnection
{
	public class DeleteConnectionCommand : IRequest<Response<string>>
	{
		public string ConnectionId { get; set; }
	}
}
