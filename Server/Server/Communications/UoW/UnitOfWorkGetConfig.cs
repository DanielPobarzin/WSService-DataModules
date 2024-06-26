﻿using Microsoft.Extensions.Configuration;
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
				var sections = configuration.GetChildren();
				foreach (var section in sections)
				{
					var sectionJson = section.Key.ToString();
					var sectionHash = HashHelper.CalculateJsonSectionMd5(filePath, sectionJson);
					sectionHashes[section.Key] = sectionHash;
				}
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



