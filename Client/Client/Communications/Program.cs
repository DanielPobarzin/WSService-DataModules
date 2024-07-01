using Communications.Helpers;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Client
{
	internal class Program
	{
		private static HubConnection connection;
		private static UnitOfWorkGetConfig unitOfWorkConfig;
		private static CheckHashHalper checkHashHalper;
		private static CancellationTokenSource cancellationTokenSource;
		private static Thread UoWThread;
		private static Thread connectThread;
		private async static Task Main(string[] args)
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

			//--------------- Defining actions when changing the configuration -----------------//
			connection = new HubConnectionBuilder()
									.WithUrl("https://localhost:443/notification", options =>
									{
										options.Transports = HttpTransportType.WebSockets;
										options.CloseTimeout = TimeSpan.FromSeconds(15);
									})
									.WithAutomaticReconnect()
									.Build();
			connection.On<MessageServerDTO>("ReceiveNotification", (message) =>
			{
				RecievedByClientNotification notification = new RecievedByClientNotification
				{
					ClientId = Guid.NewGuid(),
					ServerId = message.ServerId,
					Notification = message.Notification,
					DateAndTimeSendDataByServer = message.DateAndTimeSendDataByServer,
					DateAndTimeRecievedDataFromServer = DateTime.Now
				};
				Log.Information($"The notification {notification.Notification.Id} with message <<{notification.Notification.Content}>> " +
											   $"has been received by client {notification.ClientId} from server {notification.ServerId}. ");
			});
			
			connection.Reconnecting += (exception) =>
			{
				Console.WriteLine("Connection lost. Reconnecting...");
				return Task.CompletedTask;
			};
			await connection.StartAsync();
			try
			{
				await connection.InvokeAsync("Send", Guid.NewGuid());
			}
			catch (Exception ex) { Console.WriteLine($"{ex}"); }

			//GetEventChangeConfiguration();
			Task.Delay(Timeout.Infinite).Wait();
			Log.CloseAndFlush();

		}
		private static async Task StartConnectionToServer()
		{
		
		}

	}
}





