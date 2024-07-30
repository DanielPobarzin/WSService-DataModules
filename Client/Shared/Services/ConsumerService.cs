using Confluent.Kafka;
using Interactors.Enums;
using Interactors.Helpers;
using Interactors.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;
using Shared.Common;

namespace Shared.Services
{
	public class ConsumerService<T> : BackgroundService, IConsumerService<T>, IDisposable
		where T : class
    {
		private readonly IConsumer<string, string> _consumer;
		private readonly IConfiguration _configuration;
		private readonly string _bootstrapServers;
		private readonly string _topicConfig;
		private readonly string _topicConnect;
		private readonly string _groupId;
		public ConsumerService (IConfiguration configuration)
		{
			_configuration = configuration;
			_bootstrapServers = _configuration["Kafka:BootstrapServers"];
			_groupId = "clients";
			_topicConfig = "get-client-config-topic";
			_topicConnect = "get-command-client-connect";

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
			_consumer.Subscribe(_topicConfig);
			_consumer.Subscribe(_topicConnect);
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
						


							break;
					case ("command"):
						

						break;
				}
			}
			catch (Exception ex)
			{
				Log.Error($"Error processing Kafka message: {ex.Message}");
				return;
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
