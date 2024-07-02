using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Communications.Connections
{
	public class Connections<T> where T : Hub
	{
		private readonly ConcurrentDictionary<string, string> _all = new();
		public void AddConnection(string connectionId, string userId)
		{
			_all[connectionId] = userId;
		}

		public void RemoveConnection(string connectionId)
		{
			_all.TryRemove(connectionId, out _);
		}
		public string GetConnection(string connectionId) 
		{
			var value = (_all.TryGetValue(connectionId, out string connect)) ? connect : null;
			return value;
		} 
		public List<string> GetConnections() => _all.Keys.ToList();
	}
}

