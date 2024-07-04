using Entities.Entities;
using Interactors.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Repositories.Alarms
{
	public class AlarmsDbContext : MessagesDbContext
    {
		public DbSet<Alarm> Alarms { get; set; }
		private IConfiguration configuration;
		private string connectionString;
		public AlarmsDbContext(IConfiguration configuration)
		{
			this.configuration = configuration;
		}
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
			switch (configuration["DbConnection:DataBase"])
			{
				case ("PostgreSQL"): 
					connectionString = configuration["DbConnection:Alarm:ConnectionString"];
					optionsBuilder.UseNpgsql(connectionString);
					break;
					default: Log.Error("The database is not defined."); throw new NotFoundException(configuration["DbConnection:DataBase"], connectionString);
			}
        }
		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			builder.Entity<Notification>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Id).HasColumnName("id");
				entity.Property(e => e.Content).HasColumnName("content");
				entity.Property(e => e.CreationDateTime).HasColumnName("creationdate");
				entity.Property(e => e.Quality).HasColumnName("quality");
				entity.Property(e => e.Value).HasColumnName("value");
				entity.ToTable("alarms");
			});
		}

	}
}
