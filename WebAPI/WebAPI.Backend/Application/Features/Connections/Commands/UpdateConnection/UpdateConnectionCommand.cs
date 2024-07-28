using Application.Exceptions;
using Application.Wrappers;
using Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Features.Connections.Commands.UpdateConnection
{
	public class UpdateConnectionCommand : IRequest<Response<string>>
	{
		public string ConnectionId { get; set; }
		public TimeSpan? Session { get; set; }
		public DateTime? TimeStampCloseConnection { get; set; }
		public ConnectionStatus Status { get; set; }
	}
}
