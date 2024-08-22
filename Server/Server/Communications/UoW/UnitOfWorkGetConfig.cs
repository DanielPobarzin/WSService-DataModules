using Entities.Settings;
using Interactors.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Serilog;

namespace Communications.UoW
{
	/// <summary>
	/// Represents a unit of work for managing application configuration settings.
	/// </summary>
	public class UnitOfWorkGetConfig
	{
		/// <summary>
		/// Gets the application configuration.
		/// </summary>
		public IConfiguration Configuration;

		/// <summary>
		/// A dictionary that stores hashes of configuration sections.
		/// </summary>
		public Dictionary<string, string> sectionHashes;

		private IConfigurationRoot configuration;
		private string filePath;
		private string schemaPath;
		private bool isInitialized = false;

		/// <summary>
		/// Initializes a new instance of the <see cref="UnitOfWorkGetConfig"/> class.
		/// Loads the configuration from the specified JSON file and sets up change tokens for reloading.
		/// </summary>
		public UnitOfWorkGetConfig()
		{
			sectionHashes = new Dictionary<string, string>();
			EnsureConfigurationFile("configure.json");
			schemaPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "schema.json");

			LoadConfigFile();
			ChangeToken.OnChange(configuration.GetReloadToken, () =>
			{
				if (isInitialized)
				{
					LoadConfigFile();
				}
				else
				{
					isInitialized = true;
				}
			});
		}

		/// <summary>
		/// Loads the configuration file and validates its contents against a schema.
		/// If valid, calculates hashes for specific sections; otherwise, loads default configuration.
		/// </summary>
		private void LoadConfigFile()
		{
			try
			{
				configuration = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
					.AddJsonFile("configure.json", optional: false, reloadOnChange: true).AddEnvironmentVariables()
					.Build();
				var schema = File.ReadAllText(schemaPath);
				var config = File.ReadAllText(filePath);

				if (ConfigValidHelper.ValidateConfigurationJson(schema, config))
				{
					var keysToHash = new[]
					{
						"DbConnection:DataBase",
						"DbConnection:Notify",
						"DbConnection:Alarm",
						"HubSettings:ServerId",
						"HubSettings:Notify",
						"HubSettings:Alarm",
						"HostSettings",
						"Kafka:Producer",
						"Kafka:Consumer"
					};

					foreach (var key in keysToHash)
					{
						sectionHashes[key] = HashHelper.CalculateJsonSectionMd5(filePath, key);
					}
					this.Configuration = configuration;
				}
				else
				{
					LoadDefaultConfiguration();
				}
			}
			catch (Exception ex)
			{
				Log.Error($"Exception with configuration: {ex.Message}");
				LoadDefaultConfiguration();
			}
		}
		private void EnsureConfigurationFile(string configFile)
		{
			filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFile);

			if (!File.Exists(filePath))
			{
				try
				{
					using (File.Create(filePath)) { }
				}
				catch (Exception ex)
				{
					Log.Error($"Exception with creating configuration file: {ex.Message}");
				}
			}
		}
		private void LoadDefaultConfiguration()
		{
			var configuration = new ConfigurationBuilder()
				.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
				.AddJsonFile("configureDefault.json", optional: false, reloadOnChange: true)
			.Build();

			string json = JsonConvert.SerializeObject(BracingServerSettings(configuration), Formatting.Indented);

			if (new FileInfo(filePath).Length == 0)
			{
				File.WriteAllText(filePath, json);
				Log.Information($"The default configuration is used.");
			}
			LoadConfigFile();
		}

		public ServerSettings BracingServerSettings(IConfigurationRoot configuration)
		{
			return new ServerSettings
			{
				DbConnection = new DBSettings
				{
					DataBase = configuration["DbConnection:DataBase"],
					Alarm = new AlarmConnection
					{
						ConnectionString = configuration["DbConnection:Alarm:ConnectionString"] ??
									"host=localhost;port=5432;Database=AlarmsExchange;Username=postgres;Password=19346jaidj"
					},
					Notify = new NotifyConnection
					{
						ConnectionString = configuration["DbConnection:Notify:ConnectionString"] ??
									"host=localhost;port=5432;Database=NotificationsExchange;Username=postgres;Password=19346jaidj"
					}
				},

				HubSettings = new HubSettings
				{
					ServerId = (configuration["HubSettings:ServerId"] == "GENERATE") ? Guid.NewGuid() :
								Guid.Parse(configuration["HubSettings:ServerId"]),
					Notify = new NotifyHubSettings
					{
						DelayMilliseconds = int.Parse(configuration["HubSettings:Notify:DelayMilliseconds"] ?? "1000"),
						TargetClients = configuration["HubSettings:Notify:TargetClients"] ?? "ContextClient",
						HubMethod = configuration["HubSettings:Notify:HubMethod"] ?? "ReceiveAlarmHandler"
					},
					Alarm = new AlarmHubSettings
					{
						DelayMilliseconds = int.Parse(configuration["HubSettings:Alarm:DelayMilliseconds"] ?? "1000"),
						TargetClients = configuration["HubSettings:Alarm:TargetClients"] ?? "ContextClient",
						HubMethod = configuration["HubSettings:Alarm:HubMethod"] ?? "ReceiveAlarmHandler",
					}
				},

				HostSettings = new HostSettings
				{
					Port = int.Parse(configuration["HostSettings:Port"]),
					Urls = configuration["HostSettings:Urls"],
					PolicyName = configuration["HostSettings:PolicyName"],
					AllowedOrigins = configuration["HostSettings:AllowedOrigins"],
					RouteNotify = configuration["HostSettings:RouteNotify"],
					RouteAlarm = configuration["HostSettings:RouteAlarm"]
				},

				Kafka = new KafkaSettings
				{
					Consumer = new ConsumerConnection
					{
						BootstrapServers = configuration["Kafka:Consumer:BootstrapServers"] ?? "localhost:9092;localhost:9093"
					},
					Producer = new ProducerConnection
					{
						BootstrapServers = configuration["Kafka:Producer:BootstrapServers"] ?? "localhost:9092;localhost:9093"
					}
				}
			};
		}
	}
}

	

