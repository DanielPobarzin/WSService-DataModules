using Domain.Settings.SignalRClient;
using Microsoft.EntityFrameworkCore;
using Persistance.DbContexts.EntityTypeConfiguration.SignalRClientEFC;

namespace Persistance.DbContexts
{
	public class ClientConfigDbContext : DbContext
	{
		public DbSet<DBSettings> DbSettings { get; set; }
		public DbSet<ConnectSettings> ConnectSettings { get; set; }
		public DbSet<ModeSettings> ModeSettings { get; set; }
		public DbSet<KafkaSettings> KafkaSettings { get; set; }
		private string connectionString;

		public ClientConfigDbContext(DbContextOptions<ServerConfigDbContext> options) :
		base(options)
		{ }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfiguration(new DbConnectionConfiguration());
			modelBuilder.ApplyConfiguration(new ConnectionSettingsConfiguration());
			modelBuilder.ApplyConfiguration(new ClientSettingsConfiguration());
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
