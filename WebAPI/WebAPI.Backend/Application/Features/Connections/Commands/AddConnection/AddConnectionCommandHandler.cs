using Application.Exceptions;
using Application.Features.Connections.Commands.DeleteConnection;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using Domain.Common;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Connections.Commands.AddConnection
{
	public class AddConnectionCommandHandler : IRequestHandler<AddConnectionCommand, Response<string>>
	{
		private readonly IConnectionRepositoryAsync _repository;

		public AddConnectionCommandHandler(IConnectionRepositoryAsync repository)
		{
			_repository = repository;
		}
		public async Task<Response<string>> Handle(AddConnectionCommand command, CancellationToken cancellationToken)
		{
			var connection = new Connection
			{
				ClientId = command.ClientId,
				ServerId = command.ServerId,
				ConnectionId = command.ConnectionId,
				TimeStampOpenConnection = command.TimeStampOpenConnection,
				TimeStampCloseConnection = null,
				Session = null,
				Status = ConnectionStatus.Open
			};
			await _repository.AddAsync(connection);
			return new Response<string>(connection.ConnectionId, true);
		}
	}
}
