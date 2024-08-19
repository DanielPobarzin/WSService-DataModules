using Application.Interfaces;
using Application.Mappings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Monitoring;
using Shared.Services;
using System.Diagnostics.Metrics;
using System.Reflection;

namespace Shared
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddShared(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddAutoMapper(config =>
			{
				config.AddProfile(new AssemblyMappingProfile(Assembly.GetExecutingAssembly()));
			});
			services.AddSingleton(provider =>
			new TelemetryClientUsingPrometheus(new Meter("ClientComponents")));
			services.AddSingleton(provider =>
			new TelemetryServerUsingPrometheus(new Meter("ServerComponents")));

			services.AddTransient<IDateTimeService, DateTimeService>();
			services.AddSingleton<IProducerService, ProducerService>();
			services.AddSingleton<IConsumerService, ConsumerService>();
	
			return services;
		}
	}
}
