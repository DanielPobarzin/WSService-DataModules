using Entities.Enums;
using Entities.Settings;
using Interactors.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Serilog;

namespace Communications.UoW
{
	/// <summary>
	/// Configuration class responsible for loading and managing application configuration.
	/// </summary>
	/// <remarks>
	/// The unit of work of the class performs configuration loading (initial or when modified).
	/// It listens for changes to the configuration file and updates the configuration accordingly.
	/// </remarks>
	public class UnitOfWorkGetConfig
	{
		/// <summary>
		/// Gets the current configuration settings.
		/// </summary>
		public IConfiguration Configuration;

		/// <summary>
		/// A dictionary that stores hashes of specific configuration sections 
		/// to track changes in the configuration.
		/// </summary>
		public Dictionary<string, string> sectionHashes;

		private IConfigurationRoot configuration;
		private string filePath;
		private string schemaPath;
		private bool isInitialized = false;
		/// <summary>
		/// Constructor of the configuration class.
		/// </summary>
		/// <remarks>
		/// Initializes the configuration by loading from the specified JSON file 
		/// and sets up a change token to monitor for updates.
		/// </remarks>
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
		/// Loads the configuration file and validates its content.
		/// </summary>
		/// <remarks>
		/// If the configuration file is valid, it builds the configuration object 
		/// and calculates hashes for specific sections. If the file is invalid, 
		/// it falls back to a default configuration file.
		/// </remarks>
		public void LoadConfigFile()
		{
			try
			{
				configuration = new ConfigurationBuilder()
				.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
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
						"ClientSettings:ClientId",
						"ClientSettings:UseCache",
						"ClientSettings:Mode",
						"ConnectionSettings:Alarm",
						"ConnectionSettings:Notify",
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
			catch (Exception ex) {
				Log.Error($"Exception with configuration: {ex.Message}");
				LoadDefaultConfiguration();
			}

		}
		private void LoadDefaultConfiguration()
		{
			var configuration = new ConfigurationBuilder()
				.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
				.AddJsonFile("configureDefault.json", optional: false, reloadOnChange: true)
				.Build();

			var config = new Settings
			{
				DBSettings = new DBSettings
				{
					DataBase = configuration["DbConnection:DataBase"],
					Alarm = new AlarmDataBase
					{
						ConnectionString = configuration["DbConnection:Notify:ConnectionString"]
					},
					Notify = new NotifyDataBase
					{
						ConnectionString = configuration["DbConnection:Alarm:ConnectionString"]
					}
				},

				CLientSettings = new CLientSettings
				{
					Id = (Guid.Parse(configuration["ClientSettings:ClientId"]) == Guid.Empty) ? Guid.Parse(configuration["ClientSettings:ClientId"]) :
					Guid.Parse(configuration["ClientSettings:ClientId"]),
					UseCache = bool.Parse(configuration["ClientSettings:UseCache"]),
					Mode = (ConnectionMode)Enum.Parse(typeof(ConnectionMode), configuration["ClientSettings:Mode"])
				},

				ConnectSettings = new ConnectSettings
				{
					Notify = new NotifyConnection
					{
						Url = configuration["ConnectionSettings:Notify:Url"]
					},
					Alarm = new AlarmConnection
					{
						Url = configuration["ConnectionSettings:Alarm:Url"]
					}
				},

				Kafka = new Kafka
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

	}

}