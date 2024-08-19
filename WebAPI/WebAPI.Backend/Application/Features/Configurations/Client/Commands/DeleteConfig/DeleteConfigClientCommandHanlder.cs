using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Configurations.Client.Commands.DeleteConfig
{
	public class DeleteConfigClientCommandHanlder : IRequestHandler<DeleteConfigClientCommand, Response<Guid>>
	{
		private readonly IClientConfigRepositoryAsync _repository;

		/// <summary>
		/// Constructor for DeleteConfigClientCommandHanlder class. 
		/// Initializes a new instance of the <see cref="DeleteConfigClientCommandHanlder"/> class.
		/// </summary>
		/// <param name="repository">The repository interface for working with client configurations.</param>
		public DeleteConfigClientCommandHanlder(IClientConfigRepositoryAsync repository)
		{
			_repository = repository;
		}

		/// <summary>
		/// Handles the command to delete a client config.
		/// </summary>
		/// <param name="command">The command containing client config id to be deleted.</param>
		/// <param name="cancellationToken">Cancellation token for the asynchronous operation.</param>
		/// <returns>
		/// An instance of <see cref="Response{Guid}"/> containing the id of the deleted client config and the operation status.
		/// </returns>
		/// <exception cref="APIException">Thrown if a client config with the specified ID  does not exist.</exception>
		public async Task<Response<Guid>> Handle(DeleteConfigClientCommand command, CancellationToken cancellationToken)
		{
			var config = await _repository.GetByIdAsync(command.Id);
			if (config == null) throw new APIException($"Config Not Found.");

			await _repository.DeleteAsync(config);
			return new Response<Guid>(config.SystemId, true);
		}
	}
}
