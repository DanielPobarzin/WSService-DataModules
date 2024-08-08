using Application.Interfaces;
using Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistance.DbContexts;
using Persistance.Repositories;

namespace Persistance
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddPersistance(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDbContext<ConnectionDbContext>(options =>
			options.UseNpgsql(
				configuration.GetConnectionString("ConnectionDataBase"),
				b => b.MigrationsAssembly(typeof(ConnectionDbContext).Assembly.FullName)));

			services.AddDbContext<ClientConfigDbContext>(options =>
			options.UseNpgsql(
				configuration.GetConnectionString("ClientConfigDataBase"),
				b => b.MigrationsAssembly(typeof(ClientConfigDbContext).Assembly.FullName)));

			services.AddDbContext<ServerConfigDbContext>(options =>
			options.UseNpgsql(
				configuration.GetConnectionString("ServerConfigDataBase"),
				b => b.MigrationsAssembly(typeof(ServerConfigDbContext).Assembly.FullName)));

			#region Repositories

			services.AddTransient(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<,>));
			services.AddTransient<IClientConfigRepositoryAsync, ClientConfigRepository>();
			services.AddTransient<IServerConfigRepositoryAsync, ServerConfigRepository>();
			services.AddTransient<IClientRepositoryAsync, ClientRepository>();
			services.AddTransient<IServerRepositoryAsync, ServerRepository>();

			#endregion Repositories
			return services;
		}
	}
}