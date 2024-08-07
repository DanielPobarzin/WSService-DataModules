using Application.Exceptions;
using Application.Features.Servers.Commands.DeleteServer;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Configurations.Server.Commands.DeleteConfig
{
	public class DeleteConfigServerCommandHandler : IRequestHandler<DeleteConfigServerCommand, Response<Guid>>
	{
		private readonly IServerConfigRepositoryAsync _repository;
		public DeleteConfigServerCommandHandler(IServerConfigRepositoryAsync repository)
		{
			_repository = repository;
		}
		public async Task<Response<Guid>> Handle(DeleteConfigServerCommand command, CancellationToken cancellationToken)
		{
			var config = await _repository.GetByIdAsync(command.Id);
			if (config == null) throw new APIException($"Config Not Found.");

			await _repository.DeleteAsync(config);
			return new Response<Guid>(config.SystemId, true);
		}
	}
}
