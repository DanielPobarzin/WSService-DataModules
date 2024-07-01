using Communications.DTO;
using Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Communications.Helpers
{
	public class JsonCacheHelper
	{
		public async Task WriteToFileCache(List<Notification> notifications, Guid clientid) 
		{
			string directoryPath = $"DataCaches/";
			string filePath = $"{directoryPath}NotificationsSentData_{clientid}.json";
			
			if (!Directory.Exists(directoryPath))
			{
				Directory.CreateDirectory(directoryPath);
				DestroyFileCache(filePath);
			}
			string jsonNewData = JsonConvert.SerializeObject(notifications, Formatting.Indented);
			await Task.Run(() => File.WriteAllText(filePath, jsonNewData));
			
		}
		public async Task<IEnumerable<Notification>> ReadFromFileCache(Guid clientid) 
		{
			string directoryPath = $"DataCaches/";
			string filePath = $"{directoryPath}NotificationsSentData_{clientid}.json";
			if (File.Exists(filePath))
			{
				string jsonOldData = File.ReadAllText(filePath);
				var CacheData = JsonConvert.DeserializeObject<IEnumerable<Notification>>(jsonOldData);
				return await Task.FromResult(CacheData);
			}
				Log.Information($"The file cache for the client {clientid} was not detected.");
			return await Task.FromResult(Enumerable.Empty<Notification>());

		}
		private void DestroyFileCache(string filePath)
		{
			Task.Run(async () =>
			{
				//await Task.Delay(TimeSpan.FromDays(31));
				await Task.Delay(TimeSpan.FromSeconds(10));
				File.Delete(filePath);
			});
		}
	}
}