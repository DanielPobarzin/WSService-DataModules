using Communications.Connections;
using Communications.Helpers;
using Communications.Hubs;
using Communications.UoW;
using Entities.Enums;
using Interactors.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Serilog;
using Shared.Services;
using Shared.Share.KafkaMessage;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Diagnostics;
using System.Reflection;

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
		private static Thread UoWNotifyThread;
		private static Thread UoWAlarmThread;
		private static Thread hostThread;
		private static Thread kafkaProducerThread;

		/// <summary>
		/// The main method definies work threads, creates instances of work units, etc.
		/// </summary>
		/// <remarks>
		/// <see cref="LoggerSetupHelper"/>
		/// <see cref="UnitOfWorkGetConfig"/>
		/// <see cref="UnitOfWorkGetConfig"/>
		/// <see cref="CheckHashHalper"/>
		/// </remarks>
		public static void Main()
		{
			try
			{
				// --- Logger Build --- //
				LoggerSetupHelper.ConfigureLogging();

				// --- Initialize get and check configuration --- //
				unitOfWorkConfig = new UnitOfWorkGetConfig();
				checkHashHalper = new CheckHashHalper();

				// --- Host thread --- //
				hostThread = new Thread(CreateAndRunHostServer);

				// --- Units of Work threads --- //
				UoWNotifyThread = new Thread(StartListenNotifications);
				UoWAlarmThread = new Thread(StartListenAlarms);

				// --- Initialize Producer & Consumer and run consumer thread --- //
				producerService = new ProducerService(unitOfWorkConfig.Configuration);

				Task.Run(async () =>
				{
					await producerService.PutMessageProducerProcessAsync("current-server-config-topic", JsonConvert.SerializeObject
							(unitOfWorkConfig.BracingServerSettings((Microsoft.Extensions.Configuration.IConfigurationRoot)
							unitOfWorkConfig.Configuration), Formatting.Indented), "config");
				});

				consumerService = new ConsumerService(unitOfWorkConfig.Configuration);
				consumerService.StartAsync(CancellationToken.None);

				// --- Kafka Producer thread --- //
				kafkaProducerThread = new Thread(StartKafkaProducer);

				// --- On configuration changed --- //
				GetEventChangeConfiguration();

				// --- Start adding threads --- //
				KafkaMessageMetrics.Instance.WorkStatus = WorkStatus.Active;

				hostThread.Start();
				UoWNotifyThread.Start();
				UoWAlarmThread.Start();
				kafkaProducerThread.Start();
				Task.Delay(Timeout.Infinite).Wait();

			}catch(Exception ex)
			{
				KafkaMessageMetrics.Instance.WorkStatus = WorkStatus.NoNActive;
				Log.Error($"Error with run application. Error: {ex.Message}");
			}
			finally
			{
				hostThread.Join();
				UoWAlarmThread.Join();
				UoWNotifyThread.Join();
				kafkaProducerThread.Join();
				Log.CloseAndFlush();
			}
		}
		private static CancellationTokenSource cancellationTokenNotifySource;
		private static CancellationTokenSource cancellationTokenAlarmSource;
		private static CancellationTokenSource cancellationToken;

		/// <summary>
		/// Starts the Kafka producer to send messages to Kafka.
		/// </summary>
		/// <remarks>
		/// This method creates a new instance of <see cref="CancellationTokenSource"/> and starts an asynchronous task
		/// that sends messages to the "client-metric-topic" with a delay of 100 milliseconds.
		/// The task will continue to run until it is canceled using the cancellation token.
		/// </remarks>
		public static void StartKafkaProducer()
		{
			cancellationToken = new CancellationTokenSource();
			Task.Run(async () =>
			{
				while (!cancellationToken.Token.IsCancellationRequested)
				{
					if (KafkaMessageMetrics.Instance.ServerId != Guid.Empty)
					{
						KafkaMessageMetrics.Instance.WorkingMemoryUsage = Process.GetCurrentProcess().WorkingSet64;
						KafkaMessageMetrics.Instance.WorkingMemoryUsage = Process.GetCurrentProcess().PrivateMemorySize64;
						await producerService.PutMessageProducerProcessAsync("metric-topic",
						JsonConvert.SerializeObject((KafkaMessageMetrics.Instance), Formatting.Indented), "server-metric");
					}
					await Task.Delay(100, cancellationToken.Token);
				}
			});
		}
		static object locker1 = new();
		/// <summary>
		/// Starts listening for notifications from the database.
		/// </summary>
		/// <remarks>
		/// This method uses a lock to ensure thread safety. 
		/// It creates a new instance of <see cref="CancellationTokenSource"/> and 
		/// starts the process of retrieving all notifications using 
		/// <see cref="UnitOfWorkGetNotifications"/>.
		/// </remarks>
		public static void StartListenNotifications()
		{
			lock (locker1)
			{
				cancellationTokenNotifySource = new CancellationTokenSource();
				unitOfWorkNotify = new UnitOfWorkGetNotifications(unitOfWorkConfig.Configuration);
				unitOfWorkNotify.GetAllNotifications(cancellationTokenNotifySource.Token);
			}
		}
		static object locker2 = new();
		/// <summary>
		/// Starts listening for alarms from the database.
		/// </summary>
		/// <remarks>
		/// This method uses a lock to ensure thread safety. 
		/// It creates a new instance of <see cref="CancellationTokenSource"/> and 
		/// starts the process of retrieving all alarms using 
		/// <see cref="UnitOfWorkGetAlarms"/>.
		/// </remarks>
		public static void StartListenAlarms()
		{
			lock (locker2)
			{
				cancellationTokenAlarmSource = new CancellationTokenSource();
				unitOfWorkAlarm = new UnitOfWorkGetAlarms(unitOfWorkConfig.Configuration);
				unitOfWorkAlarm.GetAllAlarms(cancellationTokenAlarmSource.Token);
			}
		}

		/// <summary>
		/// A method that always expects to receive any changes in the configuration file. 
		/// The action is determined depending on which setting has been changed.
		/// </summary>
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

						RebootThread(cancellationToken, kafkaProducerThread, StartKafkaProducer);
					}
					if (hashConfigs.ContainsKey("Kafka:Consumer"))
					{
						Log.Information($"{unitOfWorkConfig.Configuration["HubSettings:ServerId"]}:" +
							$" Changing the settings of connecting to kafka broker(role:Consumer). ");
						await consumerService.StopAsync(CancellationToken.None);
						consumerService = new ConsumerService(unitOfWorkConfig.Configuration);
						await consumerService.StartAsync(CancellationToken.None);
					}

				await producerService.PutMessageProducerProcessAsync("current-server-config-topic", JsonConvert.SerializeObject
							(unitOfWorkConfig.BracingServerSettings((Microsoft.Extensions.Configuration.IConfigurationRoot)
							unitOfWorkConfig.Configuration), Formatting.Indented), "config");
			}
		}
		private static IHost host;
		/// <summary>
		/// Creates and starts a host server with the web application's settings.
		/// </summary>
		/// <remarks>
		/// This method configures the host server, adds necessary services and configurations, including:
		/// <list type="bullet">
		/// <item>Configuring CORS to manage access to the API.</item>
		/// <item>Adding Swagger for API documentation.</item>
		/// <item>Configuring SignalR for working with WebSockets.</item>
		/// <item>Registering various providers and stores for notifications and alarms.</item>
		/// </list>
		/// The method uses settings from the configuration file, such as host URLs and allowed origins.
		/// </remarks>
		public static void CreateAndRunHostServer()    
		{
			KafkaMessageMetrics.Instance.ServerId = Guid.Parse(unitOfWorkConfig.Configuration["HubSettings:ServerId"]);
			host = Host.CreateDefaultBuilder()
		   .ConfigureWebHostDefaults(webBuilder =>
		   {
			   webBuilder.UseUrls(unitOfWorkConfig.Configuration["HostSettings:urls"].Split(";"));
			   webBuilder.ConfigureServices(services =>
			   {
				   services.AddMemoryCache();

				   // ---  Сonfiguration provider --- //
				   services.AddSingleton(provider =>
				   {
					   return unitOfWorkConfig.Configuration;
				   });

				   // --- Notification provider --- //
				   services.AddSingleton(provider =>
				   {
					   return unitOfWorkNotify.ReceivedNotificationsList;
				   });

				   services.AddSingleton(provider =>
				   {
					   return producerService;
				   });
				   // --- Alarm provider --- //
				   services.AddSingleton(provider =>
				   {
					   return unitOfWorkAlarm.ReceivedAlarmsList;
				   });

				   // --- Helpers provider  --- //
				   services.AddScoped<TransformToDTOHelper>();
				   services.AddScoped<JsonCacheHelper>();

				   // --- CORS --- //
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

				   // --- Swagger --- //
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

				   // --- Connections --- //
				   services.AddSingleton(typeof(ConcurrentConnections<AlarmHub>));
				   services.AddSingleton(typeof(ConcurrentConnections<NotificationHub>));


				   // --- SignalR --- //
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
			cancellationToken.Dispose();
			thread.Join();
			if (action == StartKafkaProducer)
			{
				producerService.Dispose();
				producerService = new ProducerService(unitOfWorkConfig.Configuration);
			}
			Thread newThread = new Thread(new ThreadStart(action));
			newThread.Start();
			thread = newThread;
		}
	}
}




