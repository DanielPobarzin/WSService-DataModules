using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using Domain.Settings.SignalRServer;

namespace Persistance.DbContexts.EntityTypeConfiguration.SignalRServerEFC
{
	public class DbConnectionConfiguration : IEntityTypeConfiguration<DBSettings>
	{
		public void Configure(EntityTypeBuilder<DBSettings> builder)
		{
				builder.ToTable("db_connection");
				builder.HasKey(e => e.SystemId);
				builder.Property(e => e.SystemId)
					.HasColumnName("server_id")
					.ValueGeneratedOnAdd();
				builder.Property(e => e.DB)
					.HasColumnName("database_type")
					.HasColumnType("varchar(20)");
				builder.OwnsOne(e => e.AlarmDB, alarmBuilder =>
				{
					alarmBuilder.Property(a => a.ConnectionString)
						.HasColumnName("alarm_connection_string")
						.HasColumnType("text");
				});

				builder.OwnsOne(e => e.NotificationDB, notifyBuilder =>
				{
					notifyBuilder.Property(n => n.ConnectionString)
						.HasColumnName("notify_connection_string")
						.HasColumnType("text");
				});
				builder.HasOne<DBSettings>()
					   .WithMany()
					   .HasForeignKey(e => e.SystemId)
					   .OnDelete(DeleteBehavior.Cascade);
		}
	}
}
