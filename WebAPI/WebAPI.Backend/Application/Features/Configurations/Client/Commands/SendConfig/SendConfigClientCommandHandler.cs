using Application.DTOs.Message;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using Domain.Settings.SignalRClient;
using MediatR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Application.Features.Configurations.Client.Commands.SendConfig
{
	public class SendConfigClientCommandHandler : IRequestHandler<SendConfigClientCommand, Response<Guid>>
	{
		private readonly IClientConfigRepositoryAsync _repository;
		private readonly IProducerService _producer;
		private readonly IConfiguration _configuration;
		private readonly string _topicProduce;

		/// <summary>
		/// Constructor for SendConfigClientCommandHandler class. 
		/// Initializes a new instance of the <see cref="SendConfigClientCommandHandler"/> class.
		/// </summary>
		/// <param name="repository">The repository interface for working with client configurations.</param>
		/// <param name="producer">Kafka's service is a producer for sending messages.</param>
		/// <param name="configuration">Getting the current application configuration.</param>
		public SendConfigClientCommandHandler(IClientConfigRepositoryAsync repository, IProducerService producer, IConfiguration configuration)
		{
			_repository = repository;
			_producer = producer;
			_configuration = configuration;
			_topicProduce = _configuration["Kafka:Topics:Send:NewClientConfiguration"];
		}
		/// <summary>
		/// Handles the <see cref="SendConfigClientCommand"/> command to send configuration settings.
		/// </summary>
		/// <param name="command">The command containing the configuration ID.</param>
		/// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{Guid}"/> with the system ID.</returns>
		/// <exception cref="APIException">Thrown when the configuration is not found.</exception>
		public async Task<Response<Guid>> Handle(SendConfigClientCommand command, CancellationToken cancellationToken)
		{
			var config = await _repository.GetByIdAsync(command.Id);
			if (config == null) throw new APIException($"Configuration Not Found.");

			var configDto = new ClientSettings
			{
				SystemId = config.SystemId,
				DBSettings = new DBSettings
				{
					DataBase = config.DBSettings.DataBase,
					Alarm = new AlarmDataBase
					{
						ConnectionString = config.DBSettings.Alarm.ConnectionString,
					},
					Notify = new NotifyDataBase
					{
						ConnectionString = config.DBSettings.Notify.ConnectionString,
					}
				},

				ModeSettings = new ModeSettings
				{
					ClientId = command.Id,
					UseCache = config.ModeSettings.UseCache,
					Mode = config.ModeSettings.Mode
				},

				ConnectSettings = new ConnectSettings
				{
					Notify = new NotifyConnection
					{
						Url = config.ConnectSettings.Notify.Url,
					},
					Alarm = new AlarmConnection
					{
						Url = config.ConnectSettings.Alarm.Url,
					}
				},

				KafkaSettings = new KafkaSettings
				{
					Consumer = new ConsumerConnection
					{
						BootstrapServers = config.KafkaSettings.Consumer.BootstrapServers
					},
					Producer = new ProducerConnection
					{
						BootstrapServers = config.KafkaSettings.Producer.BootstrapServers
					}
				}
			};
			
			var message = new MessageRequest
			{
				To = config.SystemId,
				Body = JsonConvert.SerializeObject(configDto, Formatting.Indented)
			};
			string json = JsonConvert.SerializeObject(message, Formatting.Indented);
			await _producer.ProduceMessageProcessAsync(_topicProduce, json, "config");
			return new Response<Guid>(config.SystemId, true);
		}
	}
}
