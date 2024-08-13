using Entities.Entities;
using Interactors.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Repositories.DO;
using Serilog;

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
					connectionString = configuration["DbConnection:Notify:ConnectionString"];
					optionsBuilder.UseNpgsql(connectionString);
					break;
					default: Log.Error("The database is not defined."); throw new NotFoundException(configuration["DbConnection:DataBase"], connectionString);
			}
        }
		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			builder.Entity<DomainObjectNotification>().ToTable("notificationsclient");

			builder.Entity<Notification>().HasKey(n => n.Id);
			builder.Entity<DomainObjectNotification>().HasKey(d => d.MessageId);
			
			builder.Entity<DomainObjectNotification>()
				   .Property(e => e.MessageId).HasColumnName("messageid");
			builder.Entity<DomainObjectNotification>()
				   .Property(e => e.RecipientId).HasColumnName("clientid");
			builder.Entity<DomainObjectNotification>()
				   .Property(e => e.SenderId).HasColumnName("serverid");
			builder.Entity<DomainObjectNotification>()
				   .Property(e => e.DateAndTimeSendDataByServer)
				   .ValueGeneratedNever()
				   .HasColumnType("timestamp without time zone")
				   .HasColumnName("date_and_time_send_data_by_server");
			builder.Entity<DomainObjectNotification>()
				   .Property(e => e.DateAndTimeRecievedDataFromServer)
				   .ValueGeneratedNever()
				   .HasColumnType("timestamp without time zone")
				   .HasColumnName("date_and_time_recieved_data_from_server");

			builder.Ignore<Notification>();

			builder.Entity<DomainObjectNotification>()
				   .OwnsOne(e => e.Notification,
							owned =>
								{
									owned.ToTable("notificationsclient");
									owned.Property(n => n.Id).ValueGeneratedNever().HasColumnName("id");
									owned.Property(n => n.Value).HasColumnName("value");
									owned.Property(n => n.Quality).HasColumnName("quality");
									owned.Property(n => n.Content).HasColumnName("content");
									owned.Property(n => n.CreationDateTime)
										 .ValueGeneratedNever()
										 .HasColumnType("timestamp without time zone")
										 .HasColumnName("creation_date");
								});
		}
	}
}






