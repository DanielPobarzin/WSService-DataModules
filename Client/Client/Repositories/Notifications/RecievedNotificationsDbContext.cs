using Entities.Entities;
using Interactors.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Repositories.DO;
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
		public DbSet<DomainObjectNotification> Notifications { get; set; }
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
			builder.Entity<DomainObjectNotification>(entity =>
			{
				entity.ToTable("notificationsclient");
				
				entity.Property(e => e.ClientId).HasColumnName("clientid");
				entity.Property(e => e.ServerId).HasColumnName("serverid");
			
				entity.Property(e => e.DateAndTimeSendDataByServer)
				      .ValueGeneratedNever()
					  .HasColumnType("timestamp without time zone")
					  .HasColumnName("date_and_time_send_data_by_server");
				entity.Property(e => e.DateAndTimeRecievedDataFromServer)
					  .ValueGeneratedNever()
					  .HasColumnType("timestamp without time zone")
					  .HasColumnName("date_and_time_recieved_data_from_server");

				entity.HasOne(e => e.Notification)
						.WithOne();
				builder.Entity<Notification>(notificationEntity =>
				{
					notificationEntity.ToTable("notificationsclient");
					notificationEntity.Property(n => n.Id)
							 .ValueGeneratedNever()
								.HasColumnName("id");
					notificationEntity.Property(n => n.Value).HasColumnName("value");
					notificationEntity.Property(n => n.Quality).HasColumnName("quality");
					notificationEntity.Property(n => n.Content).HasColumnName("content");
					notificationEntity.Property(n => n.CreationDateTime)
						  .ValueGeneratedNever()
						  .HasColumnType("timestamp without time zone")
						  .HasColumnName("creationdate");
				});
			});
		}

	}
}
