﻿using Domain.Settings.SignalRServer;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.DbContexts.EntityTypeConfiguration.SignalRServerEFC
{
	public class HubSettingsConfiguration : IEntityTypeConfiguration<HubSettings>
	{
		public void Configure(EntityTypeBuilder<HubSettings> builder)
		{
			builder.ToTable("hub_settings");
			builder.HasKey(e => e.SystemId);
			builder.Property(e => e.SystemId)
				   .HasColumnName("server_id")
				   .HasColumnType("uuid");
				builder.Property(e => e.Notify.DelayMilliseconds)
					.HasColumnName("notify_delay_milliseconds");
				builder.Property(e => e.Notify.HubMethod)
					.HasColumnName("notify_hub_method")
					.HasColumnType("varchar(255)");
				builder.Property(e => e.Notify.TargetClients)
					.HasColumnName("notify_target_clients")
					.HasColumnType("varchar(255)");
				builder.Property(e => e.Alarm.DelayMilliseconds)
					.HasColumnName("alarm_delay_milliseconds");
				builder.Property(e => e.Alarm.HubMethod)
					.HasColumnName("alarm_hub_method")
					.HasColumnType("varchar(255)");
				builder.Property(e => e.Alarm.TargetClients)
					.HasColumnName("alarm_target_clients")
					.HasColumnType("varchar(255)");
				builder.HasOne<HubSettings>()
					.WithMany()
					.HasForeignKey(e => e.SystemId)
					.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
