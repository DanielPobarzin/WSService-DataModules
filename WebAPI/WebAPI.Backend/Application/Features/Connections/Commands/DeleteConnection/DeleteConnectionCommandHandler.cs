using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Connections.Commands.DeleteConnection
{
	public class DeleteConnectionCommandHandler : IRequestHandler<DeleteConnectionCommand, Response<string>>
	{
		private readonly IConnectionRepositoryAsync _repository;

		public DeleteConnectionCommandHandler(IConnectionRepositoryAsync repository)
		{
			_repository = repository;
		}
		public async Task<Response<string>> Handle(DeleteConnectionCommand command, CancellationToken cancellationToken)
		{
			var connection = await _repository.GetByConnectionIdAsync(command.ConnectionId);
			if (connection == null) throw new APIException($"Connection Not Found.");
			if (connection.Status == ConnectionStatus.Open) throw new APIException($"Connection is open. Сannot delete open connection.");
			await _repository.DeleteAsync(connection);
			return new Response<string>(connection.ConnectionId);
		}
	}
}
