using Entities.Entities;
using Entities.Models;
using Interactors.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Connections
{
	public class ConnectionDbContext : DbContext
	{
		private IConfiguration configuration;
		private string connectionString;
		public DbSet<ConnectionContext> Connections { get; set; }
		public ConnectionDbContext(IConfiguration configuration)
		{
			this.configuration = configuration;
		}
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			switch (configuration["DbContextConnection:DataBase"])
			{
				case ("SQLite"):
					connectionString = configuration["DbContextConnection:ConnectionString"];
					optionsBuilder.UseSqlite(connectionString);
					break;
				default: Log.Error("The database is not defined."); throw new NotFoundException(configuration["DbConnection:DataBase"], connectionString);
			}
		}
		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			builder.Entity<ConnectionContext>(entity =>
			{
				entity.ToTable(name: "Connections");
				entity.HasKey(e => e.ConnectionId);
				entity.Property(e => e.ServerId).HasColumnName(name: "Server").IsRequired();
				entity.Property(e => e.ClientId).HasColumnName(name: "Client").IsRequired();
				entity.Property(e => e.HubRoute).HasColumnName(name: "Hub").IsRequired();
				entity.Property(e => e.StartConnection).HasColumnName(name: "Start_Connection").HasColumnType("timestamp without time zone").IsRequired(false);
				entity.Property(e => e.EndConnection).HasColumnName(name: "End_Connection").HasColumnType("timestamp without time zone").IsRequired(false);
				entity.Property(e => e.Session).HasColumnName(name: "Session").HasColumnType("timestamp without time zone").IsRequired(false);
			});

		}
	}
}