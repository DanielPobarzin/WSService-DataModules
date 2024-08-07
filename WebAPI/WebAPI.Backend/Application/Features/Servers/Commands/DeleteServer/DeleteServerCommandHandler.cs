﻿using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Servers.Commands.DeleteServer
{
	public class DeleteServerCommandHandler : IRequestHandler<DeleteServerCommand, Response<Guid>>
	{
		private readonly IServerRepositoryAsync _repository;
		public DeleteServerCommandHandler (IServerRepositoryAsync repository)
		{
			_repository = repository;
		}
		public async Task<Response<Guid>> Handle(DeleteServerCommand command, CancellationToken cancellationToken)
		{
			var server = await _repository.GetByIdAsync(command.ServerId);
			if (server == null) throw new APIException($"Server Not Found.");

			await _repository.DeleteAsync(server);
			return new Response<Guid>(server.Id, true);
		}
	}
}