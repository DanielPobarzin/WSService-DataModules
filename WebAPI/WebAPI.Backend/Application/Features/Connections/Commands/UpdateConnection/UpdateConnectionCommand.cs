using Application.Wrappers;
using Domain.Enums;
using MediatR;

namespace Application.Features.Connections.Commands.UpdateConnection
{
	public class UpdateConnectionCommand : IRequest<Response<string>>
	{
		public string ConnectionId { get; set; }
		public TimeSpan? Session { get; set; }
		public DateTime? TimeStampCloseConnection { get; set; }
		public ConnectionStatus Status { get; private set; } = ConnectionStatus.Closed;
	}
}
