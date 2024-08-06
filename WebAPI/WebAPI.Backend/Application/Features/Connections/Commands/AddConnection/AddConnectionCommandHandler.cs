using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.Connections.Commands.AddConnection
{
	public class AddConnectionCommandHandler : IRequestHandler<AddConnectionCommand, Response<string>>
	{
		private readonly IConnectionRepositoryAsync _repositoryConnection;
		private readonly IServerRepositoryAsync _repositoryServer;
		private readonly IClientRepositoryAsync _repositoryClient;
		private readonly IMapper _mapper;

		public AddConnectionCommandHandler(IConnectionRepositoryAsync repositoryConnection, IServerRepositoryAsync repositoryServer, IClientRepositoryAsync repositoryClient, IMapper mapper)
		{
			_repositoryConnection = repositoryConnection;
			_repositoryClient = repositoryClient;
			_repositoryServer = repositoryServer;
			_mapper = mapper;
		}
		public async Task<Response<string>> Handle(AddConnectionCommand command, CancellationToken cancellationToken)
		{
			var client = await _repositoryClient.GetByIdAsync(command.ClientId);
			if (client == null) throw new APIException($"Client Not Found.");
			var server = await _repositoryServer.GetByIdAsync(command.ServerId);
			if (server == null) throw new APIException($"Server Not Found.");
			var currentConnection = await _repositoryConnection.GetByConnectionIdAsync(command.ConnectionId);
			if (currentConnection != null) throw new APIException($"Connection has already been added.");
			var connection = _mapper.Map<Connection>(command);
			
			await _repositoryConnection.AddAsync(connection);
			return new Response<string>(connection.ConnectionId, true);
		}
	}
}
