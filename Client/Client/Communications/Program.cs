using Communications.UoW;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Communications.DTO;
using Entities.Entities;
using Microsoft.Extensions.Options;
using Repositories.DO;
using System.Net;
using Microsoft.AspNetCore.Connections;
using Communications.Common.Helpers;
using Communications.Common.Handlers;
using Newtonsoft.Json;

namespace Client
{
    internal class Program
	{
		private static HubConnection connection;
		private static UnitOfWorkGetConfig unitOfWorkConfig;
		private static UnitOfWorkPublishNotifications unitOfWorkPublishNotifications;
		private static CheckHashHalper checkHashHalper;
		private static CancellationTokenSource cancellationTokenSource;
		private static Thread UoWThread;
		private static Thread connectThread;
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
			unitOfWorkPublishNotifications = new UnitOfWorkPublishNotifications(unitOfWorkConfig.Configuration);
			checkHashHalper = new CheckHashHalper();

			//--------------- Buid connection with configuration -----------------//
			ConnectionСonfigurationAndStart();


			try
			{
				connection.InvokeAsync("Send", Guid.Parse(unitOfWorkConfig.Configuration["ClientSettings:ClientId"]));
			}
			catch (Exception ex) 
			{
				Log.Error($"An error occurred when calling the method on the server: {ex.Message}");
			}
			connection.On<MessageServerDTO>("ReceiveNotification", (message) =>
			{
				DomainObjectNotification notification = new DomainObjectNotification
				{
					ClientId = Guid.Parse(unitOfWorkConfig.Configuration["ClientSettings:ClientId"]),
					ServerId = message.ServerId,
					Notification = message.Notification,
					DateAndTimeSendDataByServer = message.DateAndTimeSendDataByServer,
					DateAndTimeRecievedDataFromServer = DateTime.Now
				};
				Log.Information($"The notification {notification.Notification.Id} with message <<{notification.Notification.Content}>> " +
												   $"has been received by client {notification.ClientId} from server {notification.ServerId}. ");


				unitOfWorkPublishNotifications.PublishNotifications(notification).Wait();
			});

			connection.Closed += async (error) =>
			{
				Log.Information($"Connection closed. Message: {error}");

				await Task.Delay(new Random().Next(0, 5) * 1000);
				Log.Information($"Starting a new connection.");
				ConnectionСonfigurationAndStart();
			};
			connection.Reconnecting += (error) =>
			{
				Log.Information($"Connection lost : {error.Message}. Reconnecting...");
				return Task.CompletedTask;
			};

			connection.Reconnected += connectionId =>
			{
				connection.InvokeAsync("Send", unitOfWorkConfig.Configuration["ClientSettings:ClientId"]).Wait();
				Log.Information($"Reconnected. New connection id: {connectionId}");
				return Task.CompletedTask;
			};
			//--------------- Defining actions when changing the configuration -----------------//

			GetEventChangeConfiguration();

			Task.Delay(Timeout.Infinite).Wait();
			Log.CloseAndFlush();

		}

		public static void ConnectionСonfigurationAndStart()
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
			connection.StartAsync().Wait();
			//ContinueWith(Task =>
			//{
			//	Log.Information($"Try connect to server with url: {connection.State}" + $"\nConnection id: {connection.ConnectionId}");

			//	if (!Task.IsFaulted)
			//	{
			//		Log.Error($"There was an error opening the connection:{Task.Exception.Message}");
			//	}
			//	else
			//	{
			//		Log.Information($"Connected: {connection.State}" + $"\nConnection id: {connection.ConnectionId}");
			//	}
			//})
		}

		public static async void GetEventChangeConfiguration()
		{
			await foreach (var hashConfigs in checkHashHalper.CompareHashConfiguration(unitOfWorkConfig.sectionHashes))
			{
				if (hashConfigs.ContainsKey("ConnectionSettings"))
				{
					Log.Information("Changing the host configuration. Reconnecting ... ");
					await connection.StopAsync();
				}

				if (hashConfigs.ContainsKey("DbConnection") ||
					hashConfigs.ContainsKey("NotificationsHubSettings"))
				{
					var comment = (hashConfigs.ContainsKey("DbConnection")) ?
						((hashConfigs.ContainsKey("NotificationsHubSettings")) ?
						"database & notify hub" : "database")
						: "Notify Hub";

					Log.Information($"Changing the configuration {comment}. Continue with the new configuration ... ");
				}
			}
		}
	}
}

