using Application.Wrappers;
using Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Connections.Commands.OpenConnection
{
	public class OpenConnectionCommand : IRequest<Response<string>>
	{
		public Guid ServerId { get; set; }
		public Guid ClientId { get; set; }
		public string? ConnectionId { get; set; }
		public ConnectionStatus Status { get; set; }
	}
}
