using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Communications.Connections
{
	/// <summary>
	/// Represents a connection manager for SignalR hubs, allowing to add, remove, and retrieve connections.
	/// </summary>
	/// <typeparam name="T">The type of the hub that this connection manager is associated with. 
	/// Must be derived from <see cref="Hub"/>.</typeparam>
	public class ConcurrentConnections<T> where T : Hub
	{
		private readonly ConcurrentDictionary<string, string> _all = new();

		/// <summary>
		/// Adds a new connection to the manager.
		/// </summary>
		/// <param name="connectionId">The unique identifier for the connection.</param>
		/// <param name="userId">The unique identifier for the user associated with the connection.</param>
		public void AddConnection(string connectionId, string userId)
		{
			_all[connectionId] = userId;
		}

		/// <summary>
		/// Removes a connection from the manager.
		/// </summary>
		/// <param name="connectionId">The unique identifier for the connection to be removed.</param>
		public void RemoveConnection(string connectionId)
		{
			_all.TryRemove(connectionId, out _);
		}

		/// <summary>
		/// Retrieves the user ID associated with a specific connection ID.
		/// </summary>
		/// <param name="connectionId">The unique identifier for the connection.</param>
		/// <returns>The user ID associated with the connection, or null if the connection does not exist.</returns>
		public string? GetConnection(string connectionId) 
		{
			var value = (_all.TryGetValue(connectionId, out string? connect))? connect : null;
			return value;
		}

		/// <summary>
		/// Gets a list of all active connection IDs.
		/// </summary>
		/// <returns>A list of connection IDs currently managed by this instance.</returns>
		public List<string> GetConnections() => _all.Keys.ToList();
	}

}

