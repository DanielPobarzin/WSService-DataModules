using Microsoft.Extensions.Configuration;

namespace Interactors.Helpers
{
	public class SerializeHelper
	{
		public static Dictionary<string, object> BuildConfigDictionary(IConfiguration configuration)
		{
			var dict = new Dictionary<string, object>();

			foreach (var section in configuration.GetChildren())
			{
				dict[section.Key] = BuildConfigDictionary(section);
			}
			if (dict.Count == 0)
			{
				return configuration != null ? new Dictionary<string, object> { { "", configuration } } : [];
			}
			return dict;
		}
	}
}
