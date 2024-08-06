using Application.Wrappers;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.Clients.Commands.UpdateClient
{
	public class UpdateClientCommand : IRequest<Response<Client>>
	{
		public Guid ClientId { get; set; }
		public WorkStatus WorkStatus { get; set; }
		public ConnectionStatus ConnectionStatus { get; set; }
		public string? ConnectionId { get; set; }
	}
}
