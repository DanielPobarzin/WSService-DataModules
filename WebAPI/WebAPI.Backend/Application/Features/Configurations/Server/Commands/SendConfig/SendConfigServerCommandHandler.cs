using Application.DTOs.Message;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using Domain.Settings.SignalRServer;
using MediatR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Application.Features.Configurations.Server.Commands.SendConfig
{
	public class SendConfigServerCommandHandler : IRequestHandler<SendConfigServerCommand, Response<Guid>>
	{
		private readonly IServerConfigRepositoryAsync _repository;
		private readonly IProducerService _producer;
		private readonly IConfiguration _configuration;
		private readonly string _topicProduce;

		/// <summary>
		/// Initializes a new instance of the <see cref="SendConfigServerCommandHandler"/> class.
		/// </summary>
		/// <param name="repository">The repository used for accessing server configurations.</param>
		/// <param name="producer">The service used for producing messages to a message broker.</param>
		/// <param name="configuration">The configuration settings for the application.</param>
		public SendConfigServerCommandHandler(IServerConfigRepositoryAsync repository, IProducerService producer, IConfiguration configuration)
		{
			_repository = repository;
			_producer = producer;
			_configuration = configuration;
			_topicProduce = _configuration["Kafka:Topic:Send:NewServerConfiguration"];
		}

		/// <summary>
		/// Handles the sending of a server configuration.
		/// </summary>
		/// <param name="command">The command containing the ID of the server configuration to send.</param>
		/// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{Guid}"/> with the ID of the sent server configuration.</returns>
		/// <exception cref="APIException">Thrown when the specified configuration is not found.</exception>
		public async Task<Response<Guid>> Handle(SendConfigServerCommand command, CancellationToken cancellationToken)
		{
			var config = await _repository.GetByIdAsync(command.Id) ?? throw new APIException($"Configuration Not Found.");
			var configDto = new ServerSettings
			{
				DbConnection = new DBSettings
				{
					DataBase = config.DbConnection.DataBase,

					Alarm = new AlarmConnection
					{
						ConnectionString = config.DbConnection.Alarm.ConnectionString,
					},
					Notify = new NotifyConnection
					{
						ConnectionString = config.DbConnection.Notify.ConnectionString,
					}
				},

				HubSettings = new HubSettings
				{
					ServerId = config.HubSettings.ServerId,

					Alarm = new AlarmHubSettings
					{
						DelayMilliseconds = config.HubSettings.Alarm.DelayMilliseconds,
						HubMethod = config.HubSettings.Alarm.HubMethod,
						TargetClients = config.HubSettings.Alarm.TargetClients
					},

					Notify = new NotifyHubSettings
					{
						DelayMilliseconds = config.HubSettings.Notify.DelayMilliseconds,
						HubMethod = config.HubSettings.Notify.HubMethod,
						TargetClients = config.HubSettings.Notify.TargetClients
					}
				},

				HostSettings = new HostSettings
				{
					Port = config.HostSettings.Port,
					Urls = config.HostSettings.Urls,
					PolicyName = config.HostSettings.PolicyName,
					AllowedOrigins = config.HostSettings.AllowedOrigins,
					RouteNotify = config.HostSettings.RouteNotify,
					RouteAlarm = config.HostSettings.RouteAlarm
				},
				Kafka = new KafkaSettings
				{
					Consumer = new ConsumerConnection
					{
						BootstrapServers = config.Kafka.Consumer.BootstrapServers
					},
					Producer = new ProducerConnection
					{
						BootstrapServers = config.Kafka.Producer.BootstrapServers
					}
				}
			};

			var message = new MessageRequest
			{
				To = config.HubSettings.ServerId,
				Body = JsonConvert.SerializeObject(configDto, Formatting.Indented)
			};

			string json = JsonConvert.SerializeObject(message, Formatting.Indented);
			await _producer.ProduceMessageProcessAsync(_topicProduce, json, "server-config");
			return new Response<Guid>(config.HubSettings.ServerId, true);
		}
	}
}

