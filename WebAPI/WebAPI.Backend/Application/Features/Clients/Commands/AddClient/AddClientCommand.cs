using Application.Wrappers;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Clients.Commands.AddClient
{
	public class AddClientCommand : IRequest<Response<Client>>
	{
		public Guid ClientId { get; set; }
		public WorkStatus WorkStatus { get; set; }
		public ConnectionStatus ConnectionStatus { get; set; }
		public string? ConnectionId { get; set; }
	}
}
