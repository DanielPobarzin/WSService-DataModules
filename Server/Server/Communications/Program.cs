using Communications.Connections;
using Communications.DTO;
using Communications.Helpers;
using Communications.Hubs;
using Communications.UoW;
using Entities.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System.Collections.Generic;
using System.Threading;


namespace Communications
{
	public class Program
	{
		private static UnitOfWorkGetConfig unitOfWorkConfig;
		private static UnitOfWorkGetNotifications unitOfWork;
		private static CheckHashHalper checkHashHalper;
		private static CancellationTokenSource cancellationTokenSource;
		private static Thread UoWThread;
		private static Thread hostThread;
		static object locker = new();
		public static IHost host;
		public static void Main(string[] args)
		{
			//---------- Logging ---------------//
			{
				Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Override("Microsoft", LogEventLevel.Verbose)
				.Enrich.FromLogContext()
				.Enrich.WithClientIp()
				.Enrich.WithExceptionData()
				.Enrich.WithMemoryUsage()
				.Enrich.WithProcessName()
				.Enrich.WithThreadName()
				.Enrich.WithDemystifiedStackTraces()
				.Enrich.WithRequestUserId()
				.WriteTo.Console(theme: SystemConsoleTheme.Colored, restrictedToMinimumLevel: LogEventLevel.Information)
				.WriteTo.Console(theme: SystemConsoleTheme.Colored, restrictedToMinimumLevel: LogEventLevel.Error)
				.WriteTo.File(AppContext.BaseDirectory + @"\Log\[VERBOSE]_NotificationExchange_Log_.log",
						rollingInterval: RollingInterval.Day,
						rollOnFileSizeLimit: true,
						retainedFileCountLimit: 365,
						shared: true,
						restrictedToMinimumLevel: LogEventLevel.Verbose)
				.WriteTo.File(AppContext.BaseDirectory + @"\Log\[ERROR]_NotificationExchange_Log_.log",
						rollingInterval: RollingInterval.Day,
						rollOnFileSizeLimit: true,
						retainedFileCountLimit: 365,
						shared: true,
						restrictedToMinimumLevel: LogEventLevel.Error)
				.WriteTo.File(AppContext.BaseDirectory + @"\Log\[INFO]_NotificationExchange_Log_.log",
						rollingInterval: RollingInterval.Day,
						rollOnFileSizeLimit: true,
						retainedFileCountLimit: 365,
						shared: true,
						restrictedToMinimumLevel: LogEventLevel.Information)
				.CreateLogger();
			}

			//--------------- Initialize get and check configuration -----------------//
			unitOfWorkConfig = new UnitOfWorkGetConfig();
			checkHashHalper = new CheckHashHalper();

			//--------------- Starting the host -----------------//
			hostThread = new Thread(CreateAndRunHostServer);
			hostThread.Start();

			//--------------- Start requesting data from the database -----------------//
			UoWThread = new Thread(StartListenNotifications);
			UoWThread.Start();

			//--------------- Defining actions when changing the configuration -----------------//
				
			GetEventChangeConfiguration();
		
			Task.Delay(Timeout.Infinite).Wait();
			Log.CloseAndFlush();

		}
		public static void StartListenNotifications()
		{
			lock (locker)
			{
				cancellationTokenSource = new CancellationTokenSource();
				CancellationToken cancellationToken = cancellationTokenSource.Token;
				
				//--------------- Determining the connection and starting to receive data -----------------//
				unitOfWork = new UnitOfWorkGetNotifications(unitOfWorkConfig.Configuration);
				unitOfWork.GetAllNotifications(cancellationToken);
			}
		}

