using Application.Features.Configurations.Client.Commands.CreateConfig;
using Application.Features.Configurations.Client.Commands.UpdateConfig;
using Application.Features.Configurations.Server.Commands.CreateConfig;
using Application.Features.Configurations.Server.Commands.UpdateConfig;
using Application.Features.Connections.Commands.UpdateConnection;
using Application.Features.Servers.Commands.AddServer;
using Application.Features.Servers.Commands.UpdateServer;
using Application.Interfaces;
using Application.Wrappers;
using AutoMapper;
using Confluent.Kafka;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;
using Shared.Models.Config;
using Shared.Models.Connection;
using Shared.Models.Server;
using Shared.Monitoring;
using Shared.Monitoring.ServerMetrics;
using System.Text;
using System.Threading;

namespace Shared.Services
{
	public class ConsumerService : BackgroundService, IConsumerService
    {
        private readonly IConsumer<string, string> _consumer;
		private readonly IMediator _mediator;
		private readonly IConfiguration _configuration;
		private readonly string[] _bootstrapServers;
		private readonly Dictionary<string, string> _topics;
        private readonly string _groupId;
		private readonly IMapper _mapper;
		private readonly IMemoryCache memory;
		private readonly TelemetryClientUsingPrometheus _clientTelemetryMetrics;
		private readonly TelemetryServerUsingPrometheus _serverTelemetryMetrics;
		public ConsumerService(IConfiguration configuration, 
							   IMapper mapper, 
							   IMediator mediator, 
							   IMemoryCache memory, 
							   TelemetryClientUsingPrometheus clientMetrics, 
							   TelemetryServerUsingPrometheus serverMetrics)
		{
			_configuration = configuration;
			_mediator = mediator;
			_mapper = mapper;
			this.memory = memory;
			_clientTelemetryMetrics = clientMetrics;
			_serverTelemetryMetrics = serverMetrics;
			_bootstrapServers = _configuration["Kafka:Producer:BootstrapServers"].Split(';');
			_groupId = "ManagmentMonitorServiceGroup";
			var topicsSection = _configuration.GetSection("Kafka:Topics:Recieve");

			foreach (var child in topicsSection.GetChildren())
			{
				_topics[child.Key] = child.Value;
			}

			var consumerConfig = new ConsumerConfig
			{
				BootstrapServers = string.Join(',', _bootstrapServers),
				GroupId = _groupId,
				AutoOffsetReset = AutoOffsetReset.Earliest
			};
			_consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
			_mediator = mediator;
		}
		public override Task StartAsync(CancellationToken stoppingToken)
		{
			_consumer.Subscribe(_topics.Values);
			return Task.CompletedTask;
		}
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
 
