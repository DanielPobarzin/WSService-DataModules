using Application.Exceptions;
using Application.Features.Connections.Commands.AddConnection;
using Application.Features.Connections.Queries.GetConnectionDetails;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using Confluent.Kafka;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
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
	public class OpenConnectionCommandHandler : IRequestHandler<OpenConnectionCommand, Response<ConnectionCommand>>
	{
		private readonly IConnectionRepositoryAsync _repository;
		private readonly IProducerService _producerService;
		private readonly IConfiguration _configuration;
		private readonly CancellationToken _cancellingToken;
		private readonly IMapper _mapper;
		private readonly string _topicProduce;
		public OpenConnectionCommandHandler(IConfiguration configuration, IConnectionRepositoryAsync Repository, IProducerService producerService, CancellationToken cancellationToken)
		{
			_repository = Repository;
			_producerService = producerService;
			_configuration = configuration;
			_cancellingToken = cancellationToken;
			_topicProduce = _configuration["Kafka:Topic"];
		}
		public async Task<Response<ConnectionCommand>> Handle(OpenConnectionCommand command, CancellationToken cancellationToken)
		{
			var client = await _repository.GetByIdAsync(command.Id);
			if (client.Client.WorkStatus == WorkStatus.NoNActive) throw new APIException($"Client not active.");
			if (client.Client.ConnectionStatus == ConnectionStatus.Opened) throw new APIException($"Client is already connected.");
			var message = JsonSerializer.Serialize(command);

			await _producerService.ProduceMessageProcessAsync(_topicProduce, message);
			return new Response<ConnectionCommand>(command.Command, true);
		}
	}
}

