using Entities.Entities;
using Interactors.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Notifications
{
    public class RecievedNotificationsDbContext : DbContext
    {
		public DbSet<RecievedByClientNotification> Notifications { get; set; }
		private IConfiguration configuration;
		private string connectionString;
		public RecievedNotificationsDbContext(IConfiguration configuration)
		{
			this.configuration = configuration;
		}
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
			switch (configuration["DbConnection:DataBase"])
			{
				case ("PostgreSQL"): 
					connectionString = configuration["DbConnection:ConnectionString"];
					optionsBuilder.UseNpgsql(connectionString);
					break;
					default: Log.Error("The database is not defined."); throw new NotFoundException(configuration["DbConnection:DataBase"], connectionString);
			}
        }
		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			builder.Entity<RecievedByClientNotification>(entity =>
			{
				entity.ToTable("notificationsclient");
				entity.Property(e => e.Notification.Id)
					  .ValueGeneratedNever()
				      .HasColumnName("id");
				entity.Property(e => e.ClientId).HasColumnName("clientid");
				entity.Property(e => e.ServerId).HasColumnName("serverid");
				entity.Property(e => e.Notification.Content).HasColumnName("content");
				entity.Property(e => e.Notification.CreationDateTime)
					  .ValueGeneratedNever()
					  .HasColumnType("timestamp without time zone")
					  .HasColumnName("creationdate");
				entity.Property(e => e.Notification.Quality)
					  .HasMaxLength(1)
					  .HasColumnName("quality");
				entity.Property(e => e.Notification.Value).HasColumnName("value");
				entity.Property(e => e.DateAndTimeSendDataByServer)
				      .ValueGeneratedNever()
					  .HasColumnType("timestamp without time zone")
					  .HasColumnName("dateandtimesenddatabyserver");
				entity.Property(e => e.DateAndTimeRecievedDataFromServer)
					  .ValueGeneratedNever()
					  .HasColumnType("timestamp without time zone")
					  .HasColumnName("dateandtimerecieveddatabyserver");
			});
		}

	}
}
