using Entities.Entities;
using Interactors.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Repositories.Notifications
{
	public class NotificationsDbContext : MessagesDbContext
    {
		public DbSet<Notification> Notifications { get; set; }
		private IConfiguration configuration;
		private string connectionString;
		public NotificationsDbContext(IConfiguration configuration)
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
						connectionString = configuration["DbConnection:Notify:ConnectionString"];
						optionsBuilder.UseNpgsql(connectionString);
						break;
					default:  throw new NotFoundException(configuration["DbConnection:DataBase"], connectionString);
				}
			}catch (Exception ex)
			{
				Log.Error($"Error Type: {ex.GetType()}. Message: {ex.Message}");
			}
			
        }
	}
}
