using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Features.Connections.Commands.UpdateConnection
{
	public class UpdateConnectionCommandHandler : IRequestHandler<UpdateConnectionCommand, Response<string>>
	{
		private readonly IConnectionRepositoryAsync _connectionRepository;

		public UpdateConnectionCommandHandler(IConnectionRepositoryAsync Repository)
		{
			_connectionRepository = Repository;
		}
		public async Task<Response<string>> Handle(UpdateConnectionCommand command, CancellationToken cancellationToken)
		{
			var connection = await _connectionRepository.GetByConnectionIdAsync(command.ConnectionId);

				if (connection == null)
				{
					throw new APIException($"Connection Not Found.");
				}
				else
				{
					connection.TimeStampCloseConnection = command.TimeStampCloseConnection;
					connection.Session = command.Session;
					connection.Status = command.Status;
					await _connectionRepository.UpdateAsync(connection);

					return new Response<string>(connection.ConnectionId, true);
				}
		}
	}
}
