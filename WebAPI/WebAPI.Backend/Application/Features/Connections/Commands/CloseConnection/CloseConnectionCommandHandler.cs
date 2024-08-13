using Application.Exceptions;
using Application.Features.Connections.Commands.OpenConnection;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Application.Features.Connections.Commands.CloseConnection
{
	public class CloseConnectionCommandHandler : IRequestHandler<CloseConnectionCommand, Response<ConnectionCommand>>
	{
		private readonly IClientRepositoryAsync _repositoryClient;
		private readonly IProducerService _producerService;
		private readonly IConfiguration _configuration;
		private readonly string _topicProduce;
		public CloseConnectionCommandHandler(IConfiguration configuration, IClientRepositoryAsync repositoryClient, IProducerService producerService)
		{
				_repositoryClient = repositoryClient;
				_producerService = producerService;
				_configuration = configuration;
				_topicProduce = _configuration["Kafka:Topic"];
		}
		public async Task<Response<ConnectionCommand>> Handle(CloseConnectionCommand command, CancellationToken cancellationToken)
		{
			var client = await _repositoryClient.GetByIdAsync(command.Id);
			if (client.WorkStatus != WorkStatus.Active) throw new APIException($"Client not active.");
			if (client.ConnectionStatus == ConnectionStatus.Closed) throw new APIException($"Client is already disconnected.");

			var message = JsonSerializer.Serialize(command);

			await _producerService.ProduceMessageProcessAsync(_topicProduce, message);
			return new Response<ConnectionCommand>(command.Command, true);

		}
	}
}
