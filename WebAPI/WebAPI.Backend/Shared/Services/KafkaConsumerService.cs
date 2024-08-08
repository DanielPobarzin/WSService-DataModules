using Application.Interfaces;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Shared.Services
{
	public class ConsumerService : BackgroundService, IConsumerService
    {
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly ILogger<ConsumerService> _logger;
        private readonly IConfiguration _configuration;


        // KafKa configuration
        private readonly string _bootstrapServers;
        private readonly string _topic;
        private readonly string _groupId;

        public ConsumerService(IConfiguration configuration, ILogger<ConsumerService> logger)
        {

            _logger = logger;
            _configuration = configuration;

            _bootstrapServers = _configuration["Kafka:BootstrapServers"];
            _groupId = _configuration["Kafka:GroupId"];
            _topic = _configuration["Kafka:Topic"];

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = _bootstrapServers,
                GroupId = _groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            _consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Subscribe(_topic);

            while (!stoppingToken.IsCancellationRequested)
            {
				KafkaPullMessageProcess(stoppingToken);
                await Task.Delay(100, stoppingToken);
            }

            _consumer.Close();
        }
        public void KafkaPullMessageProcess(CancellationToken stoppingToken)
        {
            try
            {
                var consumeResult = _consumer.Consume(stoppingToken);
                var message = consumeResult.Message.Value;

                _logger.LogInformation($"Received message: {message}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing Kafka message: {ex.Message}");
            }
        }
    }

}
