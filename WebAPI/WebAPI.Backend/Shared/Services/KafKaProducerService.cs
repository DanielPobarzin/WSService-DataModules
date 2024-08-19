using Application.Interfaces;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Shared.Services
{
	public class ProducerService : IProducerService
    {
        private readonly IConfiguration _configuration;
        private readonly IProducer<string, string> _producer;
		private readonly string[] _bootstrapServers;

		public ProducerService(IConfiguration configuration)
        {
            _configuration = configuration;
			_bootstrapServers = _configuration["Kafka:BootstrapServers"].Split(';');

            var producerconfig = new ProducerConfig
            {
                BootstrapServers = string.Join(",", _bootstrapServers)
			};
            _producer = new ProducerBuilder<string, string>(producerconfig).Build();
        }

        public async Task ProduceMessageProcessAsync(string topic, string message, string key)
        {
            var kafkaMessage = new Message<string, string> { Value = message, Key = key };
            try
            {
                var result = await _producer.ProduceAsync(topic, kafkaMessage);
                Log.Information($"Message sent to {result.TopicPartitionOffset}");
            }
            catch (ProduceException<string, string> e)
            {
				Log.Error($"Failed to deliver message: {e.Message} [{e.Error.Code}]");
            }
        }
		public void Dispose()
		{
			_producer?.Dispose();
		}
	}
}
