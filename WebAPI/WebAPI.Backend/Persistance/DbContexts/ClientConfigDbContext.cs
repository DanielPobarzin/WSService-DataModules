using Domain.Settings.SignalRClient;
using Microsoft.EntityFrameworkCore;
using Persistance.DbContexts.EntityTypeConfiguration.SignalRClientEFC;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.DbContexts
{
	public class ClientConfigDbContext : DbContext
	{
		public DbSet<DbConnection> DbConnections { get; set; }
		public DbSet<ConnectSettings> ConnectSettings { get; set; }
		public DbSet<OtherSettings> OtherSettings { get; set; }
		private string connectionString;

		public ClientConfigDbContext(DbContextOptions<ServerConfigDbContext> options) :
		base(options)
		{ }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfiguration(new DbConnectionConfiguration());
			modelBuilder.ApplyConfiguration(new ConnectionSettingsConfiguration());
			modelBuilder.ApplyConfiguration(new ClientSettingsConfiguration());

			base.OnModelCreating(modelBuilder);
		}

		//protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		//{
		//	connectionString = "DbConnection/...../";
		//	optionsBuilder.UseNpgsql(connectionString);
		//}
	}
}
