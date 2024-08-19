using Application.Wrappers;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.Clients.Commands.UpdateClient
{
	/// <summary>
	/// UpdateClientCommand - handles IRequest
	/// BaseRequestParameter - contains parameters for updating an existing client entity
	/// BaseResponseParameter - contains the entity of the client and the result of the execution (successful/unsuccessful)
	/// To update an existing client, use the properties from the body of this class (do not change the id).
	/// </summary>
	/// <remarks>
	/// Client Id - unique identifier for the client.
	/// WorkStatus - work status <see cref="Domain.Enums.WorkStatus"/> of the client.
	/// Connection status - connection status <see cref="Domain.Enums.ConnectionStatus"/> of the client.
	/// Connection id - optional.
	/// </remarks>
	public class UpdateClientCommand : IRequest<Response<Client>>
	{
		public Guid ClientId { get; set; }
		public WorkStatus WorkStatus { get; set; }
		public ConnectionStatus ConnectionStatus { get; set; }
		public string? ConnectionId { get; set; }
	}
}
