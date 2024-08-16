using Application.Wrappers;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.Clients.Commands.AddClient
{
	/// <summary>
	/// Command for adding a new client.
	/// </summary>
	public class AddClientCommand : IRequest<Response<Client>>
	{
		/// <summary>
		/// Gets or sets the unique identifier for the client.
		/// </summary>
		public Guid ClientId { get; set; }

		/// <summary>
		/// Gets or sets the work status <see cref="Domain.Enums.WorkStatus"/> of the client.
		/// </summary>
		public WorkStatus WorkStatus { get; set; }

		/// <summary>
		/// Gets or sets the connection status <see cref="Domain.Enums.ConnectionStatus"/> of the client.
		/// </summary>
		public ConnectionStatus ConnectionStatus { get; set; }

		/// <summary>
		/// Gets or sets the connection identifier for the client.
		/// This property is optional and can be null.
		/// </summary>
		public string? ConnectionId { get; set; }
	}
}
