using Confluent.Kafka;
using Interactors.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Confluent.Kafka.ConfigPropertyNames;

namespace Shared.Services
{
    public class ConsumerService<T> : BackgroundService, IConsumerService<T>, IDisposable
		where T : class
    {
		private readonly IConsumer<string, string> _consumer;
		private readonly IConfiguration _configuration;

		private readonly string _bootstrapServers;
		private readonly string _topic;
		private readonly string _groupId;
		public ConsumerService (IConfiguration configuration)
		{
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
			_consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
		}
		public override Task StartAsync(CancellationToken cancellationToken)
		{
			_consumer.Subscribe(_topic);
			return Task.CompletedTask;
		}
		protected override async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				PullMessageConsumerProcess(cancellationToken);
				await Task.Delay(100, cancellationToken);
			}
			return;
		}
		public T? PullMessageConsumerProcess(CancellationToken cancellationToken)
		{
			try
			{
				var consumeResult = _consumer.Consume(cancellationToken);
				var message = consumeResult.Message;
				if (message is null) { return null; }
				
				if (message.Key == "config")
				{
					
				}
				try
				{
					var entity = JsonConvert.DeserializeObject<T>(message.Value);
					Log.Information($"Received message type of {typeof(T).Name}: ");
					if (entity is null)
					{
						Log.Warning($"Deserialized object is null for message: {message.Value}");
					}
					return entity;
				}
				catch (JsonException ex)
				{
					Log.Error($"Failed to deserialize message to type {typeof(T).Name}: {ex.Message}"); return null;
				}
			}
			catch (Exception ex)
			{
				Log.Error($"Error processing Kafka message: {ex.Message}");
				return null;
			}
		}
		public override Task StopAsync(CancellationToken cancellationToken)
		{
			_consumer.Close();
			_consumer.Dispose();
			return Task.CompletedTask;
		}
		public override void Dispose()
		{
			_consumer?.Dispose();
		}
	}
}
