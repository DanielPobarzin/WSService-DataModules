using Domain.Settings.SignalRClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.DbContexts.EntityTypeConfiguration.SignalRClientEFC
{
	public class ClientSettingsConfiguration : IEntityTypeConfiguration<ClientSettings>
	{
		public void Configure(EntityTypeBuilder<ClientSettings> builder)
		{
			
			builder.ToTable("client_configuration");
			builder.HasKey(e => e.SystemId);
			builder.Property(e => e.SystemId)
				.HasColumnName("client_id")
				.HasColumnType("uuid");

			builder.OwnsOne(e => e.ConnectSettings, connectBuilder =>
			{
				connectBuilder.OwnsOne(m => m.Alarm, alarmBuilder =>
				{
					alarmBuilder.Property(c => c.Url)
					.HasColumnName("address_to_alarm")
					.HasColumnType("text");
				});
				connectBuilder.OwnsOne(m => m.Notify, notifyBuilder =>
				{
					notifyBuilder.Property(c => c.Url)
					.HasColumnName("address_to_notify")
					.HasColumnType("text");
				});
			});

			builder.OwnsOne(e => e.DBSettings, dbBuilder =>
			{
				dbBuilder.OwnsOne(m => m.Alarm, alarmBuilder =>
				{
					alarmBuilder.Property(c => c.ConnectionString)
					.HasColumnName("alarm_connection_string")
					.HasColumnType("text");
				});
				dbBuilder.OwnsOne(m => m.Notify, notifyBuilder =>
				{
					notifyBuilder.Property(c => c.ConnectionString)
					.HasColumnName("notify_connection_string")
					.HasColumnType("text");
				});
				dbBuilder.Property(c => c.DataBase)
					.HasColumnName("database_type")
					.HasColumnType("varchar(20)");
			});

			builder.OwnsOne(e => e.KafkaSettings, kafkaBuilder =>
			{
				kafkaBuilder.OwnsOne(m => m.Consumer, consumerBuilder =>
				{
					consumerBuilder.Property(c => c.BootstrapServers)
					.HasColumnName("consumer_bootstrap_server")
					.HasColumnType("varchar(255)");
				});
				kafkaBuilder.OwnsOne(m => m.Producer, producerBuilder =>
				{
					producerBuilder.Property(c => c.BootstrapServers)
					.HasColumnName("producer_bootstrap_server")
					.HasColumnType("varchar(255)");
				});
			});
			builder.OwnsOne(e => e.ModeSettings, modeBuilder =>
			{
				modeBuilder.Property(c => c.ClientId).HasColumnName("system_id")
				.HasColumnType("uuid");
				modeBuilder.Property(c => c.Mode).HasColumnName("mode")
					.HasColumnType("varchar(20)");
				modeBuilder.Property(c => c.UseCache).HasColumnName("caching")
					.HasColumnType("boolean");
			});
		}	
	}
}
