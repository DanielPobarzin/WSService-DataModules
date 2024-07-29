using Communications.Common.Helpers;
using Communications.DTO;
using Communications.Helpers;
using Communications.UoW;
using Interactors.Enums;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Repositories;
using Repositories.DO;
using Repositories.Notifications;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Shared.Services;
using System.Net;

namespace Client
{
	public class Program
	{
		private static HubConnection connectionNotify { get; set; }
		private static HubConnection connectionAlarm { get; set; }

		private static Guid clientId;

		private static UnitOfWorkGetConfig unitOfWorkConfig;

		private static CheckHashHalper checkHashHalper;

		private static Thread notificationExchangeThread;
		private static Thread alarmExchangeThread;

		private static MemoryCache MemoryCache;
		
		private static void Main(string[] args)
		{
			//---------- Logging ---------------//
			{
				Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Override("Microsoft", LogEventLevel.Verbose)
				.Enrich.FromLogContext()
				.Enrich.WithExceptionData()
				.Enrich.WithThreadName()
				.Enrich.WithDemystifiedStackTraces()
				.WriteTo.Console(theme: SystemConsoleTheme.Colored, restrictedToMinimumLevel: LogEventLevel.Information)
				.WriteTo.Console(theme: SystemConsoleTheme.Colored, restrictedToMinimumLevel: LogEventLevel.Error)
				.WriteTo.File(AppContext.BaseDirectory + @"\Log\[VERBOSE]_Client_Log_.log",
						rollingInterval: RollingInterval.Day,
						rollOnFileSizeLimit: true,
						retainedFileCountLimit: 365,
						shared: true,
						restrictedToMinimumLevel: LogEventLevel.Verbose)
				.WriteTo.File(AppContext.BaseDirectory + @"\Log\[ERROR]_Client_Log_.log",
						rollingInterval: RollingInterval.Day,
						rollOnFileSizeLimit: true,
						retainedFileCountLimit: 365,
						shared: true,
						restrictedToMinimumLevel: LogEventLevel.Error)
				.WriteTo.File(AppContext.BaseDirectory + @"\Log\[INFO]_Client_Log_.log",
						rollingInterval: RollingInterval.Day,
						rollOnFileSizeLimit: true,
						retainedFileCountLimit: 365,
						shared: true,
						restrictedToMinimumLevel: LogEventLevel.Information)
				.CreateLogger();
			}
			//---------- CacheInMemory ---------------//
			MemoryCache = new MemoryCache(new MemoryCacheOptions
			{
				ExpirationScanFrequency = TimeSpan.FromMinutes(60),	SizeLimit = 1024 * 1024 * 100, CompactionPercentage = 0.4
			});

			//--------------- Initialize get and check configuration -----------------//
			unitOfWorkConfig = new UnitOfWorkGetConfig();
			checkHashHalper = new CheckHashHalper();	

			//--------------- Мain threads of the client's work -----------------//
			notificationExchangeThread = new Thread(ExchangeBetweenServerAndClient);
			alarmExchangeThread = new Thread(ExchangeBetweenServerAndClient);

			notificationExchangeThread.Start(TypeHub.Notify);
			alarmExchangeThread.Start(TypeHub.Alarm);

			//--------------- Defining actions when changing the configuration -----------------//
			GetEventChangeConfiguration();

			Task.Delay(Timeout.Infinite).Wait();
			Log.CloseAndFlush();
		}


