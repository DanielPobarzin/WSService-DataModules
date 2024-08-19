using Application.Wrappers;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.Clients.Commands.AddClient
{
	/// <summary>
	/// AddClientCommand - handles IRequest
	/// BaseRequestParameter - contains the entity of the client
	/// BaseResponseParameter - contains the entity of the client and the result of the execution (successful/unsuccessful)
	/// To add an existing client, use the properties from the body of this class
	/// </summary>
	/// <remarks>
	/// Client Id - unique identifier for the client.
	/// WorkStatus - work status <see cref="Domain.Enums.WorkStatus"/> of the client.
	/// Connection status - connection status <see cref="Domain.Enums.ConnectionStatus"/> of the client.
	/// Connection id - optional.
	/// </remarks>
	public class AddClientCommand : IRequest<Response<Client>>
	{
		public Guid ClientId { get; set; }
		public WorkStatus WorkStatus { get; set; }
		public ConnectionStatus ConnectionStatus { get; set; }
		public string? ConnectionId { get; set; }
	}
}
