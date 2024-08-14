using Domain.Settings.SignalRClient;
using Microsoft.EntityFrameworkCore;
using Persistance.DbContexts.EntityTypeConfiguration.SignalRClientEFC;

namespace Persistance.DbContexts
{
	public class ClientConfigDbContext : DbContext
	{
		public DbSet<ClientSettings> ClientSettings { get; set; }
		private string connectionString;

		public ClientConfigDbContext(DbContextOptions<ClientConfigDbContext> options) :
		base(options)
		{ }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
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