		public static async void ExchangeBetweenServerAndClient(object? Hub)
		{
			if (Hub is TypeHub type)
			{
				clientId = Guid.Parse(unitOfWorkConfig.Configuration["ClientSettings:ClientId"]);
				string url = "https://...";
				switch (type)
				{
					//--------------- Notify Hub -----------------//
					case TypeHub.Notify:
						url = unitOfWorkConfig.Configuration["ConnectionSettings:NotifyUrl"];
						connectionNotify = ConnectionСonfiguration(url); //---- Setting the connection configuration ------// 
						await StartHubConnectionAsync(url, connectionNotify); //---- Start Hub Connection ----//
						EventWithConnectionHandler(connectionNotify); //--- Connection Event Triggers -----//
						GetHubMessages(type, connectionNotify); //--- Try get messages ----//
						break;

					//--------------- Alarm Hub -----------------//
					case TypeHub.Alarm:
						url = unitOfWorkConfig.Configuration["ConnectionSettings:AlarmUrl"];
						connectionAlarm = ConnectionСonfiguration(url); //---- Setting the connection configuration ------// 
						await StartHubConnectionAsync(url, connectionAlarm); //---- Start Hub Connection ----//
						EventWithConnectionHandler(connectionAlarm); //--- Connection Event Triggers -----//
						GetHubMessages(type, connectionAlarm); //--- Try get messages ----//
					break;
				}
			}
		}
		public static HubConnection ConnectionСonfiguration(string Url)
		{
			var connection = new HubConnectionBuilder()
									.WithUrl(Url, options =>
									{
										options.AccessTokenProvider = null; // Required!
										options.SkipNegotiation = false;
										options.Cookies = new CookieContainer();
										options.CloseTimeout = TimeSpan.FromSeconds(15);
										options.Headers["Notification"] = "Content";
										options.ClientCertificates = new System.Security.Cryptography.X509Certificates.X509CertificateCollection();
										options.DefaultTransferFormat = TransferFormat.Text;
										options.Credentials = null; // Required!
										options.UseDefaultCredentials = true;
									})
									.WithAutomaticReconnect()
									.Build();
			
			connection.ServerTimeout = TimeSpan.FromMinutes(2);
			return connection;
		}
		public static void GetKafkaConfigs()
		{

		}
		public static void GetKafkaCommands()
		{

		}
		public static void PushKafkaConnectionStatus()
		{

		}
		public static void PushKafkaMetrics()
		{

		}

		public static void EventWithConnectionHandler(HubConnection connection)
		{
			connection.Closed += (error) =>
			{
				var messageError = (error != null) ? $"An exception has occurred :{error.Message}" : "";
				Log.Information($"Connection closed." + $"{messageError}");
				return Task.CompletedTask;
			};
			connection.Reconnecting += (error) =>
			{
				Log.Information($"Connection lost : {error.Message}. Reconnecting...");
				return Task.CompletedTask;
			};

			connection.Reconnected += async (connectionId) =>
			{
				Log.Information($"Reconnected. New connection id: {connectionId}");
				try
				{
					await connection.InvokeAsync("OnReconnectedAsync", clientId);
				}
				catch (Exception ex) { Log.Error($"An error occurred when calling the method on the server : {ex.Message}");}
			};
		}

