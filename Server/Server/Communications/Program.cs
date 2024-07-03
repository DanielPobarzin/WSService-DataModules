using Communications.Connections;
using Communications.Helpers;
using Communications.Hubs;
using Communications.UoW;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;


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
			#region Logging 
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
			#endregion

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
						UoWThread.Join();
						Thread newUoWThread = new Thread(StartListenNotifications);
						newUoWThread.Start();
						UoWThread = newUoWThread;
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
				   //--------------- Helpers provider  -----------------//
				   services.AddScoped<TransformToDTOHelper>();
				   services.AddScoped<JsonCacheHelper>();

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
					   configure.MaximumReceiveMessageSize = 65_536;
					   configure.HandshakeTimeout = TimeSpan.FromSeconds(10);
					   configure.MaximumParallelInvocationsPerClient = 5;

				   })
				   .AddJsonProtocol(options =>
				   {
					   options.PayloadSerializerOptions.DefaultBufferSize = 65_536;
					   options.PayloadSerializerOptions.WriteIndented = true;
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
						   options.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling;
						   options.WebSockets.CloseTimeout = TimeSpan.Parse(unitOfWorkConfig.Configuration["HostSettings:CloseTimeout"]);
						   options.LongPolling.PollTimeout = TimeSpan.Parse(unitOfWorkConfig.Configuration["HostSettings:CloseTimeout"]);
						   options.CloseOnAuthenticationExpiration = false;
						   options.TransportSendTimeout = TimeSpan.FromSeconds(15);
						   options.ApplicationMaxBufferSize = 131_072;
						   options.TransportMaxBufferSize = 131_072;
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




