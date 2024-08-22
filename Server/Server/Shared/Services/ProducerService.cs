using Confluent.Kafka;
using Interactors.Interfaces;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Shared.Services
{
	/// <summary>
	/// Represents a service that produces messages to Kafka topics.
	/// Implements <see cref="IProducerService"/> and <see cref="IDisposable"/>.
	/// </summary>
	public class ProducerService : IProducerService, IDisposable
	{
		private readonly IConfiguration _configuration;
		private readonly IProducer<string, string> _producer;
		private readonly string[] _bootstrapServers;

		/// <summary>
		/// Initializes a new instance of the <see cref="ProducerService"/> class.
		/// </summary>
		/// <param name="configuration">The configuration settings for the producer.</param>
		public ProducerService(IConfiguration configuration)
		{
			_configuration = configuration;
			_bootstrapServers = _configuration["Kafka:Producer:BootstrapServers"].Split(';');

			var producerConfig = new ProducerConfig
			{
				SecurityProtocol = SecurityProtocol.Plaintext,
				BootstrapServers = string.Join(",", _bootstrapServers)
			};
			_producer = new ProducerBuilder<string, string>(producerConfig).Build();
		}

		/// <summary>
		/// Sends a message to the specified Kafka topic asynchronously.
		/// </summary>
		/// <param name="topic">The Kafka topic to which the message will be sent.</param>
		/// <param name="message">The message to send.</param>
		/// <param name="key">The key associated with the message.</param>
		/// <returns>A task that represents the asynchronous operation.</returns>
		public async Task PutMessageProducerProcessAsync(string topic, string message, string key)
		{
			var kafkaMessage = new Message<string, string> { Value = message, Key = key };
			try
			{
				var result = await _producer.ProduceAsync(topic, kafkaMessage);
				if (key != "metric")
				{
					Log.Information($"Message sent to {result.TopicPartitionOffset} with Key : {key}");
				}
			}
			catch (ProduceException<Null, string> e)
			{
				Log.Error($"Failed to deliver message: {e.Message} [{e.Error.Code}]");
			}
		}

		/// <summary>
		/// Disposes of the resources used by the producer.
		/// </summary>
		public void Dispose()
		{
			_producer?.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
