using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.DbContexts.EntityTypeConfiguration.ConnectionServerClientEFC
{
	public class ServerConfiguration : IEntityTypeConfiguration<Domain.Entities.Server>
	{
		public void Configure(EntityTypeBuilder<Domain.Entities.Server> builder)
		{
			builder.ToTable("servers");
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id)
			   .HasColumnName("server_id")
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
			builder.Property(e => e.CountListeners)
			   .HasColumnName("listeners");
		}
	}
}
