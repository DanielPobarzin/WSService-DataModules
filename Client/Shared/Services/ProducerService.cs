using Confluent.Kafka;
using Interactors.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using Serilog.Core;
using Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Confluent.Kafka.ConfigPropertyNames;

namespace Shared.Services
{
    public class ProducerService : IProducerService, IDisposable
    {
		private readonly IConfiguration _configuration;
		private readonly IProducer<string, string> _producer;
		private readonly string _bootstrapServers;
		private readonly string _topic;

		public ProducerService(IConfiguration configuration)
		{
			_configuration = configuration;
			_bootstrapServers = _configuration["Kafka:BootstrapServers"];
			_topic = _configuration["Kafka:Topic"];

			var producerConfig = new ProducerConfig
			{
				BootstrapServers = _bootstrapServers
			};
		    _producer = new ProducerBuilder<string, string>(producerConfig).Build();
		}

		public async Task PutMessageProducerProcessAsync(string topic, string message, string Key)
		{
			var kafkaMessage = new Message<string, string> { Value = message, Key = Key };
			try
			{
				var result = await _producer.ProduceAsync(topic, kafkaMessage);
				Log.Information($"Message sent to {result.TopicPartitionOffset}");
			}
			catch (ProduceException<Null, string> e)
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