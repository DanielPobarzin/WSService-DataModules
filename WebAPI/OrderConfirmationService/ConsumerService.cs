using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderConfirmationService
{
	public class ConsumerService(IConsumer<string, string> consumer) : IHostedService
	{
		private readonly IConsumer<string, string> _consumer = consumer;

		public Task StartAsync(CancellationToken cancellationToken)
		{
			_consumer.Subscribe("order-events");

			Task.Run(() =>
			{
				while (!cancellationToken.IsCancellationRequested)
				{
					var consumeResult = _consumer.Consume(cancellationToken);

					if (consumeResult is null)
					{
						return;
					}

					var orderDetails = JsonConvert.DeserializeObject<OrderDetails>(consumeResult.Message.Value);

					Console.WriteLine($"Received message: " +
						$"Order Id: {orderDetails?.OrderId}, Product name: {orderDetails?.ProductName}, " +
						$"Price: {orderDetails?.Price}, Order date: {orderDetails?.OrderDate}");
				}
			}, cancellationToken);

			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}
