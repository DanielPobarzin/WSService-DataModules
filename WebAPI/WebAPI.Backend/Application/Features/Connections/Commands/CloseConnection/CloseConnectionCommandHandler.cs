using Application.DTOs.Message;
using Application.Exceptions;
using Application.Features.Connections.Commands.OpenConnection;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using Confluent.Kafka;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
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
				_topicProduce = _configuration["Kafka:Topics:Send:CommandManagment"];
		}
		public async Task<Response<ConnectionCommand>> Handle(CloseConnectionCommand command, CancellationToken cancellationToken)
		{
			var client = await _repositoryClient.GetByIdAsync(command.Id);
			if (client.WorkStatus != WorkStatus.Active) throw new APIException($"Client not active.");
			if (client.ConnectionStatus == ConnectionStatus.Closed) throw new APIException($"Client is already disconnected.");

			var message = new MessageRequest
			{
				To = command.Id,
				Body = command.Command.ToString()
			};
			string json = JsonConvert.SerializeObject(message, Formatting.Indented);
			await _producerService.ProduceMessageProcessAsync(_topicProduce, json, "command");
			return new Response<ConnectionCommand>(command.Command, true);
		}
	}
}
