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
	public class ClientSettingsConfiguration : IEntityTypeConfiguration<ModeSettings>
	{
		public void Configure(EntityTypeBuilder<ModeSettings> builder)
		{
			builder.ToTable("client_settings");
			builder.HasKey(e => e.SystemId);
			builder.Property(e => e.SystemId)
				   .HasColumnName("client_id")
				   .HasColumnType("uuid");
			builder.Property(e => e.UseCache)
					.HasColumnName("caching")
					.HasColumnType("boolean");
			builder.Property(e => e.Mode)
					.HasColumnName("mode")
					.HasColumnType("varchar(20)");
			builder.HasOne<ModeSettings>()
				   .WithMany()
	               .HasForeignKey(e => e.SystemId)
	               .OnDelete(DeleteBehavior.Cascade);
		}
	}
}
