using Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repositories.EntityTypeConfiguration
{
	public class AlarmConfig : IEntityTypeConfiguration<Alarm>
	{
		public void Configure(EntityTypeBuilder<Alarm> builder)
		{
			builder.HasKey(e => e.Id);
			builder.HasIndex(e => e.Id).IsUnique();
			builder.Property(e => e.Id).HasColumnName("id");
			builder.Property(e => e.Content).HasColumnName("content");
			builder.Property(e => e.CreationDateTime).HasColumnName("creationdate");
			builder.Property(e => e.Quality).HasColumnName("quality");
			builder.Property(e => e.Value).HasColumnName("value");
			builder.ToTable("alarms");

		}
	}
}