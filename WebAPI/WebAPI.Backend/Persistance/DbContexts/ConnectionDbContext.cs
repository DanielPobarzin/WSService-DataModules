using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistance.DbContexts.EntityTypeConfiguration.ConnectionServerClientEFC;

namespace Persistance.DbContexts
{
	public class ConnectionDbContext : DbContext
	{
		public DbSet<Server> Servers { get; set; }
		public DbSet<Client> Clients { get; set; }
		public DbSet<Connection> Connections { get; set; }
		private string connectionString;
		public ConnectionDbContext(DbContextOptions<ConnectionDbContext> options) :
			base(options)
		{ }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfiguration(new ConnectionConfiguration());
			modelBuilder.ApplyConfiguration(new ServerConfiguration());
			modelBuilder.ApplyConfiguration(new ClientConfiguration());
			modelBuilder.ApplyConfiguration(new ClientConfiguration());

			base.OnModelCreating(modelBuilder);
		}
		//protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		//{
		//	connectionString = "DbConnection/...../";
		//	optionsBuilder.UseNpgsql(connectionString);
		//}

	}
}

