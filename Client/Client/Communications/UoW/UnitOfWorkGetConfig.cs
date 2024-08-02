using Interactors.Enums;
using Interactors.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Shared.Common;
using System;

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
			filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "configure.json");
			schemaPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "schema.json");
			sectionHashes = new Dictionary<string, string>();
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
			configuration = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
				.AddJsonFile("configure.json", optional: false, reloadOnChange: true)
				.Build();
			var schema = File.ReadAllText(schemaPath);
			var config = File.ReadAllText(filePath);

			if (ConfigValidHelper.ValidateConfigurationJson(schema, config))
			{
				sectionHashes["DbConnection:DataBase"] = HashHelper.CalculateJsonSectionMd5(filePath, "DbConnection:DataBase");
				sectionHashes["DbConnection:NotifyConnectionString"] = HashHelper.CalculateJsonSectionMd5(filePath, "DbConnection:NotifyConnectionString");
				sectionHashes["DbConnection:AlarmConnectionString"] = HashHelper.CalculateJsonSectionMd5(filePath, "DbConnection:AlarmConnectionString");
				sectionHashes["ClientSettings:ClientId"] = HashHelper.CalculateJsonSectionMd5(filePath, "ClientSettings:ClientId");
				sectionHashes["ClientSettings:UseCache"] = HashHelper.CalculateJsonSectionMd5(filePath, "ClientSettings:UseCache");
				sectionHashes["ClientSettings:Mode"] = HashHelper.CalculateJsonSectionMd5(filePath, "ClientSettings:Mode");
				sectionHashes["ConnectionSettings:AlarmUrl"] = HashHelper.CalculateJsonSectionMd5(filePath, "ConnectionSettings:AlarmUrl");
				sectionHashes["ConnectionSettings:NotifyUrl"] = HashHelper.CalculateJsonSectionMd5(filePath, "ConnectionSettings:NotifyUrl");
				sectionHashes["Kafka:Producer"] = HashHelper.CalculateJsonSectionMd5(filePath, "Kafka:Producer");
				sectionHashes["Kafka:Consumer"] = HashHelper.CalculateJsonSectionMd5(filePath, "Kafka:Consumer");

				this.Configuration = configuration;
			}
			else
			{
				configuration = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
					.AddJsonFile("configureDefault.json", optional: false, reloadOnChange: true)
					.Build();
				this.Configuration = configuration;
			}
		}
	}

}


