using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.Servers.Commands.UpdateServer
{
	public class UpdateServerCommandHandler : IRequestHandler<UpdateServerCommand, Response<Server>>
	{
		private readonly IServerRepositoryAsync _repository;
		private readonly IMapper _mapper;
		public UpdateServerCommandHandler(IServerRepositoryAsync repository, IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}
		public async Task<Response<Server>> Handle(UpdateServerCommand command, CancellationToken cancellationToken)
		{
			var server = await _repository.GetByIdAsync(command.ServerId);
			if (server == null) throw new APIException($"Server Not Found.");
			server = _mapper.Map<Server>(command);

			server.CountListeners = command.CountListeners;
			server.ConnectionId = command.ConnectionId;
			server.ConnectionStatus = command.ConnectionStatus;
			server.WorkStatus = command.WorkStatus;

			await _repository.UpdateAsync(server);
			return new Response<Server>(server, true);
		}
	}
}
