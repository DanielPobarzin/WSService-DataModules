using Application.Wrappers;
using Domain.Common;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Connections.Commands.AddConnection
{
	public class AddConnectionCommand : IRequest<Response<string>>
	{
		public Guid ServerId { get; set; }
		public Guid ClientId { get; set; }
		public string ConnectionId { get; set; }
		public ConnectionStatus Status { get; private set; } = ConnectionStatus.Opened;
		public DateTime? TimeStampOpenConnection { get; set; }
	}
}
