using Serilog;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Communications.Helpers
{
	public class HashHelper
	{
		public static string CalculateJsonSectionMd5(string filePath, string sectionName)
		{
			using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
			{
				using (StreamReader reader = new StreamReader(fileStream))
				{
					string jsonContent = reader.ReadToEnd();

					string pattern = $"\"{Regex.Escape(sectionName)}\"\\s*:\\s*{{[^}}]*}}";
					Match match = Regex.Match(jsonContent, pattern);

					if (!match.Success)
					{
						Log.Error("The required section was not found in the configuration file. The hash is undefined.");
					}

					string sectionJson = match.Value;

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
	}
}

