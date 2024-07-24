using Communications.Common.Helpers;
using Communications.DTO;
using Communications.Helpers;
using Communications.UoW;
using Interactors.Enums;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Caching.Memory;
using Repositories.DO;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System.Net;

namespace Client
{
	public class Program
	{
		private static HubConnection connectionNotify;
		private static HubConnection connectionAlarm;

		private static Guid clientId;

		private static UnitOfWorkGetConfig unitOfWorkConfig;
		private static UnitOfWorkPublishNotifications unitOfWorkPublishNotifications;
		private static UnitOfWorkPublishAlarms unitOfWorkPublishAlarms;

		private static CheckHashHalper checkHashHalper;

		private static Thread notificationExchangeThread;
		private static Thread alarmExchangeThread;

		private static IMemoryCache memoryCache;
		private static TransformToDOHelper TransformToDOHelper;
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
			//--------------- Initialize get and check configuration -----------------//
			unitOfWorkConfig = new UnitOfWorkGetConfig();
			checkHashHalper = new CheckHashHalper();

			//--------------- Initialize publish method notification -----------------//
			unitOfWorkPublishNotifications = new UnitOfWorkPublishNotifications(unitOfWorkConfig.Configuration);

			//--------------- Initialize publish method alarm -----------------//
			unitOfWorkPublishAlarms = new UnitOfWorkPublishAlarms(unitOfWorkConfig.Configuration);

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


		public static void ExchangeBetweenServerAndClient(object? Hub)
		{
			if (Hub is TypeHub type)
			{
				clientId = Guid.Parse(unitOfWorkConfig.Configuration["ClientSettings:ClientId"]);
				switch (type)
				{
					//--------------- Notify Hub -----------------//
					case TypeHub.Notify:
						string url = unitOfWorkConfig.Configuration["ConnectionSettings:NotifyUrl"];
						ConnectionСonfiguration(url, connectionNotify); //---- Setting the connection configuration ------// 
						StartHubConnectionAsync(connectionNotify); //---- Start Hub Connection ----//
						GetHubMessages("Notification", connectionNotify); //--- Try get messages ----//
						EventWithConnectionHandler(connectionNotify); //--- Connection Event Triggers -----//
						break;

					//--------------- Alarm Hub -----------------//
					case TypeHub.Alarm:
						url = unitOfWorkConfig.Configuration["ConnectionSettings:AlarmUrl"];
						ConnectionСonfiguration(url, connectionAlarm); //---- Setting the connection configuration ------// 
						StartHubConnectionAsync(connectionAlarm); //---- Start Hub Connection ----//
						GetHubMessages("Notification", connectionAlarm); //--- Try get messages ----//
						EventWithConnectionHandler(connectionAlarm); //--- Connection Event Triggers -----//
					break;
				}
			}
		}
		public static void ConnectionСonfiguration(string Url , HubConnection connection)
		{	
			connection = new HubConnectionBuilder()
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

		public static async void StartHubConnectionAsync(HubConnection connection)
		{
			Log.Information($"Attempt to connect to the server by url: {unitOfWorkConfig.Configuration["ConnectionSettings:Url"]}");
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
		public static async void GetHubMessages(string MessageType, HubConnection connection)
		{
			switch (MessageType)
			{ 
				case "Notification":
					connection.On<MessageServerDTO>("ReceiveMessageHandler", async (message) =>
					{
						var messageNotify = await TransformToDOHelper.TransformToDomainObjectNotification(message, clientId);
						Log.Information($"{MessageType} {messageNotify.Notification.Id} has been recieved."
											+ "\nSender:   " + $" Server - {messageNotify.SenderId}"
											+ "\nRecipient:" + $" Client - {clientId}");

						memoryCache.TryGetValue($"{clientId}_{messageNotify.Notification.Id}", out DomainObjectNotification? cachedNotification);

						if (cachedNotification == null)
						{
							memoryCache.Set($"{clientId}_{messageNotify.Notification.Id}", messageNotify);
							try
							{
								unitOfWorkPublishNotifications.PublishNotifications(messageNotify).Wait();
								unitOfWorkPublishNotifications.Save();
							}
							catch (Exception ex) { Log.Error($"Еrror working with the database : {ex.Message}"); }

						}
					});
					break;
				case "Alarm":
					connection.On<AlarmServerDTO>("ReceiveMessageHandler", async (message) =>
					{
						var messageAlarm = await TransformToDOHelper.TransformToDomainObjectAlarm(message, clientId);
						Log.Information($"{MessageType} {messageAlarm.Alarm.Id} has been recieved."
											+ "\nSender:   " + $" Server - {messageAlarm.SenderId}"
											+ "\nRecipient:" + $" Client - {clientId}");

						memoryCache.TryGetValue($"{clientId}_{messageAlarm.Alarm.Id}", out DomainObjectAlarm? cachedAlarm);

						if (cachedAlarm == null)
						{
							memoryCache.Set($"{clientId}_{messageAlarm.Alarm.Id}", messageAlarm);
							try
							{
								unitOfWorkPublishAlarms.PublishAlarms(messageAlarm).Wait();
								unitOfWorkPublishAlarms.Save();
							}
							catch (Exception ex) { Log.Error($"Еrror working with the database : {ex.Message}"); }
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
					await ReconnectAsync(TypeHub.Notify, notificationExchangeThread);
					await ReconnectAsync(TypeHub.Alarm, alarmExchangeThread);
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
					await ReconnectAsync(TypeHub.Notify, notificationExchangeThread);
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
					await ReconnectAsync(TypeHub.Alarm, alarmExchangeThread);
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
		public static async Task ReconnectAsync(TypeHub hub, Thread thread)
		{
			thread.Join();
			Thread newThread = new Thread(ExchangeBetweenServerAndClient);
			newThread.Start(hub);
			thread = newThread;
		}
	}
}