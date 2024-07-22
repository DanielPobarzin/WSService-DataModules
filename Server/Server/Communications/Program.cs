using Communications.Connections;
using Communications.Helpers;
using Communications.Hubs;
using Communications.UoW;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Repositories.Connections;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;

namespace Communications
{
	public class Program
	{
		private static UnitOfWorkGetConfig unitOfWorkConfig;
		private static UnitOfWorkGetNotifications unitOfWorkNotify;
		private static UnitOfWorkGetAlarms unitOfWorkAlarm;
		private static CheckHashHalper checkHashHalper;

		private static CancellationTokenSource cancellationTokenNotifySource;
		private static CancellationTokenSource cancellationTokenAlarmSource;

		private static Thread UoWNotifyThread;
		private static Thread UoWAlarmThread;
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
			UoWNotifyThread = new Thread(StartListenNotifications);
			UoWNotifyThread.Start();

			//--------------- Start requesting data from the database -----------------//
			//UoWAlarmThread = new Thread(StartListenAlarms);
			//UoWAlarmThread.Start();

			//--------------- Defining actions when changing the configuration -----------------//

			GetEventChangeConfiguration();
		
			Task.Delay(Timeout.Infinite).Wait();
			Log.CloseAndFlush();

		}
		public static void StartListenNotifications()
		{
			lock (locker)
			{
				cancellationTokenNotifySource = new CancellationTokenSource();
				CancellationToken cancellationToken = cancellationTokenNotifySource.Token;

				//--------------- Determining the connection and starting to receive data -----------------//
				unitOfWorkNotify = new UnitOfWorkGetNotifications(unitOfWorkConfig.Configuration);
				unitOfWorkNotify.GetAllNotifications(cancellationToken);
			}
		}

		public static void StartListenAlarms()
		{
			cancellationTokenAlarmSource = new CancellationTokenSource();
			CancellationToken cancellationToken = cancellationTokenAlarmSource.Token;

			//--------------- Determining the connection and starting to receive data -----------------//
			unitOfWorkAlarm = new UnitOfWorkGetAlarms(unitOfWorkConfig.Configuration);
			unitOfWorkAlarm.GetAllAlarms(cancellationToken);
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
						hashConfigs.ContainsKey("HubSettings"))
					{
						var comment = (hashConfigs.ContainsKey("DbConnection")) ?
							((hashConfigs.ContainsKey("HubSettings")) ?
							"database & hub" : "database")
							: "Hub";

						Log.Information($"Changing the configuration {comment}. Continue with the new configuration ... ");
						cancellationTokenNotifySource.Cancel();
						//cancellationTokenAlarmSource.Cancel();
						UoWNotifyThread.Join();
						//UoWAlarmThread.Join();
						Thread newUoWNotifyThread = new Thread(StartListenNotifications);
						//Thread newUoWAlarmThread = new Thread(StartListenAlarms);
						newUoWNotifyThread.Start();
						//newUoWAlarmThread.Start();
						UoWNotifyThread = newUoWNotifyThread;
						//UoWAlarmThread = newUoWAlarmThread;
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

				   //--------------- Сonfiguration provider  -----------------//
				   services.AddSingleton(provider =>
				   {
					   return unitOfWorkConfig.Configuration;
				   });

				   //--------------- Notification provider  -----------------//
				   services.AddSingleton(provider =>
				   {
					   return unitOfWorkNotify.ReceivedNotificationsList;
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

				   services.AddMvcCore().AddApiExplorer();

				   //--------------- Swagger -----------------//
				   services.AddSwaggerGen(c =>
				   {
					   var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
					   var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);
					   c.IncludeXmlComments(xmlCommentsFullPath);

					   c.SwaggerDoc("v1", new OpenApiInfo { Title = "WSService", Version = "v1", Description = "SelfHost websocket service." });
					   c.AddSignalRSwaggerGen(opt =>
					   {
						   opt.UseHubXmlCommentsSummaryAsTag = true;
						   opt.UseHubXmlCommentsSummaryAsTagDescription = true;
						   opt.UseXmlComments(xmlCommentsFullPath);
					   });
					  c.CustomOperationIds(apiDescription =>
						apiDescription.TryGetMethodInfo(out MethodInfo methodInfo)
						? methodInfo.Name
						: null);
				   });

				   services.AddSwaggerGenNewtonsoftSupport();

				   //--------------- Connections -----------------//
				   services.AddSingleton<UnitOfWorkConnections>();
				   services.AddSingleton(typeof(Connections<NotificationHub>));

				   //--------------- SignalR -----------------//
				   services.AddSignalR(configure =>
				   {
					   configure.KeepAliveInterval = TimeSpan.FromMinutes(1);
					   configure.EnableDetailedErrors = true;
					   configure.MaximumReceiveMessageSize = 65_536;
					   configure.HandshakeTimeout = TimeSpan.FromSeconds(15);
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
				   app.UseSwagger();
				   app.UseSwaggerUI(c =>
				   {
					   c.SwaggerEndpoint("/swagger/v1/swagger.json", "WSService");
					   c.DocumentTitle = "WebSockets service";
					   c.DocExpansion(DocExpansion.None);
				   }
				   );
				   app.UseRouting();
				   app.UseCors("AllowAll");
				   app.UseEndpoints(endpoints =>
				   {
					   endpoints.MapHub<NotificationHub>(unitOfWorkConfig.Configuration["HostSettings:RouteNotify"], options =>
					   {
						   options.TransportMaxBufferSize =65_365;
						   options.Transports = HttpTransportType.WebSockets;
						   options.WebSockets.CloseTimeout = TimeSpan.FromSeconds(15);
						   options.CloseOnAuthenticationExpiration = false;
						   options.TransportSendTimeout = TimeSpan.FromSeconds(15);
						   options.ApplicationMaxBufferSize = 131_072;
						   options.TransportMaxBufferSize = 131_072;
					   });
					   endpoints.MapHub<AlarmHub>(unitOfWorkConfig.Configuration["HostSettings:RouteAlarm"], options =>
					   {
						   options.TransportMaxBufferSize = 65_365;
						   options.Transports = HttpTransportType.WebSockets;
						   options.WebSockets.CloseTimeout = TimeSpan.FromSeconds(15);
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




