using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities;

namespace Persistance.DbContexts
{
	public class ConnectionDbContext : DbContext
	{
		public DbSet<Connection> Connections { get; set; }
		private string connectionString;
		public ConnectionDbContext(DbContextOptions<ConnectionDbContext> options) :
			base(options)
		{ }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfiguration(new ConnectionConfiguration());
			base.OnModelCreating(modelBuilder);
		}
		//protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		//{
		//	connectionString = "DbConnection/...../";
		//	optionsBuilder.UseNpgsql(connectionString);
		//}
		private class ConnectionConfiguration : IEntityTypeConfiguration<Connection>
		{
			public void Configure(EntityTypeBuilder<Connection> builder)
			{
				builder.ToTable("Connections");
				builder.HasKey(e => e.ConnectionId);
				builder.Property(e => e.ConnectionId)
					   .HasColumnName("connection_id")
					   .HasColumnType("text")
					   .ValueGeneratedNever()
					   .IsRequired();
				builder.Property(e => e.ServerId)
					   .HasColumnName("server_id")
					   .HasColumnType("uuid")
					   .IsRequired();
				builder.Property(e => e.ClientId)
					   .HasColumnName("client_id")
					   .HasColumnType("uuid")
					   .IsRequired();
				builder.Property(e => e.Status)
					   .HasColumnName("connection_status");
				builder.Property(e => e.TimeStampOpenConnection)
					   .HasColumnName("connection_open")
					   .HasColumnType("timestamp without time zone");
				builder.Property(e => e.TimeStampCloseConnection)
					   .HasColumnName("connection_close")
					   .HasColumnType("timestamp without time zone");
				builder.Property(e => e.Session)
					   .HasColumnName("session")
					   .HasColumnType("timestamp without time zone");

			}
		}
	}
}
