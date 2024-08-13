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
			ChangeToken.OnChange(() => configuration.GetReloadToken(), () =>
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
					.AddJsonFile("configure.json", optional: false, reloadOnChange: true)
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
			string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFile);

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
			var config = new ServerSettings
			{
				ServerDB = new DBSettings
				{
					DB = configuration["DbConnection:DataBase"],
					AlarmDB = new AlarmConnection
					{
						ConnectionString = configuration["DbConnection:Alarm:ConnectionString"]
					},
					NotificationDB = new NotifyConnection
					{
						ConnectionString = configuration["DbConnection:Notify:ConnectionString"]
					}
				},

				ServerHost = new HostSettings
				{
					Port = int.Parse(configuration["HostSettings:Port"]),
					Urls = configuration["HostSettings:Urls"],
					PolicyName = configuration["HostSettings:PolicyName"],
					AllowedOrigins = configuration["HostSettings:AllowedOrigins"],
					RouteNotify = configuration["HostSettings:RouteNotify"],
					RouteAlarm = configuration["HostSettings:RouteAlarm"]
				},

				ServerHub = new HubSettings
				{
					ServerId = (Guid.Parse(configuration["HubSettings:ServerId"]) == Guid.Empty) ? Guid.NewGuid() : 
					Guid.Parse(configuration["HubSettings:ServerId"]),
					Notify = new NotifyHubSettings
					{
						DelayMilliseconds = int.Parse(configuration["HubSettings:Notify:DelayMilliseconds"]),
						TargetClients = configuration["HubSettings:Notify:TargetClients"],
						HubMethod = configuration["HubSettings:Notify:HubMethod"],
					},
					Alarm = new AlarmHubSettings
					{
						DelayMilliseconds = int.Parse(configuration["HubSettings:Alarm:DelayMilliseconds"]),
						TargetClients = configuration["HubSettings:Alarm:TargetClients"],
						HubMethod = configuration["HubSettings:Alarm:HubMethod"],
					}
				},

				ServerKafka = new KafkaSettings
				{
					Consumer = new ConsumerConnection
					{
						BootstrapServers = configuration["Kafka:Consumer:BootstrapServers"]
					},
					Producer = new ProducerConnection
					{
						BootstrapServers = configuration["Kafka:Producer:BootstrapServers"]
					}
				}
			};
			string json = JsonConvert.SerializeObject(config, Formatting.Indented);

			if (new FileInfo(filePath).Length == 0)
			{
				File.WriteAllText(filePath, json);
				Log.Information($"The default configuration is used.");
			}
			LoadConfigFile();
		}
	}
}

	

