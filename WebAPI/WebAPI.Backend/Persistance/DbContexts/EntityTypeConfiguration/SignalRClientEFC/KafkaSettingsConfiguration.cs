using Domain.Settings.SignalRClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.DbContexts.EntityTypeConfiguration.SignalRClientEFC
{
	public class KafkaSettingsConfiguration : IEntityTypeConfiguration<KafkaSettings>
	{
		public void Configure(EntityTypeBuilder<KafkaSettings> builder)
		{
			builder.ToTable("kafka_settings");
			builder.HasKey(e => e.SystemId);
			builder.Property(e => e.SystemId)
				.HasColumnName("client_id")
				.ValueGeneratedOnAdd();
			builder.Property(e => e.Producer.BootstrapServers)
				.HasColumnName("producer_bootstrap_server")
				.HasColumnType("varchar(255)");
			builder.Property(e => e.Consumer.BootstrapServers)
				.HasColumnName("consumer_bootstrap_server")
				.HasColumnType("varchar(255)");
			builder.HasOne<KafkaSettings>()
				   .WithMany()
				   .HasForeignKey(e => e.SystemId)
				   .OnDelete(DeleteBehavior.Cascade);
		}
	}
}
