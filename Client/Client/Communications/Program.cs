using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Client
{
	public class Program
	{
		public static HubConnection connection;
		static async Task Main(string[] args)
		{
			var host = CreateHostBuilder(args).Build();
			using (var scope = host.Services.CreateScope())
			{
				var serviceProvider = scope.ServiceProvider;
				connection = new HubConnectionBuilder()
				.WithUrl("https://localhost:7097/notificationhub", options =>
				{
					options.Transports = HttpTransportType.WebSockets;
				})
				.WithAutomaticReconnect()
				.Build();


				connection.ServerTimeout = TimeSpan.FromMinutes(2);
				var context = serviceProvider.GetRequiredService<ClientDbContext>();

				connection.On<ServerModel, Guid>("ShowNotification", async (ServerModelNotification, ServerId) =>
				{
					var client = new ClientModel()
					{
						ClientId = ClientId,
						ServerId = ServerId,
						Id = ServerModelNotification.Id,
						Content = ServerModelNotification.Content,
						Value = ServerModelNotification.Value,
						Quality = ServerModelNotification.Quality,
						CreationDate = ServerModelNotification.CreationDate
					};
					Console.WriteLine($"The ñlient {ClientId} received a notification {client.Id} from the server {ServerId}.");
					try
					{
						await context.NotificationsÑlient.AddAsync(client);
					}
					catch (Exception ex) { Console.WriteLine($"The exception: {ex}"); }
					finally { await context.SaveChangesAsync(); }
				});

				connection.Reconnecting += (exception) =>
				{
					Console.WriteLine("Connection lost. Reconnecting...");
					return Task.CompletedTask;
				};
				await connection.StartAsync();
				try { await connection.InvokeAsync("Enter", ClientId, "Group"); }
				catch (Exception ex) { Console.WriteLine($"{ex}"); }

				try { await connection.InvokeAsync("Send", ClientId, "Group"); }
				catch (Exception ex) { Console.WriteLine($"{ex}"); }
				host.Run();

			}
		}
		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });

		public class Startup
		{
			public IConfiguration Configuration { get; }
			public Startup(IConfiguration configuration)
			{
				Configuration = configuration;
			}
			public void ConfigureServices(IServiceCollection services)
			{
				services.AddSignalR();
				services.AddAutoMapper(config =>
				{
					config.AddProfile(new AssemblyMappingProfile(Assembly.GetExecutingAssembly()));
				});

				services.AddEntityExchange(Configuration);
			}
			public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
			{
				if (env.IsDevelopment())
				{
					app.UseDeveloperExceptionPage();
				}
				app.UseHttpsRedirection();
				app.UseRouting();
				app.UseEndpoints(endpoints =>
				{
					endpoints.MapHub<NotificationsHub>("/notificationhub");
				});
			}
		}
	}
}

