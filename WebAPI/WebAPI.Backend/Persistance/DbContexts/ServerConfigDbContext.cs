using Domain.Settings.SignalRServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;
using Persistance.DbContexts;
using Persistance.DbContexts.EntityTypeConfiguration.SignalRServerEFC;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.DbContexts
{
	public class ServerConfigDbContext : DbContext
	{
		public DbSet<DbConnection> DbConnections { get; set; }
		public DbSet<HubSettings> HubSettings { get; set; }
		public DbSet<HostSettings> HostSettings { get; set; }
		private string connectionString;

		public ServerConfigDbContext(DbContextOptions<ServerConfigDbContext> options) :
			base(options)
		{ }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
				modelBuilder.ApplyConfiguration(new DbConnectionConfiguration());
				modelBuilder.ApplyConfiguration(new HubSettingsConfiguration());
				modelBuilder.ApplyConfiguration(new HostSettingsConfiguration());

				base.OnModelCreating(modelBuilder);
		}

		//protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		//{
		//	connectionString = "DbConnection/...../";
		//	optionsBuilder.UseNpgsql(connectionString);
		//}

	}
}
