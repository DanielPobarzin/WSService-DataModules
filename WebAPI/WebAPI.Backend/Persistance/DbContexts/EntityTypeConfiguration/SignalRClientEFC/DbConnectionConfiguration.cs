using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Settings.SignalRClient;

namespace Persistance.DbContexts.EntityTypeConfiguration.SignalRClientEFC
{
	public class DbConnectionConfiguration : IEntityTypeConfiguration<DBSettings>
	{
		public void Configure(EntityTypeBuilder<DBSettings> builder)
		{
			builder.ToTable("db_connection");
			builder.HasKey(e => e.SystemId);
			builder.Property(e => e.SystemId)
					.HasColumnName("client_id")
					.ValueGeneratedOnAdd();
			builder.Property(e => e.DataBase)
					.HasColumnName("database_type")
					.HasColumnType("varchar(20)");
			builder.Property(e => e.Alarm.ConnectionString)
					.HasColumnName("alarm_connection_string")
					.HasColumnType("text");
			builder.Property(e => e.Notify.ConnectionString)
			.HasColumnName("notify_connection_string")
					.HasColumnType("text");
			builder.HasOne<DBSettings>()
					   .WithMany()
					   .HasForeignKey(e => e.SystemId)
					   .OnDelete(DeleteBehavior.Cascade);
		}
	}
}
