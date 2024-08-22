using Newtonsoft.Json.Linq;
using Serilog;
using System.Security.Cryptography;
using System.Text;

namespace Interactors.Helpers
{
	public class HashHelper
	{
		public static string CalculateJsonSectionMd5(string filePath, string sectionName)
		{
			using FileStream fileStream = new (filePath, FileMode.Open);
			using StreamReader reader = new (fileStream);
			string jsonContent = reader.ReadToEnd();
			var jsonObject = JObject.Parse(jsonContent);

			string jsonPath = sectionName.Replace(":", ".");

			var section = jsonObject.SelectToken(jsonPath);

			if (section == null)
			{
				Log.Error($"The {sectionName} section was not found in the configuration file. The hash is undefined.");
				section = "";
			}
			string sectionJson = section.ToString();
			byte[] bytes = Encoding.UTF8.GetBytes(sectionJson);
			using (MD5 md5 = MD5.Create())
			{
				byte[] hashBytes = md5.ComputeHash(bytes);
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < hashBytes.Length; i++)
				{
					sb.Append(hashBytes[i].ToString("x2"));
				}

				return sb.ToString();
			}
		}

	}
}