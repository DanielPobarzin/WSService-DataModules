using Application.Wrappers;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.Servers.Commands.AddServer
{
	public class AddServerCommand : IRequest<Response<Server>>
	{
		public Guid ServerId { get; set; }
		public WorkStatus WorkStatus { get; set; }
		public ConnectionStatus ConnectionStatus { get; set; }
		public string? ConnectionId { get; set; }
		public int CountListeners { get; set; }
	}
}