            while (!stoppingToken.IsCancellationRequested)
            {
				await KafkaPullMessageProcess(stoppingToken);
                await Task.Delay(100, stoppingToken);
            }
			return;
		}
        public async Task KafkaPullMessageProcess(CancellationToken stoppingToken)
        {
            try
            {
                var consumeResult = _consumer.Consume(stoppingToken);
                var message = consumeResult.Message;
				if (message is null) { return; }
				switch (message.Key)
				{
					case ("server-metric"):

						var serverMetrics = JsonConvert.DeserializeObject<ServerMetrics>(message.Value);
						if (serverMetrics != null)
						{
							_serverTelemetryMetrics.AddTotalCountMessages(serverMetrics.TotalCountMessages, serverMetrics.ServerId);
							_serverTelemetryMetrics.AddTotalCountAlarms(serverMetrics.CountAlarms, serverMetrics.ServerId);
							_serverTelemetryMetrics.AddTotalCountNotifications(serverMetrics.CountNotifications, serverMetrics.ServerId);
							_serverTelemetryMetrics.RecordTotalMessagesSize(serverMetrics.TotalMessagesSize);
							_serverTelemetryMetrics.RecordLatency(serverMetrics.Latency);
							_serverTelemetryMetrics.RecordWorkingMemoryUsage(serverMetrics.WorkingMemoryUsage);
							_serverTelemetryMetrics.RecordPrivateMemoryUsage(serverMetrics.PrivateMemoryUsage);
							_serverTelemetryMetrics.ChangeAverageMessageSize(serverMetrics.AverageMessageSize);
							_serverTelemetryMetrics.ChangeCountListeners(serverMetrics.CountListeners);
							var server = new AddServerDTO
							{
								 ServerId = serverMetrics.ServerId,
								 WorkStatus = serverMetrics.WorkStatus,
								 ConnectionStatus = serverMetrics.ConnectionStatus,
								 CountListeners = serverMetrics.CountListeners
							};
							if (!memory.TryGetValue(server.ServerId, out Server? Server))
							{
								var commandDTO = _mapper.Map<AddServerCommand>(server);
								Response<Server> responses = (Response<Server>)await _mediator.Send(commandDTO);
								Log.Information($"Response Consumer command: {responses.Data}; Successed: {responses.Succeeded}");
								memory.Set(server.ServerId, server);
							}
							else
							{
								var updateServer = new UpdateServerDTO
								{
									ServerId = serverMetrics.ServerId,
									WorkStatus = serverMetrics.WorkStatus,
									ConnectionStatus = serverMetrics.ConnectionStatus,
									CountListeners = serverMetrics.CountListeners
								};
								if (!IsEqual(Server, server))
								{
									var commandDTO = _mapper.Map<UpdateServerCommand>(updateServer);
									Response<Server> responses = (Response<Server>)await _mediator.Send(commandDTO);
									Log.Information($"Response Consumer command: {responses.Data}; Successed: {responses.Succeeded}");
								}
							}


						}
						break;

					case ("client-metric"):
						
						break;
					case ("new-connection"):
						await ConsumerCommandHandler<string, AddConnectionDTO, UpdateConnectionCommand>(message.Value, stoppingToken);
					break;

					case ("close-connection"):
						await ConsumerCommandHandler<string, UpdateConnectionDTO, UpdateConnectionCommand>(message.Value, stoppingToken);
					break;

					case ("server-config"):
						try
						{
							await ConsumerCommandHandler<Guid, CreateServerConfigDTO, CreateConfigServerCommand>(message.Value, stoppingToken);
						}
						catch (Exception ex)
						{
							Log.Error($"Error with add current config: {ex.Message}");
							await ConsumerCommandHandler<Guid, UpdateServerConfigDTO, UpdateConfigServerCommand>(message.Value, stoppingToken);
						}
						finally
						{
							Log.Error($"Warning! Incorrect operation of the Consumer service!");
						}
					break;

					case ("client-config"):
						try
						{
							await ConsumerCommandHandler<Guid, CreateClientConfigDTO, CreateConfigClientCommand>(message.Value, stoppingToken);
						}
						catch (Exception ex)
						{
							Log.Error($"Error with add current config: {ex.Message}");
							await ConsumerCommandHandler<Guid, UpdateClientConfigDTO, UpdateConfigClientCommand>(message.Value, stoppingToken);
						}
						finally
						{
							Log.Error($"Warning! Incorrect operation of the Consumer service!");
						}
					break;
				}
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error processing Kafka message.");
            }
        }
		public override Task StopAsync(CancellationToken stoppingToken)
		{
			_consumer.Close();
			Dispose();
			return Task.CompletedTask;
		}

		public override void Dispose()
		{
			_consumer?.Dispose();
		}
		public async Task ConsumerCommandHandler<TResponse, TMessage, TCommand> (string message, CancellationToken cancellationToken)
		{
			Log.Information($"Received message: {message}");
			var messageDTO = JsonConvert.DeserializeObject<TMessage>(message);
			if (messageDTO != null)
			{
				var commandDTO = _mapper.Map<TCommand>(messageDTO);
				Response<TResponse> response = (Response<TResponse>) await _mediator.Send(commandDTO, cancellationToken);
				Log.Information($"Response Consumer command: {response.Data}; Successed: {response.Succeeded}");
			}
		}
		private bool IsEqual(Server server, AddServerDTO dto)
		{
			return server.CountListeners == dto.CountListeners &&
				   server.ConnectionId == dto.ConnectionId &&
				   server.ConnectionStatus == dto.ConnectionStatus &&
				   server.WorkStatus == dto.WorkStatus;
		}
	}
}