		public static async Task StartHubConnectionAsync(string url, HubConnection connection)
		{
			Log.Information($"Attempt to connect to the server by url: {url}");
				try
				{
					await connection.StartAsync().ContinueWith(async task =>
					{
						if (task.IsCompletedSuccessfully)
						{
							Log.Information($"Connection id: {connection.ConnectionId}");
							await connection.InvokeAsync("Send", clientId);
						}
					});
				}
				catch (Exception ex) { Log.Error($"Connection error: {ex.Message}"); }
		}
		public static void GetHubMessages(TypeHub type, HubConnection connection)
		{
			TransformToDOHelper TransformToDOHelper = new();
			switch (type)
			{ 
				case TypeHub.Notify:
					connection.On<MessageServerDTO>("ReceiveMessageHandler", async (message) =>
					{
						Log.Information($"Notification {message.Notification.Id} has been recieved."
											+ "\nSender:   " + $" Server - {message.ServerId}"
											+ "\nRecipient:" + $" Client - {clientId}");
				
						var messageNotify = await TransformToDOHelper.TransformToDomainObjectNotification(message, clientId);

						MemoryCache.TryGetValue($"{clientId}_{messageNotify.Notification.Id}", out DomainObjectNotification? cachedNotification);

						if (cachedNotification == null)
						{
							try
							{
								using (UnitOfWorkPublishNotifications unitOfWorkPublishNotifications = new UnitOfWorkPublishNotifications(unitOfWorkConfig.Configuration))
								{
									unitOfWorkPublishNotifications.PublishNotifications(messageNotify).Wait();
								}
							}
							catch (Exception ex) { Log.Error($"Еrror working with the database: {ex.Message}"); }
							MemoryCache.Set($"{clientId}_{messageNotify.Notification.Id}", messageNotify);
						}
					});
					break;
				case TypeHub.Alarm:
					connection.On<AlarmServerDTO>("ReceiveMessageHandler", async (message) =>
					{
						Log.Information($"Alarm {message.Alarm.Id} has been recieved."
											+ "\nSender:   " + $" Server - {message.ServerId}"
											+ "\nRecipient:" + $" Client - {clientId}");

						var messageAlarm = await TransformToDOHelper.TransformToDomainObjectAlarm(message, clientId);
						MemoryCache.TryGetValue($"{clientId}_{messageAlarm.Alarm.Id}", out DomainObjectAlarm? cachedAlarm);

						if (cachedAlarm == null)
						{
							try
							{
								using (UnitOfWorkPublishAlarms unitOfWorkPublishAlarms = new UnitOfWorkPublishAlarms(unitOfWorkConfig.Configuration))
								{
									unitOfWorkPublishAlarms.PublishAlarms(messageAlarm).Wait();
								}
							}
							catch (Exception ex) { Log.Error($"Еrror working with the database : {ex.Message}"); }
							MemoryCache.Set($"{clientId}_{messageAlarm.Alarm.Id}", messageAlarm);
						}
					});
					break;
			}
			connection.On<string>("Notify", (message) =>
			{
				Log.Information($"The message from server: {message}");
			});
		}
		public static async void GetEventChangeConfiguration()
		{
			await foreach (var hashConfigs in checkHashHalper.CompareHashConfiguration(unitOfWorkConfig.sectionHashes))
			{
				if (hashConfigs.ContainsKey("ClientSettings:ClientId"))
				{
					await AllDisconnectAsync();
					ReconnectAsync(TypeHub.Notify, notificationExchangeThread);
					ReconnectAsync(TypeHub.Alarm, alarmExchangeThread);
					Log.Information($"Changing the ClientId. Reconnect with the new configuration ... ");
				}
				if (hashConfigs.ContainsKey("DbConnection:DataBase") ||
							hashConfigs.ContainsKey("DbConnection:NotifyConnectionString") ||
							hashConfigs.ContainsKey("ConnectionSettings:NotifyUrl"))
				{
					var comment = (hashConfigs.ContainsKey("DbConnection:DataBase") ||
								   hashConfigs.ContainsKey("DbConnection:NotifyConnectionString")) ?
								   ((hashConfigs.ContainsKey("ConnectionSettings:NotifyUrl")) ?
								   "database & connection to NotifyHub" : "database")
								   : "NotifyHub";
					Log.Information($"Changing the configuration {comment}. Reconnecting ... ");

					await connectionNotify.StopAsync();
					await connectionNotify.DisposeAsync();
					ReconnectAsync(TypeHub.Notify, notificationExchangeThread);
				}
				
				else if (hashConfigs.ContainsKey("DbConnection:DataBase") ||
							hashConfigs.ContainsKey("DbConnection:AlarmConnectionString") ||
							hashConfigs.ContainsKey("ConnectionSettings:AlarmUrl"))
				{
					var comment = (hashConfigs.ContainsKey("DbConnection:DataBase") ||
								   hashConfigs.ContainsKey("DbConnection:AlarmConnectionString")) ?
								   ((hashConfigs.ContainsKey("ConnectionSettings:AlarmUrl")) ?
								   "database & connection to AlarmHub" : "database")
								   : "AlarmHub";
					Log.Information($"Changing the configuration {comment}. Reconnecting ... ");

					await connectionAlarm.StopAsync();
					await connectionAlarm.DisposeAsync();
					ReconnectAsync(TypeHub.Alarm, alarmExchangeThread);
				}
			}
		}
		public static async Task AllDisconnectAsync()
		{
			if (connectionNotify != null && connectionNotify.State == HubConnectionState.Connected)
			{
				await connectionNotify.StopAsync();
				await connectionNotify.DisposeAsync();
			}
			if (connectionAlarm != null && connectionAlarm.State == HubConnectionState.Connected)
			{
				await connectionAlarm.StopAsync();
				await connectionAlarm.DisposeAsync();
			}
		}
		public static void ReconnectAsync(TypeHub hub, Thread thread)
		{
			thread.Join();
			Thread newThread = new Thread(ExchangeBetweenServerAndClient);
			newThread.Start(hub);
			thread = newThread;
		}
	}
}