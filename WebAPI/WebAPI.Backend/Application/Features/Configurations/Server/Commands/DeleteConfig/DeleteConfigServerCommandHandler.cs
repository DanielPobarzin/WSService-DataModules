using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Configurations.Server.Commands.DeleteConfig
{
	public class DeleteConfigServerCommandHandler : IRequestHandler<DeleteConfigServerCommand, Response<Guid>>
	{
		private readonly IServerConfigRepositoryAsync _repository;

		/// <summary>
		/// Initializes a new instance of the <see cref="DeleteConfigServerCommandHandler"/> class.
		/// </summary>
		/// <param name="repository">The repository used for accessing server configurations.</param>
		public DeleteConfigServerCommandHandler(IServerConfigRepositoryAsync repository)
		{
			_repository = repository;
		}

		/// <summary>
		/// Handles the deletion of a server configuration.
		/// </summary>
		/// <param name="command">The command containing the ID of the server configuration to delete.</param>
		/// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{Guid}"/> with the ID of the deleted server configuration.</returns>
		/// <exception cref="APIException">Thrown when the specified configuration is not found.</exception>
		public async Task<Response<Guid>> Handle(DeleteConfigServerCommand command, CancellationToken cancellationToken)
		{
			var config = await _repository.GetByIdAsync(command.Id) ?? throw new APIException($"Config Not Found.");
			await _repository.DeleteAsync(config);
			return new Response<Guid>(config.HubSettings.ServerId, true);
		}
	}
}
