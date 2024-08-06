using Application.Wrappers;
using MediatR;

namespace Application.Features.Connections.Commands.DeleteConnection
{
	public class DeleteConnectionCommand : IRequest<Response<string>>
	{
		public string ConnectionId { get; set; }
	}
}
