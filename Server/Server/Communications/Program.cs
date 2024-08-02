using Communications.Connections;
using Communications.Helpers;
using Communications.Hubs;
using Communications.UoW;
using Interactors.Helpers;
using Interactors.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Shared.Services;
using Shared.Share.KafkaMessage;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;
using System.Text.Json;
using System.Threading;

namespace Communications
{
	public class Program
	{
		private static ProducerService producerService;
		private static UnitOfWorkGetConfig unitOfWorkConfig;
		private static UnitOfWorkGetNotifications unitOfWorkNotify;
		private static UnitOfWorkGetAlarms unitOfWorkAlarm;
		private static CheckHashHalper checkHashHalper;
		private static ConsumerService consumerService;
		private static CancellationTokenSource cancellationTokenNotifySource;
		private static CancellationTokenSource cancellationTokenAlarmSource;
		private static CancellationTokenSource cancellationToken;
		private static Thread UoWNotifyThread;
		private static Thread UoWAlarmThread;
		private static Thread hostThread;
		private static Thread kafkaConsumerThread;
		private static readonly JsonSerializerOptions DefaultOptions = new JsonSerializerOptions
		{
			WriteIndented = true
		};

		public static IHost host;

		static object locker1 = new();
		static object locker2 = new();
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
			

			//--------------- Start requesting data from the database -----------------//
			UoWNotifyThread = new Thread(StartListenNotifications);
			

			//--------------- Start requesting data from the database -----------------//
			UoWAlarmThread = new Thread(StartListenAlarms);
			
			GetEventChangeConfiguration();

			{
				producerService = new ProducerService(unitOfWorkConfig.Configuration);

				Task.Run(async () => await producerService.PutMessageProducerProcessAsync("current-client-config-topic",
						 JsonSerializer.Serialize(SerializeHelper.BuildConfigDictionary(unitOfWorkConfig.Configuration), DefaultOptions), "config"));
				Task.Run(async () =>
				{
					while (!cancellationToken.Token.IsCancellationRequested)
					{
						await producerService.PutMessageProducerProcessAsync("client-metric-topic",
							JsonSerializer.Serialize((KafkaMessageMetrics.Instance), DefaultOptions), "metric");
						await Task.Delay(100, cancellationToken.Token);
					}
				});
			}

			hostThread.Start();
			UoWNotifyThread.Start();
			UoWAlarmThread.Start();
			kafkaConsumerThread.Start();

			Task.Delay(Timeout.Infinite).Wait();
			Log.CloseAndFlush();
		}
		public static void StartKafka()
		{
			consumerService = new ConsumerService(unitOfWorkConfig.Configuration);
			consumerService.StartAsync(CancellationToken.None);
		}
		public static void StartListenNotifications()
		{
			lock (locker1)
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
			lock (locker2)
			{
				cancellationTokenAlarmSource = new CancellationTokenSource();
				CancellationToken cancellationToken = cancellationTokenAlarmSource.Token;

				//--------------- Determining the connection and starting to receive data -----------------//
				unitOfWorkAlarm = new UnitOfWorkGetAlarms(unitOfWorkConfig.Configuration);
				unitOfWorkAlarm.GetAllAlarms(cancellationToken);
			}
		}
		public static async void GetEventChangeConfiguration()
		{
			await foreach (var hashConfigs in checkHashHalper.CompareHashConfiguration(unitOfWorkConfig.sectionHashes))
			{
					if (hashConfigs.ContainsKey("HostSettings"))
					{
						Log.Information("Changing the host configuration. Reboot ... ");
						await host.StopAsync();
						hostThread.Join();
						Thread newHostThread = new Thread(CreateAndRunHostServer);
						newHostThread.Start();
						hostThread = newHostThread;
					}

					if (hashConfigs.ContainsKey("HubSettings:ServerId"))
					{
						Log.Information($"Changing the serverId. Continue with the new configuration ... ");
						RebootThread(cancellationTokenAlarmSource, UoWAlarmThread, StartListenAlarms);
						RebootThread(cancellationTokenNotifySource, UoWNotifyThread, StartListenNotifications);
					}
					else if (hashConfigs.ContainsKey("DbConnection:DataBase") ||
							hashConfigs.ContainsKey("DbConnection:Alarm") ||
							hashConfigs.ContainsKey("HubSettings:Alarm"))
					{
						var comment = (hashConfigs.ContainsKey("DbConnection:DataBase") ||
								   hashConfigs.ContainsKey("DbConnection:Alarm")) ?
								   ((hashConfigs.ContainsKey("HubSettings:Alarm")) ?
								   "database & AlarmHub" : "database")
								   : "AlarmHub";
						Log.Information($"Changing the configuration {comment}. Continue with the new configuration ... ");
						RebootThread(cancellationTokenAlarmSource, UoWAlarmThread, StartListenAlarms);
					}
					else if (hashConfigs.ContainsKey("DbConnection:DataBase") ||
							hashConfigs.ContainsKey("DbConnection:Notify") ||
							hashConfigs.ContainsKey("HubSettings:Notify"))
					{
						var comment = (hashConfigs.ContainsKey("DbConnection:DataBase") ||
								   hashConfigs.ContainsKey("DbConnection:Notify")) ?
								   ((hashConfigs.ContainsKey("HubSettings:Notify")) ?
								   "database & NotifyHub" : "database")
								   : "NotifyHub";
						Log.Information($"Changing the configuration {comment}. Continue with the new configuration ... ");
						RebootThread(cancellationTokenNotifySource, UoWNotifyThread, StartListenNotifications);
					}
					if (hashConfigs.ContainsKey("Kafka:Producer"))
					{
						Log.Information($"{unitOfWorkConfig.Configuration["HubSettings:ServerId"]}: " +
							$"Changing the settings of connecting to kafka broker (role:Producer). ");
						producerService.Dispose();
						producerService = new ProducerService(unitOfWorkConfig.Configuration);
					}
					if (hashConfigs.ContainsKey("Kafka:Consumer"))
					{
						Log.Information($"{unitOfWorkConfig.Configuration["HubSettings:ServerId"]}:" +
							$" Changing the settings of connecting to kafka broker(role:Consumer). ");
						await consumerService.StopAsync(CancellationToken.None);
						kafkaConsumerThread.Join();
						Thread newThread = new Thread(StartKafka);
						newThread.Start();
						kafkaConsumerThread = newThread;
					}

				await producerService.PutMessageProducerProcessAsync("current-client-config-topic", JsonSerializer.Serialize
						(SerializeHelper.BuildConfigDictionary(unitOfWorkConfig.Configuration), DefaultOptions), "config");
			}
		}
		
		//------------- Hosting --------------------------------------------------//
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
				   //--------------- Alarm provider  -----------------//
				   services.AddSingleton(provider =>
				   {
					   return unitOfWorkAlarm.ReceivedAlarmsList;
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
				   services.AddSingleton(typeof(Connections<AlarmHub>));
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
		private static void RebootThread(CancellationTokenSource cancellationToken, Thread thread, Action action)
		{
			cancellationToken.Cancel();
			thread.Join();
			Thread newThread = new Thread(new ThreadStart(action));
			newThread.Start();
			thread = newThread;
		}
	}
}




