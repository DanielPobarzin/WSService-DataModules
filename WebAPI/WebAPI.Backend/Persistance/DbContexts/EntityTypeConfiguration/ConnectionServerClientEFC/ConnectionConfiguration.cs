using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.DbContexts.EntityTypeConfiguration.ConnectionServerClientEFC
{
	public class ConnectionConfiguration : IEntityTypeConfiguration<Domain.Entities.Connection>
	{
		public void Configure(EntityTypeBuilder<Domain.Entities.Connection> builder)
		{
			builder.ToTable("connections");
			builder.HasKey(e => e.ConnectionId);
			builder.Property(e => e.ConnectionId)
			   .HasColumnName("connectinon_id")
			   .HasColumnType("varchar(50)");
			builder.Property(e => e.ServerId)
			   .HasColumnName("server_id")
			   .HasColumnType("uuid");
			builder.Property(e => e.Status)
			   .HasColumnName("connection_status")
			   .HasColumnType("varchar(20)");
			builder.Property(e => e.ClientId)
			   .HasColumnName("client_id")
			   .HasColumnType("uuid");
			builder.Property(e => e.TimeStampOpenConnection)
			   .HasColumnName("time_stamp_open_connection")
			   .HasColumnType("timestamp without time zone");
			builder.Property(e => e.TimeStampCloseConnection)
			   .HasColumnName("time_stamp_close_connection")
			   .HasColumnType("timestamp without time zone");
			builder.Property(e => e.Session)
				   .HasColumnName("session")
				   .HasColumnType("interval");
			builder.HasOne(c => c.Client)
					.WithMany(cl => cl.Connections)
					.HasForeignKey(c => c.ClientId);
			builder.HasOne(c => c.Server)
				   .WithMany(s => s.Connections)
	               .HasForeignKey(c => c.ServerId);
		}
	}
}



