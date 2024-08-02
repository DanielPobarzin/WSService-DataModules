using Application.Features.Connections.Commands.AddConnection;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Features.Connections.Commands.OpenConnection
{
	public class OpenConnectionCommandHandler : IRequestHandler<OpenConnectionCommand, Response<string>>
	{
		private readonly IConnectionRepositoryAsync _repository;
		private readonly IProducerService _producerService;
		private readonly IConsumerService _consumerService;
		private readonly IConfiguration _configuration;
		private	readonly IDateTimeService _dateTime;
		private readonly CancellationToken _cancellingToken;
		private readonly string _topic;
		public OpenConnectionCommandHandler(IConfiguration configuration, IDateTimeService dateTime, IConnectionRepositoryAsync Repository, IProducerService producerService, IConsumerService consumerService, CancellationToken cancellationToken)
		{
			_repository = Repository;
			_producerService = producerService;
			_consumerService = consumerService;
			_configuration = configuration;
			_dateTime = dateTime;
			_cancellingToken = cancellationToken;
			_topic = _configuration["Kafka:Topic"];
		}
		public async Task<Response<string>> Handle(OpenConnectionCommand command, CancellationToken cancellationToken)
		{
			var connection = await _repository.GetByConnectionIdAsync(command.ConnectionId);
			if ( connection == null ) {
				connection = new Connection
				{
					ClientId = command.ClientId,
					ServerId = command.ServerId,
					ConnectionId = command.ConnectionId,
					TimeStampOpenConnection = _dateTime.Now,
					Status = ConnectionStatus.Open
				};
				var message = JsonSerializer.Serialize(connection);
				await _producerService.ProduceMessageProcessAsync(_topic, message);
			    _consumerService.KafkaPullMessageProcess(_cancellingToken);
				await _repository.AddAsync(connection);
				return new Response<string>(connection.ConnectionId, true);
			}
			connection






		}
	}
}
