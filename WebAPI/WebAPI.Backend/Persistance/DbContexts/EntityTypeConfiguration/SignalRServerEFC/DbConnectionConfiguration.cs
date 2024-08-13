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
				builder.Property(e => e.AlarmDB.ConnectionString)
					.HasColumnName("alarm_connection_string")
					.HasColumnType("text");
				builder.Property(e => e.NotificationDB.ConnectionString)
					.HasColumnName("notify_connection_string")
					.HasColumnType("text");
				builder.HasOne<DBSettings>()
					   .WithMany()
					   .HasForeignKey(e => e.SystemId)
					   .OnDelete(DeleteBehavior.Cascade);
		}
	}
}
