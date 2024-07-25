using Entities.Entities;
using Interactors.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Repositories.DO;
using Serilog;

namespace Repositories.Alarms
{
	public class RecievedAlarmsDbContext : DbContext
    {
		public DbSet<DomainObjectAlarm> Alarms { get; set; }
		private IConfiguration configuration;
		private string connectionString;
		public RecievedAlarmsDbContext(IConfiguration configuration)
		{
			this.configuration = configuration;
		}
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
			switch (configuration["DbConnection:DataBase"])
			{
				case ("PostgreSQL"): 
					connectionString = configuration["DbConnection:AlarmConnectionString"];
					optionsBuilder.UseNpgsql(connectionString);
					break;
					default: Log.Error("The database is not defined."); throw new NotFoundException(configuration["DbConnection:DataBase"], connectionString);
			}
        }
		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			builder.Entity<DomainObjectAlarm>().ToTable("alarmsclient");
			
			builder.Entity<DomainObjectAlarm>().HasKey(d => d.MessageId);
			
			builder.Entity<DomainObjectAlarm>()
				   .Property(e => e.MessageId).HasColumnName("messageid");
			builder.Entity<DomainObjectAlarm>()
				   .Property(e => e.RecipientId).HasColumnName("clientid");
			builder.Entity<DomainObjectAlarm>()
				   .Property(e => e.SenderId).HasColumnName("serverid");
			builder.Entity<DomainObjectAlarm>()
				   .Property(e => e.DateAndTimeSendDataByServer)
				   .ValueGeneratedNever()
				   .HasColumnType("timestamp without time zone")
				   .HasColumnName("date_and_time_send_data_by_server");
			builder.Entity<DomainObjectAlarm>()
				   .Property(e => e.DateAndTimeRecievedDataFromServer)
				   .ValueGeneratedNever()
				   .HasColumnType("timestamp without time zone")
				   .HasColumnName("date_and_time_recieved_data_from_server");
			builder.Entity<Alarm>().HasKey(n => n.Id);
			builder.Ignore<Alarm>();
			builder.Entity<DomainObjectAlarm>()
				   .OwnsOne(e => e.Alarm,
							owned =>
								{
									owned.ToTable("alarmsclient");
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






