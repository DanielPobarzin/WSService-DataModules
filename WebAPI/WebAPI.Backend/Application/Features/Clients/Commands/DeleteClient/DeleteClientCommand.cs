using Application.Wrappers;
using MediatR;

namespace Application.Features.Clients.Commands.DeleteClient
{
	/// <summary>
	/// DeleteClientCommand - handles IRequest
	/// BaseRequestParameter - contains the client Id
	/// BaseResponseParameter - contains the client Id and the result of the execution (successful/unsuccessful)
	/// To delete an existing client, use its Id from the body of this class
	/// </summary>
	/// <remarks>
	/// Client Id - unique identifier for the client. 
	/// </remarks>
	public class DeleteClientCommand : IRequest<Response<Guid>>
	{
		public Guid ClientId { get; set; }
	}
}
