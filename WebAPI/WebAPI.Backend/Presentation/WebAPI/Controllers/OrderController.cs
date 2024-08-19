using Application.Features.Configurations.Client.Queries.GetConfigClientList;
using Application.Interfaces;
using AutoMapper;
using Confluent.Kafka;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI.Controllers
{
	[Route("api/[controller]")]
	public class OrderController : ControllerBase
	{
		private readonly IProducerService _producer;
		private const string Topic = "test";
		public OrderController(IProducerService producer) => _producer = producer;


		[HttpPost("test")]
		public async Task<IActionResult> PlaceOrder(string orderDetails)
		{
			try
			{

				await _producer.ProduceMessageProcessAsync(Topic, orderDetails, "server-metric");

				return Ok("Order placed successfully");
			}
			catch (ProduceException<string, string> ex)
			{
				return BadRequest($"Error publishing message: {ex.Error.Reason}");
			}
		}
	}
}