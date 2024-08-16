using Application.DTOs.Message;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Application.Features.Connections.Commands.OpenConnection
{
	public class OpenConnectionCommandHandler : IRequestHandler<OpenConnectionCommand, Response<ConnectionCommand>>
	{
		private readonly IClientRepositoryAsync _repository;
		private readonly IProducerService _producerService;
		private readonly IConfiguration _configuration;
		private readonly string _topicProduce;
		public OpenConnectionCommandHandler(IConfiguration configuration, IClientRepositoryAsync Repository, IProducerService producerService)
		{
			_repository = Repository;
			_producerService = producerService;
			_configuration = configuration;
			_topicProduce = _configuration["Kafka:Topics:Send:CommandManagment"];
		}
		public async Task<Response<ConnectionCommand>> Handle(OpenConnectionCommand command, CancellationToken cancellationToken)
		{
			var client = await _repository.GetByIdAsync(command.Id);
			if (client.WorkStatus == WorkStatus.NoNActive) throw new APIException($"Client not active.");
			if (client.ConnectionStatus == ConnectionStatus.Opened) throw new APIException($"Client is already connected.");

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

