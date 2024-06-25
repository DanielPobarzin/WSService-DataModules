using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;
using Serilog;

namespace Communications.Helpers
{
	public class ConfigValidHelper
	{
		public static bool ValidateConfigurationJson(string pathSchema, string pathConfig)
		{
			JSchema schema = JSchema.Parse(File.ReadAllText(pathSchema));
			JObject jsonObject = JObject.Parse(File.ReadAllText(pathConfig));
			if (!jsonObject.IsValid(schema))
			{
				Log.Warning("The configuration file was not found or it is incorrect. The default configuration is used.");
				return false;
			}
			Log.Information("The provided configuration is used.");
			return true;
		}
	}


}