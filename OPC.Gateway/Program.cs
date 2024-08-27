extern alias UaCore;
extern alias UaFx;
using Opc.Ua.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using UaCore::Opc.Ua.Client;

namespace Opc.Ua.Sample
{
	static class Program
	{
		[STAThread]
		static async Task Main()
		{
			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Debug() 
				.MinimumLevel.Override("Microsoft", LogEventLevel.Warning) 
				.MinimumLevel.Override("Opc.Ua", LogEventLevel.Debug) 
				.Enrich.FromLogContext()
				.Enrich.WithExceptionData()
				.Enrich.WithThreadName()
				.Enrich.WithDemystifiedStackTraces()
				.WriteTo.Console(theme: SystemConsoleTheme.Colored, restrictedToMinimumLevel: LogEventLevel.Information)
				.WriteTo.File(AppContext.BaseDirectory + @"\Log\[VERBOSE]_Log_.log",
					rollingInterval: RollingInterval.Day,
					rollOnFileSizeLimit: true,
					retainedFileCountLimit: 365,
					shared: true,
					restrictedToMinimumLevel: LogEventLevel.Verbose)
				.WriteTo.File(AppContext.BaseDirectory + @"\Log\[ERROR]_Log_.log",
					rollingInterval: RollingInterval.Day,
					rollOnFileSizeLimit: true,
					retainedFileCountLimit: 365,
					shared: true,
					restrictedToMinimumLevel: LogEventLevel.Error)
				.WriteTo.File(AppContext.BaseDirectory + @"\Log\[INFO]_Log_.log",
					rollingInterval: RollingInterval.Day,
					rollOnFileSizeLimit: true,
					retainedFileCountLimit: 365,
					shared: true,
					restrictedToMinimumLevel: LogEventLevel.Information)
				.CreateLogger();
			ApplicationInstance application = new ApplicationInstance
			{
				ApplicationName = "UA gateway",
				ApplicationType = ApplicationType.Client,
				ConfigSectionName = "Opc.Ua.Client"
			};

			try
			{
				if (application.ProcessCommandLine())
				{
					return;
				}
				// Load the application configuration.
				await application.LoadApplicationConfiguration(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.xml"), false);

				// Check the application certificate.
				await application.CheckApplicationInstanceCertificate(false, 0);

				// Create a session with the OPC UA server.
				var endpointURL = "opc.tcp://192.168.0.10:4840/";
				var endpoint = CoreClientUtils.SelectEndpoint(endpointURL, false);
				var configuredEndpoint = new ConfiguredEndpoint(
					null, 
					endpoint,
					EndpointConfiguration.Create());
				using (var session = await Session.Create(application.ApplicationConfiguration, configuredEndpoint, false, "", 60000, null, null))
				{
					Log.Information("Connected to OPC UA server at {Endpoint}", endpointURL);
					var subscription = new Subscription(session.DefaultSubscription) { PublishingInterval = 1000 };
					var item = new MonitoredItem
					{
						StartNodeId = "ns=2;s=12345678-1234-1234-1234-1234567890ab",
						AttributeId = Attributes.Value,
						SamplingInterval = 1000,
						QueueSize = 0,
						DiscardOldest = true
					};

					item.Notification += OnDataChanged;
					subscription.AddItem(item);
					session.AddSubscription(subscription);
					subscription.Create();
					Console.ReadKey();
				}
			}
			catch (Exception e)
			{
				Log.Fatal(e, "An error occurred in the application: {Message}", e.Message);
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}

		private static void OnDataChanged(MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs e)
		{
			var notifications = e.NotificationValue as IList<MonitoredItemNotification>;

			if (notifications != null)
			{
				foreach (var notification in notifications)
				{
					Log.Information("Data changed: {Value}", notification.Value);
				}
			}
			else
			{
				Log.Warning("NotificationValue is not a list of MonitoredItemNotification.");
			}
		}
	}
}