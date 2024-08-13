using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Application.Mappings;
using Domain.Entities;

namespace Application.Features.Connections.Queries.GetConnectionsList
{
	public class ConnectionLookupDTO : IMapWith<Connection>
	{
		public Guid ConnectionId { get; set; }
		public Guid ClientId { get; set; }
		public Guid ServerId { get; set; }

		public void Mapping(Profile profile)
		{
			profile.CreateMap<Connection, ConnectionLookupDTO>()
				.ForMember(connectionDto => connectionDto.ConnectionId,
					 opt => opt.MapFrom(connection => connection.ConnectionId))
				.ForMember(connectionDto => connectionDto.ServerId,
					 opt => opt.MapFrom(connection => connection.ServerId))
				.ForMember(connectionDto => connectionDto.ClientId,
					 opt => opt.MapFrom(connection => connection.ClientId));
		}
	}
}
