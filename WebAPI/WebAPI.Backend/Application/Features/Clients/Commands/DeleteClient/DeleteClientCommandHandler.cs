using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Clients.Commands.DeleteClient
{
	public class DeleteClientCommandHandler : IRequestHandler<DeleteClientCommand, Response<Guid>>
	{
		private readonly IClientRepositoryAsync _repository;
		public DeleteClientCommandHandler (IClientRepositoryAsync repository)
		{
			_repository = repository;
		}
		public async Task<Response<Guid>> Handle(DeleteClientCommand command, CancellationToken cancellationToken)
		{
			var client = await _repository.GetByIdAsync(command.ClientId);
			if (client == null) throw new APIException($"Client Not Found.");

			await _repository.DeleteAsync(client);
			return new Response<Guid>(client.Id, true);
		}
	}
}