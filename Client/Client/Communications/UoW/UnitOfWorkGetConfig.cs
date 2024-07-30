using Interactors.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Communications.UoW
{
	public class UnitOfWorkGetConfig
	{
		public IConfiguration Configuration;
		public Dictionary<string, string> sectionHashes;

		private IConfigurationRoot configuration;
		private string filePath;
		private string schemaPath;
		private bool isInitialized = false;

		public UnitOfWorkGetConfig()
		{
			filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "configure.json");
			schemaPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "schema.json");
			sectionHashes = new Dictionary<string, string>();
			LoadConfigFile();
			ChangeToken.OnChange(() => configuration.GetReloadToken(), () =>
			{
				if (isInitialized)
				{ LoadConfigFile();} else 	{isInitialized = true;}
			});
		}

		private void LoadConfigFile()
		{
			configuration = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
				.AddJsonFile("configure.json", optional: false, reloadOnChange: true)
				.Build();
			if (ConfigValidHelper.ValidateConfigurationJson(schemaPath, filePath))
			{
				sectionHashes["DbConnection:DataBase"] = HashHelper.CalculateJsonSectionMd5(filePath, "DbConnection:DataBase");
				sectionHashes["DbConnection:NotifyConnectionString"] = HashHelper.CalculateJsonSectionMd5(filePath, "DbConnection:NotifyConnectionString");
				sectionHashes["DbConnection:AlarmConnectionString"] = HashHelper.CalculateJsonSectionMd5(filePath, "DbConnection:AlarmConnectionString");
				sectionHashes["ClientSettings:ClientId"] = HashHelper.CalculateJsonSectionMd5(filePath, "ClientSettings:ClientId");
				sectionHashes["ClientSettings:UseCache"] = HashHelper.CalculateJsonSectionMd5(filePath, "ClientSettings:UseCache");
				sectionHashes["ClientSettings:Mode"] = HashHelper.CalculateJsonSectionMd5(filePath, "ClientSettings:Mode");
				sectionHashes["ConnectionSettings:AlarmUrl"] = HashHelper.CalculateJsonSectionMd5(filePath, "ConnectionSettings:AlarmUrl");
				sectionHashes["ConnectionSettings:NotifyUrl"] = HashHelper.CalculateJsonSectionMd5(filePath, "ConnectionSettings:NotifyUrl");

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



