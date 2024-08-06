using Application.Wrappers;
using Domain.Enums;
using MediatR;

namespace Application.Features.Connections.Commands.CloseConnection
{
	public class CloseConnectionCommand :  IRequest<Response<ConnectionCommand>>
	{
		public Guid Id { get; set; }
		public ConnectionCommand Command { get; private set; } = ConnectionCommand.CLose;
		
	}
}
