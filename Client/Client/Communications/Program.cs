using Communications.Common.Helpers;
using Communications.DTO;
using Communications.UoW;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.SignalR.Client;
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
		private static UnitOfWorkGetConfig unitOfWorkConfig;
		private static UnitOfWorkPublishNotifications unitOfWorkPublishNotifications;
		private static CheckHashHalper checkHashHalper;
		private static Thread notificationExchangeThread;
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
										options.CloseTimeout = TimeSpan.Parse(unitOfWorkConfig.Configuration["ConnectionSettings:CloseTimeout"]);
										options.Headers["Notification"] = "Content";
										options.ClientCertificates = new System.Security.Cryptography.X509Certificates.X509CertificateCollection();
										options.DefaultTransferFormat = TransferFormat.Text;
										options.Credentials = null; // Required!
										options.UseDefaultCredentials = true;
									})
									.WithAutomaticReconnect()
									.Build();
			connection.ServerTimeout = TimeSpan.Parse(unitOfWorkConfig.Configuration["ConnectionSettings:ServerTimeout"]);
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
					await connection.InvokeAsync("OnReconnectedAsync", Guid.Parse(unitOfWorkConfig.Configuration["ClientSettings:ClientId"]));
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
							await connection.InvokeAsync("Send", Guid.Parse(unitOfWorkConfig.Configuration["ClientSettings:ClientId"]));
						}
					});
				}
				catch (Exception ex) { Log.Error($"Connection error: {ex.Message}"); }
		}
		public static void GetHubMessages()
		{
			connection.On<MessageServerDTO>("ReceiveMessageHandler", (message) =>
			{
				DomainObjectNotification notification = new DomainObjectNotification
				{
					ClientId = Guid.Parse(unitOfWorkConfig.Configuration["ClientSettings:ClientId"]),
					ServerId = message.ServerId,
					MessageId = message.Notification.Id,
					Notification = message.Notification,
					DateAndTimeSendDataByServer = message.DateAndTimeSendDataByServer,
					DateAndTimeRecievedDataFromServer = DateTime.Now
				};
				Log.Information($"Notification {notification.MessageId} has been recieved."
											+ "\nSender:\t\t" + $" Server - {notification.ServerId}"
											+ "\nRecipient:\t" + $" Client - {notification.ClientId}");
				//try
				//{
				//	unitOfWorkPublishNotifications.PublishNotifications(notification).Wait();
				//	unitOfWorkPublishNotifications.Save();
				//}
				//catch (Exception ex) { Log.Error($"Еrror working with the database : {ex.Message}"); }
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