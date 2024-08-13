using Application.Mappings;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Connections.Queries.GetConnectionDetails
{
	public class ConnectionDetailsViewModel : IMapWith<Connection>
	{
		public Guid ClientId { get; set; }
		public Guid ServerId { get; set; }
		public string ConnectionId { get; set; }
		public TimeSpan? Session { get; set; }
		public DateTime? TimeStampCloseConnection { get; set; }
		public DateTime? TimeStampOpenConnection { get; set; }
		public ConnectionStatus Status { get; set; }
		public void Mapping(Profile profile)
		{

			profile.CreateMap<Connection, ConnectionDetailsViewModel>()
					.ForMember(connectionVm => connectionVm.ClientId,
							   opt => opt.MapFrom(connection => connection.ClientId))
					.ForMember(connectionVm => connectionVm.ServerId,
							   opt => opt.MapFrom(connection => connection.ServerId))
					.ForMember(connectionVm => connectionVm.ConnectionId,
							   opt => opt.MapFrom(connection => connection.ConnectionId))
					.ForMember(connectionVm => connectionVm.Session,
							   opt => opt.MapFrom(connection => connection.Session))
					.ForMember(connectionVm => connectionVm.TimeStampCloseConnection,
							   opt => opt.MapFrom(connection => connection.TimeStampCloseConnection))
					.ForMember(connectionVm => connectionVm.TimeStampOpenConnection,
							   opt => opt.MapFrom(connection => connection.TimeStampOpenConnection))
					.ForMember(connectionVm => connectionVm.Status,
							   opt => opt.MapFrom(connection => connection.Status));
		}
	}
}
