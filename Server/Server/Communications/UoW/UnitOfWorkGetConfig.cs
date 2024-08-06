using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Primitives;
using System.Runtime;
using System.Threading;
using Serilog;
using Newtonsoft.Json.Linq;
using Communications.Helpers;
using Interactors.Helpers;

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
		/// Loads the configuration file and validates its contents against a schema.
		/// If valid, calculates hashes for specific sections; otherwise, loads default configuration.
		/// </summary>
		private void LoadConfigFile()
		{
			configuration = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
				.AddJsonFile("configure.json", optional: false, reloadOnChange: true)
				.Build();
			var schema = File.ReadAllText(schemaPath);
			var config = File.ReadAllText(filePath);
			if (ConfigValidHelper.ValidateConfigurationJson(schema, config))
			{
				sectionHashes["DbConnection:DataBase"] = HashHelper.CalculateJsonSectionMd5(filePath, "DbConnection:DataBase");
				sectionHashes["DbConnection:Alarm"] = HashHelper.CalculateJsonSectionMd5(filePath, "DbConnection:Alarm");
				sectionHashes["DbConnection:Notify"] = HashHelper.CalculateJsonSectionMd5(filePath, "DbConnection:Notify");
				sectionHashes["HubSettings:ServerId"] = HashHelper.CalculateJsonSectionMd5(filePath, "HubSettings:ServerId");
				sectionHashes["HubSettings:Notify"] = HashHelper.CalculateJsonSectionMd5(filePath, "HubSettings:Notify");
				sectionHashes["HubSettings:Alarm"] = HashHelper.CalculateJsonSectionMd5(filePath, "HubSettings:Alarm");
				sectionHashes["HostSettings"] = HashHelper.CalculateJsonSectionMd5(filePath, "HostSettings");
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



