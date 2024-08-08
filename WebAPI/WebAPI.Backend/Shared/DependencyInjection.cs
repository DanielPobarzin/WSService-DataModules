using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Services;

namespace Shared
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddShared(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddTransient<IDateTimeService, DateTimeService>();
			services.AddTransient<IProducerService, ProducerService>();
			services.AddTransient<IConsumerService, ConsumerService>();

			return services;
		}
	}
}
