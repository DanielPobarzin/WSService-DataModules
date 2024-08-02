using Domain.Settings.SignalRServer;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.DbContexts.EntityTypeConfiguration.SignalRServerEFC
{
	public class HostSettingsConfiguration : IEntityTypeConfiguration<HostSettings>
	{
		public void Configure(EntityTypeBuilder<HostSettings> builder)
		{
			builder.ToTable("host_settings");
			builder.HasKey(e => e.SystemId);
			builder.Property(e => e.SystemId)
				   .HasColumnName("server_id")
				   .HasColumnType("uuid");
			builder.Property(e => e.Port)
					.HasColumnName("port");
				builder.Property(e => e.Urls)
					.HasColumnName("urls")
					.HasColumnType("text");
				builder.Property(e => e.PolicyName)
					.HasColumnName("policy_name")
					.HasColumnType("varchar(255)");
				builder.Property(e => e.AllowedOrigins)
					.HasColumnName("allowed_origins")
					.HasColumnType("text");
				builder.Property(e => e.RouteNotify)
					.HasColumnName("route_notify")
					.HasColumnType("varchar(255)");
				builder.Property(e => e.RouteAlarm)
					.HasColumnName("route_alarm")
					.HasColumnType("varchar(255)");
				builder.HasOne<HostSettings>()
					.WithMany()
					.HasForeignKey(e => e.SystemId)
					.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
