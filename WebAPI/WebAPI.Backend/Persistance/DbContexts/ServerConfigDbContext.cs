using Domain.Settings.SignalRServer;
using Microsoft.EntityFrameworkCore;
using Persistance.DbContexts.EntityTypeConfiguration.SignalRServerEFC;

namespace Persistance.DbContexts
{
	public class ServerConfigDbContext : DbContext
	{
		public DbSet<DBSettings> DbSettings { get; set; }
		public DbSet<HubSettings> HubSettings { get; set; }
		public DbSet<HostSettings> HostSettings { get; set; }
		public DbSet<KafkaSettings> KafkaSettings { get; set; }
		private string connectionString;

		public ServerConfigDbContext(DbContextOptions<ServerConfigDbContext> options) :
			base(options)
		{ }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
				modelBuilder.ApplyConfiguration(new DbConnectionConfiguration());
				modelBuilder.ApplyConfiguration(new HostSettingsConfiguration());
				modelBuilder.ApplyConfiguration(new HubSettingsConfiguration());
				modelBuilder.ApplyConfiguration(new KafkaSettingsConfiguration());

			base.OnModelCreating(modelBuilder);
		}

		//protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		//{
		//	connectionString = "DbConnection/...../";
		//	optionsBuilder.UseNpgsql(connectionString);
		//}

	}
}
