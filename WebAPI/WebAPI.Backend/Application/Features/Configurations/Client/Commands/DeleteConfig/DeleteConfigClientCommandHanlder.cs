using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Configurations.Client.Commands.DeleteConfig
{
	public class DeleteConfigClientCommandHanlder : IRequestHandler<DeleteConfigClientCommand, Response<Guid>>
	{
		private readonly IClientConfigRepositoryAsync _repository;
		public DeleteConfigClientCommandHanlder(IClientConfigRepositoryAsync repository)
		{
			_repository = repository;
		}
		public async Task<Response<Guid>> Handle(DeleteConfigClientCommand command, CancellationToken cancellationToken)
		{
			var config = await _repository.GetByIdAsync(command.Id);
			if (config == null) throw new APIException($"Config Not Found.");

			await _repository.DeleteAsync(config);
			return new Response<Guid>(config.SystemId, true);
		}
	}
}
