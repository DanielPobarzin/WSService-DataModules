using Entities.Entities;
using Entities.Models;
using Interactors.Interfaces;
using Microsoft.EntityFrameworkCore;
using Repositories.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Repositories.Connections
{
	public class ConnectionRepository : IRepositoryPublish<ConnectionContext>
	{
		private ConnectionDbContext db;

		public ConnectionRepository(ConnectionDbContext context)
		{
			this.db = context;
		}
		public async Task AddConnection(ConnectionContext entity)
		{
			await db.Connections.AddAsync(entity);
			await db.SaveChangesAsync();
		}
		public async Task RemoveConnection(string connectionId)
		{
			var connection = await db.Connections.FirstOrDefaultAsync(c => c.ConnectionId == connectionId);
			if (connection != null)
			{
				var endConnection = DateTime.Now;
				connection.EndConnection = endConnection;
				connection.Session = endConnection - connection.StartConnection;
				await db.SaveChangesAsync();
			}

		}
	}
}