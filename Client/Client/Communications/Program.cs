using Communications.Common.Helpers;
using Communications.DTO;
using Communications.Helpers;
using Communications.UoW;
using Entities.Entities;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Repositories.DO;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System.Net;

namespace Client
{
	internal class Program
	{
		private static HubConnection connection;
		private static Guid clientId;
		private static UnitOfWorkGetConfig unitOfWorkConfig;
		private static UnitOfWorkPublishNotifications unitOfWorkPublishNotifications;
		private static CheckHashHalper checkHashHalper;
		private static Thread notificationExchangeThread;
		private static IMemoryCache memoryCache;
		private static JsonCacheHelper jsonCacheHelper;
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

			//--------------- Мain thread of the client's work -----------------//
			notificationExchangeThread = new Thread(ExchangeBetweenServerAndClient);
			notificationExchangeThread.Start();
			
			//--------------- Defining actions when changing the configuration -----------------//
			GetEventChangeConfiguration();

			Task.Delay(Timeout.Infinite).Wait();
			Log.CloseAndFlush();
		}


		public static void ExchangeBetweenServerAndClient()
		{
			clientId = Guid.Parse(unitOfWorkConfig.Configuration["ClientSettings:ClientId"]);
			ConnectionСonfiguration(); //---- Setting the connection configuration ------//
			StartHubConnectionAsync(); //---- Start Hub Connection ----//
			GetHubMessages(); //--- Try get messages ----//
			EventWithConnectionHandler(); //--- Connection Event Triggers -----//
		}
		public static void ConnectionСonfiguration()
		{
			connection = new HubConnectionBuilder()
									.WithUrl(unitOfWorkConfig.Configuration["ConnectionSettings:Url"], options =>
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

		public static void EventWithConnectionHandler()
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

		public static async void StartHubConnectionAsync()
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
		public static async void GetHubMessages()
		{
			var cacheNotification = await jsonCacheHelper.ReadFromFileCache<Notification>(clientId);
			if (cacheNotification.Any())
			{
				foreach (var notification in cacheNotification)
					memoryCache.Set($"{clientId}_{notification.Id}", notification);
			}

			connection.On<MessageServerDTO>("ReceiveMessageHandler", async (message) =>
			{
				var messageNotify = await TransformToDOHelper.TransformToDomainObject(message, clientId);
				Log.Information($"Notification {messageNotify.Notification.Id} has been recieved."
									+ "\nSender:\t" + $" Server - {messageNotify.ServerId}"
									+ "\nRecipient:\t" + $" Client - {clientId}");

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
			connection.On<string>("Notify", (message) =>
			{
				Log.Information($"The message from server: {message}");
			});
		}

		public static async void GetEventChangeConfiguration()
		{
			await foreach (var hashConfigs in checkHashHalper.CompareHashConfiguration(unitOfWorkConfig.sectionHashes))
			{
				if (hashConfigs.Count > 0)
				{
					await connection.StopAsync();
					notificationExchangeThread.Join();
					Thread newWork = new Thread(ExchangeBetweenServerAndClient);
					newWork.Start();
					notificationExchangeThread = newWork;
				}

				if (hashConfigs.ContainsKey("ConnectionSettings"))	Log.Information("Changing the configuration. Reconnecting ... ");

				if (hashConfigs.ContainsKey("DbConnection") ||	hashConfigs.ContainsKey("ClientSettings"))
				{
					var comment = (hashConfigs.ContainsKey("DbConnection")) ?
						((hashConfigs.ContainsKey("ClientSettings")) ?
						"database & client" : "database") : "client";

					Log.Information($"Changing the configuration {comment}. Reconnecting ... ");
				}

			}
		}
	}
}