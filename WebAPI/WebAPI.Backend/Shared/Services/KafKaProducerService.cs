using Application.Interfaces;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Shared.Services
{
	public class ProducerService : IProducerService
    {
        private readonly IConfiguration _configuration;
        private readonly IProducer<Null, string> _producer;
        private readonly ILogger<ProducerService> _logger;

        // KafKa configuration
        private readonly string _bootstrapServers;

		public ProducerService(IConfiguration configuration, ILogger<ProducerService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            _bootstrapServers = configuration["Kafka:BootstrapServers"];

            var producerconfig = new ProducerConfig
            {
                BootstrapServers = _bootstrapServers
            };
            _producer = new ProducerBuilder<Null, string>(producerconfig).Build();
        }

        public async Task ProduceMessageProcessAsync(string topic, string message)
        {
            var kafkaMessage = new Message<Null, string> { Value = message };
            try
            {
                var result = await _producer.ProduceAsync(topic, kafkaMessage);
                _logger.LogInformation($"Message sent to {result.TopicPartitionOffset}");
            }
            catch (ProduceException<Null, string> e)
            {
                _logger.LogError($"Failed to deliver message: {e.Message} [{e.Error.Code}]");
                // Optionally, implement retry logic here
            }
        }
    }
}
