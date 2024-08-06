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
		private readonly IConnectionRepositoryAsync _repository;
		private readonly IProducerService _producerService;
		private readonly IConfiguration _configuration;
		private readonly CancellationToken _cancellingToken;
		private readonly string _topicProduce;
		public CloseConnectionCommandHandler(IConfiguration configuration, IConnectionRepositoryAsync Repository, IProducerService producerService, CancellationToken cancellationToken)
		{
				_repository = Repository;
				_producerService = producerService;
				_configuration = configuration;
				_cancellingToken = cancellationToken;
				_topicProduce = _configuration["Kafka:Topic"];
		}
		public async Task<Response<ConnectionCommand>> Handle(CloseConnectionCommand command, CancellationToken cancellationToken)
		{
			var client = await _repository.GetByIdAsync(command.Id);
			if (client.Client.WorkStatus != WorkStatus.Active) throw new APIException($"Client not active.");

			var message = JsonSerializer.Serialize(command);

			await _producerService.ProduceMessageProcessAsync(_topicProduce, message);
			return new Response<ConnectionCommand>(command.Command, true);

		}
	}
}
