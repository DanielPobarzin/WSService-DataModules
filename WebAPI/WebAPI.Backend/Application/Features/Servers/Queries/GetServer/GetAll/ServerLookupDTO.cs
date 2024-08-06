using Application.Features.Connections.Queries.GetConnectionsList;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Servers.Queries.GetServer.GetAll
{
	public class ServerLookupDTO
	{
		public Guid ServerId { get; set; }

		public void Mapping(Profile profile)
		{
			profile.CreateMap<Server, ServerLookupDTO>()
				.ForMember(serverDto => serverDto.ServerId,
					 opt => opt.MapFrom(server => server.Id));
		}
	}
}
