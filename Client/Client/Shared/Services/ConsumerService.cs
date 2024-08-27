using Confluent.Kafka;
using Entities.Enums;
using Interactors.Helpers;
using Interactors.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;
using Shared.Common;
using Shared.Share;
using System.Net;

namespace Shared.Services
{
	/// <summary>
	/// Represents a background service that consumes messages from Kafka topics.
	/// Implements <see cref="IConsumerService"/>.
	/// </summary>
	public class ConsumerService : BackgroundService, IConsumerService
	{
		private readonly IConsumer<string, string> _consumer;
		private readonly IConfiguration _configuration;
		private readonly string[] _bootstrapServers;
		private readonly string[] _topics;
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
			_bootstrapServers = _configuration["Kafka:Consumer:BootstrapServers"].Split(';');
			_groupId = "ClientsGroup";
			_topics = ["new-config-topic", "command-connect"];

			var consumerConfig = new ConsumerConfig
			{
				ClientId = Dns.GetHostEntry(Environment.MachineName).HostName,
				SecurityProtocol = SecurityProtocol.Plaintext,
				BootstrapServers = string.Join(',', _bootstrapServers),
				GroupId = _groupId,
				AutoOffsetReset = AutoOffsetReset.Earliest
			};
			_consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
		}

		/// <summary>
		/// Executes the background service logic to pull messages from Kafka.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token to monitor for cancellation requests.</param>
		/// <returns>A task that represents the asynchronous operation.</returns>
		protected override async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			_consumer.Subscribe(_topics);

			while (!cancellationToken.IsCancellationRequested)
			{
				PullMessageConsumerProcess();
				await Task.Delay(100, cancellationToken);
			}
			return;
		}

		/// <summary>
		/// Processes messages consumed from Kafka.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token to monitor for cancellation requests.</param>
		public void PullMessageConsumerProcess()
		{
			try
			{
				var consumeResult = _consumer.Consume(TimeSpan.FromMilliseconds(100));
				if (consumeResult == null) return;
				var message = consumeResult.Message;

				switch (message.Key)
				{
					case ("client-config"):
						var newConfig = JsonConvert.DeserializeObject<MessageRequest>(message.Value);
						if (newConfig == null || newConfig.To != Guid.Parse(_configuration["ClientSettings:ClientId"]))
						{ return; }
						schemaPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "schema.json");
						var schema = File.ReadAllText(schemaPath);
						if (ConfigValidHelper.ValidateConfigurationJson(schema, newConfig.Body))
						{
							filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "configure.json");
							var jsonNewConfig = JsonConvert.SerializeObject(newConfig.Body, Formatting.Indented);
							File.WriteAllText(filePath, jsonNewConfig);
						}
						break;

					case ("command"):
						var Command = JsonConvert.DeserializeObject<MessageRequest>(message.Value);
						if (Command == null || Command.To != Guid.Parse(_configuration["ClientSettings:ClientId"]))
						{ return; }
						GlobalSingletonParameters.Instance.ConnectionCommand = (ConnectionCommand)Enum.Parse(typeof(ConnectionMode), Command.Body);
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
			GC.SuppressFinalize(this);
		}
	}
}