		public static async void GetEventChangeConfiguration()
		{
			await foreach (var hashConfigs in checkHashHalper.CompareHashConfiguration(unitOfWorkConfig.sectionHashes))
			{
					if (hashConfigs.ContainsKey("HostSettings"))
					{
						await host.StopAsync();
						Log.Information("Changing the host configuration. Reboot ... ");
						hostThread.Join();
						Thread newHostThread = new Thread(CreateAndRunHostServer);
						newHostThread.Start();
						hostThread = newHostThread;
					} 

					if (hashConfigs.ContainsKey("DbConnection") ||
						hashConfigs.ContainsKey("NotificationsHubSettings"))
					{
						var comment = (hashConfigs.ContainsKey("DbConnection")) ?
							((hashConfigs.ContainsKey("NotificationsHubSettings")) ?
							"database & notify hub" : "database")
							: "Notify Hub";

						Log.Information($"Changing the configuration {comment}. Continue with the new configuration ... ");
						cancellationTokenSource.Cancel();
						Console.WriteLine(UoWThread.ThreadState);
						UoWThread.Join();
						Console.WriteLine(UoWThread.ThreadState);
						Thread newUoWThread = new Thread(StartListenNotifications);
						newUoWThread.Start();
						UoWThread = newUoWThread;
						Console.WriteLine(UoWThread.ThreadState);
				    }
			}
		}

		//------------- Hosting ------------------------------------------------------------------------//
		public static void CreateAndRunHostServer()    
		{
			host = Host.CreateDefaultBuilder()
		   .ConfigureWebHostDefaults(webBuilder =>
		   {
			   webBuilder.UseUrls(unitOfWorkConfig.Configuration["HostSettings:urls"].Split(";"));
			   webBuilder.ConfigureServices(services =>
			   {
				   services.AddMemoryCache();
				   //--------------- Connections -----------------//
				   services.AddSingleton(typeof(Connections<NotificationHub>));

				   //services.AddScoped<UnitOfWorkRealTime>();

				   //--------------- Сonfiguration provider  -----------------//
				   services.AddSingleton(provider =>
				   {
					   return unitOfWorkConfig.Configuration;
				   });

				   //--------------- Notification provider  -----------------//
				   services.AddSingleton(provider =>
				   {
					   return unitOfWork.ReceivedNotificationsList;
				   });

				   //--------------- CORS -----------------//
				   services.AddCors(options =>
				   {
					   options.AddPolicy("CorsPolicy",
						   builder => builder.AllowAnyMethod()
							   .AllowAnyHeader()
							   .AllowCredentials()
							   .WithExposedHeaders()
							   .WithOrigins(unitOfWorkConfig.Configuration["HostSettings:AllowedOrigins"].Split(";")));
					   options.AddPolicy("AllowAll",
						   builder => builder.AllowAnyMethod()
							   .AllowAnyHeader()
							   .WithExposedHeaders()
							   .AllowAnyOrigin());
				   });

				   //--------------- SignalR -----------------//
				   services.AddSignalR(configure =>
				   {
					   configure.KeepAliveInterval = TimeSpan.Parse(unitOfWorkConfig.Configuration["HostSettings:KeepAliveInterval"]);
					   configure.EnableDetailedErrors = bool.Parse(unitOfWorkConfig.Configuration["HostSettings:EnableDetailedErrors"]);
				   })
				   .AddJsonProtocol(options =>
				   {
					   options.PayloadSerializerOptions.PropertyNamingPolicy = null;
				   });
			   })
			   .Configure(app =>
			   {
				   app.UseRouting();
				   app.UseCors("CorsPolicy");
				   app.UseEndpoints(endpoints =>
				   {				
					   endpoints.MapHub<NotificationHub>(unitOfWorkConfig.Configuration["HostSettings:Route"], options =>
					   {
						   options.TransportMaxBufferSize = long.Parse(unitOfWorkConfig.Configuration["HostSettings:TransportMaxBufferSize"]);
						   options.Transports = HttpTransportType.WebSockets;
						   options.WebSockets.CloseTimeout = TimeSpan.Parse(unitOfWorkConfig.Configuration["HostSettings:CloseTimeout"]);
					   });
				   });
			   });

		   }).Build();
			host.Services.GetRequiredService<IHostApplicationLifetime>().ApplicationStarted.Register(() =>
			{
				var serverAddressesFeature = host.Services.GetService<IServer>().Features.Get<IServerAddressesFeature>();
				if (serverAddressesFeature != null)
				{
					Log.Information($"Host started. Listening on: {string.Join(", ", serverAddressesFeature.Addresses)}");
				}
			});
			host.Start();
			host.WaitForShutdown();
		}
	}
}

