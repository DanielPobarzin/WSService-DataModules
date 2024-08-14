using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Settings.SignalRServer;

namespace Persistance.DbContexts.EntityTypeConfiguration.SignalRServerEFC
{
	public class KafkaSettingsConfiguration : IEntityTypeConfiguration<KafkaSettings>
	{
		public void Configure(EntityTypeBuilder<KafkaSettings> builder)
		{
			builder.ToTable("kafka_settings");
			builder.HasKey(e => e.SystemId);
			builder.Property(e => e.SystemId)
				.HasColumnName("server_id")
				.ValueGeneratedOnAdd();
			builder.OwnsOne(e => e.Producer, alarmBuilder =>
			{
				alarmBuilder.Property(a => a.BootstrapServers)
					.HasColumnName("producer_bootstrap_server")
					.HasColumnType("varchar(255)");
			});

			builder.OwnsOne(e => e.Consumer, notifyBuilder =>
			{
				notifyBuilder.Property(n => n.BootstrapServers)
					.HasColumnName("consumer_bootstrap_server")
					.HasColumnType("varchar(255)");
			});
			builder.HasOne<KafkaSettings>()
				   .WithMany()
				   .HasForeignKey(e => e.SystemId)
				   .OnDelete(DeleteBehavior.Cascade);
		}
	}
}
