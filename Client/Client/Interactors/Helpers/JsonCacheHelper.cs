using Newtonsoft.Json;
using Serilog;

namespace Communications.Common.Helpers
{
	public class JsonCacheHelper
	{
		public async Task WriteToFileCache<T> (List<T> messages, Guid clientid) 
		{
			string directoryPath = $"{typeof(T).Name}Caches/";
			string filePath = $"{directoryPath}Data_{clientid}.json";
			
			if (!Directory.Exists(directoryPath))
			{
				Directory.CreateDirectory(directoryPath);
			}
			string jsonNewData = JsonConvert.SerializeObject(messages, Formatting.Indented);
			await Task.Run(() => File.WriteAllText(filePath, jsonNewData));
			
		}
		public async Task<IEnumerable<T>> ReadFromFileCache<T>(Guid clientid) 
		{
			string directoryPath = $"{typeof(T).Name}Caches/";
			string filePath = $"{directoryPath}Data_{clientid}.json";

			if (File.Exists(filePath))
			{
				string jsonOldData = File.ReadAllText(filePath);
				var CacheData = JsonConvert.DeserializeObject<IEnumerable<T>>(jsonOldData);
				return await Task.FromResult(CacheData);
			}
				Log.Information($"The file cache for the client {clientid} was not detected.");
			return await Task.FromResult(Enumerable.Empty<T>());

		}
	}
}