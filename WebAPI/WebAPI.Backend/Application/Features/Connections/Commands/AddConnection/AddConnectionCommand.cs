using Application.Wrappers;
using Domain.Enums;
using MediatR;

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
