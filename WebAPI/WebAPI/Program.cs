using Confluent.Kafka;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared;


namespace WebAPI
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = Host.CreateDefaultBuilder()
			.ConfigureWebHostDefaults(webBuilder =>
			{
				webBuilder.UseUrls("https://localhost:443");
				webBuilder.ConfigureServices(services =>
				{
					services.AddControllers();
					services.AddEndpointsApiExplorer();
					services.AddSwaggerGen();
					var producerConfig = new ProducerConfig
					{
						BootstrapServers = $"localhost:{Helper.GetKafkaBrokerPort(
						Directory.GetParent(Environment.CurrentDirectory)?.FullName!)}",
						ClientId = "order-producer"
					};
					services.AddSingleton(new ProducerBuilder<string, string>(producerConfig).Build());

				}).Configure(app =>
				{

					app.UseSwagger();
					app.UseSwaggerUI();
					app.UseRouting();
					app.UseHttpsRedirection();

					app.UseAuthorization();
					app.UseEndpoints(endpoit =>
					{
						endpoit.MapControllers();
					});
				});
			}).Build();
			builder.Run();
		}
	}
	
	
}



		