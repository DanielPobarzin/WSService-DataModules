using Application.Interfaces;
using Application.Mappings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Services;
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
			services.AddTransient<IDateTimeService, DateTimeService>();
			services.AddSingleton<IProducerService, ProducerService>();
			services.AddSingleton<IConsumerService, ConsumerService>();

			return services;
		}
	}
}
