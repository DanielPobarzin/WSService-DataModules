using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Clients.Commands.DeleteClient
{
	public class DeleteClientCommandHandler : IRequestHandler<DeleteClientCommand, Response<Guid>>
	{
		private readonly IClientRepositoryAsync _repository;

		/// <summary>
		/// Constructor for DeleteClientCommandHandler class. 
		/// Initializes a new instance of the <see cref="DeleteClientCommandHandler"/> class.
		/// </summary>
		/// <param name="repository">The repository interface for working with clients.</param>
		public DeleteClientCommandHandler (IClientRepositoryAsync repository)
		{
			_repository = repository;
		}

		/// <summary>
		/// Handles the command to delete a client.
		/// </summary>
		/// <param name="command">The command containing client id to be deleted.</param>
		/// <param name="cancellationToken">Cancellation token for the asynchronous operation.</param>
		/// <returns>
		/// An instance of <see cref="Response{Guid}"/> containing the id of the deleted client and the operation status.
		/// </returns>
		/// <exception cref="APIException">Thrown if a client with the specified ID  does not exist.</exception>
		public async Task<Response<Guid>> Handle(DeleteClientCommand command, CancellationToken cancellationToken)
		{
			var client = await _repository.GetByIdAsync(command.ClientId);
			if (client == null) throw new APIException($"Client Not Found.");

			await _repository.DeleteAsync(client);
			return new Response<Guid>(client.Id, true);
		}
	}
}