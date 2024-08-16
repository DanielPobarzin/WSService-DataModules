using Application.Features.Configurations.Client.Commands.CreateConfig;
using Application.Features.Configurations.Client.Commands.UpdateConfig;
using Application.Features.Configurations.Server.Commands.CreateConfig;
using Application.Features.Configurations.Server.Commands.UpdateConfig;
using Application.Features.Connections.Commands.AddConnection;
using Application.Features.Connections.Commands.UpdateConnection;
using Application.Interfaces;
using Application.Wrappers;
using AutoMapper;
using Confluent.Kafka;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using Shared.Models.Config;
using Shared.Models.Connection;
using Shared.Models.Server;
using System.Threading;
using static Confluent.Kafka.ConfigPropertyNames;

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

		public ConsumerService(IConfiguration configuration, IMapper mapper, IMediator mediator)
		{
			_configuration = configuration;
			_mediator = mediator;
			_mapper = mapper;
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
	}
}
