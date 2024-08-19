using Application.Wrappers;
using MediatR;

namespace Application.Features.Configurations.Client.Commands.SendConfig
{
	/// <summary>
	/// SendConfigClientCommand - handles IRequest
	/// BaseRequestParameter - contains the client config Id
	/// BaseResponseParameter - contains the client config Id and the result of the execution (successful/unsuccessful)
	/// To get and send an existing client Config, use its Id from the body of this class
	/// </summary>
	/// <remarks>
	/// Client Id - unique identifier for the client and client Config. 
	/// </remarks>
	public class SendConfigClientCommand : IRequest<Response<Guid>>
	{
		public Guid Id { get; set; }
	}
}
