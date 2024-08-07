using Domain.Settings.SignalRClient;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.DbContexts.EntityTypeConfiguration.SignalRClientEFC
{
	public class ConnectionSettingsConfiguration : IEntityTypeConfiguration<ConnectSettings>
	{
		public void Configure(EntityTypeBuilder<ConnectSettings> builder)
		{
			builder.ToTable("connect_settings");
			builder.HasKey(e => e.SystemId);
			builder.Property(e => e.SystemId)
				   .HasColumnName("client_id")
				   .HasColumnType("uuid");
				builder.Property(e => e.Alarm.Url)
					.HasColumnName("address_to_Alarm")
					.HasColumnType("text");
				builder.Property(e => e.Notify.Url)
					.HasColumnName("address_to_Notify")
					.HasColumnType("text");
				builder.HasOne<ConnectSettings>()
					.WithMany()
					.HasForeignKey(e => e.SystemId)
					.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
