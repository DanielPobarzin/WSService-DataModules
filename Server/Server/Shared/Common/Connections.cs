using Interactors.Enums;
using Serilog;
using System.Collections.Concurrent;

namespace Shared.Common
{
	public class Connection
	{
		public Guid ServerId { get; set; }
		public Guid ClientId { get; set; }
		public string? ConnectionId { get; set; }
		public ConnectionStatus Status { get; set; }
		public DateTime? TimeStampOpenConnection { get; set; }
		public DateTime? TimeStampCloseConnection { get; set; }
		public TimeSpan? Session { get; set; }
	}

	public class ConnectionsHandler
	{
		private static readonly Lazy<ConnectionsHandler> lazy =
		new Lazy<ConnectionsHandler>(() => new ConnectionsHandler());
		public static ConnectionsHandler Instance { get { return lazy.Value; } }
		private readonly ConcurrentDictionary<string, Connection> _All = new();
		public void AddConnection(string connectionId, Guid serverId, Guid clientId)
		{
			_All[connectionId] = new Connection
			{
				ServerId = serverId,
				ClientId = clientId,
				ConnectionId = connectionId,
				Status = ConnectionStatus.Opened,
				TimeStampOpenConnection = DateTime.Now,
			};
		}
		public void RemoveConnection(string connectionId)
		{
			if (_All.ContainsKey(connectionId))
			{
				_All[connectionId].Status = ConnectionStatus.Closed;
				_All[connectionId].TimeStampCloseConnection = DateTime.Now;
				_All[connectionId].Session = _All[connectionId].TimeStampCloseConnection - _All[connectionId].TimeStampOpenConnection;
			}
			else
			{
				Log.Information("Does not contain a connection, cannot be closed.");
			}
		}
		public Connection GetConnection(string connectionId)
		{
			var connection = (_All.TryGetValue(connectionId, out Connection? connect)) ? connect : null;
			return connection;
		}
	}
}
