using Confluent.Kafka;
using Interactors.Interfaces;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Services
{
	public class ProducerService : IProducerService, IDisposable
	{
		private readonly IConfiguration _configuration;
		private readonly IProducer<string, string> _producer;
		private readonly string _bootstrapServers;
		public ProducerService(IConfiguration configuration)
		{
			_configuration = configuration;
			_bootstrapServers = _configuration["Kafka:Producer:BootstrapServers"];

			var producerConfig = new ProducerConfig
			{
				BootstrapServers = _bootstrapServers
			};
			_producer = new ProducerBuilder<string, string>(producerConfig).Build();
		}
		public async Task PutMessageProducerProcessAsync(string topic, string message, string key)
		{
			var kafkaMessage = new Message<string, string> { Value = message, Key = key };
			try
			{
				var result = await _producer.ProduceAsync(topic, kafkaMessage);
				if (key != "metric")
				{
					Log.Information($"Message sent to {result.TopicPartitionOffset}");
				}
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
