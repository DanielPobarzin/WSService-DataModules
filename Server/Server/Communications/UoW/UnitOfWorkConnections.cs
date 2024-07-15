using Entities.Entities;
using Microsoft.Extensions.Configuration;
using Repositories.Connections;
using Repositories.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communications.UoW
{
	public class UnitOfWorkConnections : IDisposable
	{
		private ConnectionDbContext db;
		private IConfiguration configuration;
		private ConnectionRepository connectionRepository;
		private bool disposed = false;
		public UnitOfWorkConnections(IConfiguration configuration)
		{
			this.configuration = configuration;
			db = new ConnectionDbContext(configuration);
			Initializer.Initialize(db);
		}
		public ConnectionRepository Notifications
		{
			get
			{
				connectionRepository ??= new ConnectionRepository(db);
				return connectionRepository;
			}
		}
		public void Save()
		{
			db.SaveChanges();
		}
		public virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
					db.Dispose();
				this.disposed = true;
			}
		}
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}


