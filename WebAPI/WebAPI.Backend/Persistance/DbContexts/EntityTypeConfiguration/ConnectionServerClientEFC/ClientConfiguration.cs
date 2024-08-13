using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.DbContexts.EntityTypeConfiguration.ConnectionServerClientEFC
{
	public class ClientConfiguration : IEntityTypeConfiguration<Domain.Entities.Client>
	{
		public void Configure(EntityTypeBuilder<Domain.Entities.Client> builder)
		{
			builder.ToTable("clients");
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id)
			   .HasColumnName("client_id")
			   .HasColumnType("uuid");
			builder.Property(e => e.WorkStatus)
			   .HasColumnName("work_status")
			   .HasColumnType("varchar(20)");
			builder.Property(e => e.ConnectionStatus)
			   .HasColumnName("connection_status")
			   .HasColumnType("varchar(20)");
			builder.Property(e => e.ConnectionId)
			   .HasColumnName("connection_id")
			   .HasColumnType("varchar(50)");
		}
	}
}
