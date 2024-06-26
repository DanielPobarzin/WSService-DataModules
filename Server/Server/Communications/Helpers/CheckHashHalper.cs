﻿using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communications.Helpers
{

	public class CheckHashHalper
	{
		private static Dictionary<string, string> SectionHashes = new Dictionary<string, string>();

		public async IAsyncEnumerable<Dictionary<string, string>> CompareHashConfiguration(Dictionary<string, string> sectionHashes)
		{
			await Task.Yield();
			while (true)
			{
				var d3 = SectionHashes.Any() ?
					 sectionHashes
					.Where(entry => SectionHashes.ContainsKey(entry.Key) && SectionHashes[entry.Key] != entry.Value)
					.ToDictionary(entry => entry.Key, entry => entry.Value)
					: new();

				if (d3.Count > 0)
				{
					yield return d3;
				}
					
				foreach (var entry in sectionHashes)
				{
					SectionHashes[entry.Key] = entry.Value;
				}
				await Task.Delay(100);
			}
		}
	}
}
