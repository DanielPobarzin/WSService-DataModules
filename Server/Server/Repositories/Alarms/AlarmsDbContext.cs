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
		private readonly IConfiguration configuration;
		private string connectionString;
		public AlarmsDbContext(IConfiguration configuration)
		{
			this.configuration = configuration;
		}
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
			try
			{
				switch (configuration["DbConnection:DataBase"])
				{
					case ("PostgreSQL"):
						connectionString = configuration["DbConnection:Alarm:ConnectionString"];
						optionsBuilder.UseNpgsql(connectionString);
						break;
					default:  throw new NotFoundException(configuration["DbConnection:DataBase"], connectionString);
				}
			}catch(Exception ex) {
				Log.Error($"Error Type: {ex.GetType()}. Message: {ex.Message}");
			}
        }
	}
}
