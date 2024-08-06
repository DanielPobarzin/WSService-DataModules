﻿using Confluent.Kafka;
using Interactors.Helpers;
using Interactors.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;
using Shared.Share.KafkaMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Services
{
	/// <summary>
	/// Represents a background service that consumes messages from Kafka topics.
	/// Implements <see cref="IConsumerService"/> and <see cref="IDisposable"/>.
	/// </summary>
	public class ConsumerService : BackgroundService, IConsumerService, IDisposable
	{
		private readonly IConsumer<string, string> _consumer;
		private readonly IConfiguration _configuration;
		private readonly string _bootstrapServers;
		private readonly string _topicConfig;
		private readonly string _groupId;
		private string filePath;
		private string schemaPath;

		/// <summary>
		/// Initializes a new instance of the <see cref="ConsumerService"/> class.
		/// </summary>
		/// <param name="configuration">The configuration settings for the consumer.</param>
		public ConsumerService(IConfiguration configuration)
		{
			_configuration = configuration;
			_bootstrapServers = _configuration["Kafka:Consumer:BootstrapServers"];
			_groupId = "servers";
			_topicConfig = "new-server-config-topic";

			var consumerConfig = new ConsumerConfig
			{
				BootstrapServers = _bootstrapServers,
				GroupId = _groupId,
				AutoOffsetReset = AutoOffsetReset.Earliest
			};
			_consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
		}

		/// <summary>
		/// Starts the consumer service and subscribes to the specified Kafka topics.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token to monitor for cancellation requests.</param>
		/// <returns>A task that represents the asynchronous operation.</returns>
		public override Task StartAsync(CancellationToken cancellationToken)
		{
			_consumer.Subscribe(_topicConfig);
			return Task.CompletedTask;
		}

		/// <summary>
		/// Executes the background service logic to pull messages from Kafka.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token to monitor for cancellation requests.</param>
		/// <returns>A task that represents the asynchronous operation.</returns>
		protected override async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				PullMessageConsumerProcess(cancellationToken);
				await Task.Delay(100, cancellationToken);
			}
			return;
		}

		/// <summary>
		/// Processes messages consumed from Kafka.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token to monitor for cancellation requests.</param>
		public void PullMessageConsumerProcess(CancellationToken cancellationToken)
		{
			try
			{
				var consumeResult = _consumer.Consume(cancellationToken);
				var message = consumeResult.Message;
				if (message is null) { return; }

				switch (message.Key)
				{
					case ("config"):
						var newConfig = JsonConvert.DeserializeObject<KafkaMessageConfig>(message.Value);
						if (newConfig == null || newConfig.ServerId != Guid.Parse(_configuration["HubSettings:ServerId"]))
						{ return; }
						schemaPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "schema.json");
						var schema = File.ReadAllText(schemaPath);
						if (ConfigValidHelper.ValidateConfigurationJson(schema, newConfig.JsonConfig))
						{
							filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "configure.json");
							var jsonNewConfig = JsonConvert.SerializeObject(newConfig.JsonConfig, Formatting.Indented);
							File.WriteAllText(filePath, jsonNewConfig);
						}
					break;
				}
			}
			catch (Exception ex)
			{
				Log.Error($"Error processing Kafka message: {ex.Message}");
				return;
			}
		}

		/// <summary>
		/// Stops the consumer service and closes the consumer.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token to monitor for cancellation requests.</param>
		/// <returns>A task that represents the asynchronous operation.</returns>
		public override Task StopAsync(CancellationToken cancellationToken)
		{
			_consumer.Close();
			Dispose();
			return Task.CompletedTask;
		}

		/// <summary>
		/// Disposes of the resources used by the consumer.
		/// </summary>
		public override void Dispose()
		{
			_consumer?.Dispose();
		}
	}
}
