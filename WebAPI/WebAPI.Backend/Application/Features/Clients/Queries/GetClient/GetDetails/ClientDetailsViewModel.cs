using Application.Features.Servers.Queries.GetServer.GetAll;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.CLients.Queries.GetClient.GetDetails
{
	public class ClientDetailsViewModel
	{
		public Guid ServerId { get; set; }
		public WorkStatus WorkStatus { get; set; }
		public ConnectionStatus ConnectionStatus { get; set; }
		public string? ConnectionId { get; set; }
		public int CountListeners { get; set; }

		public void Mapping(Profile profile)
		{
			profile.CreateMap<Server, ClientDetailsViewModel>()
				.ForMember(serverDto => serverDto.ServerId,
					 opt => opt.MapFrom(server => server.Id))
				.ForMember(serverDto => serverDto.WorkStatus,
					 opt => opt.MapFrom(server => server.WorkStatus))
				.ForMember(serverDto => serverDto.ConnectionStatus,
					 opt => opt.MapFrom(server => server.ConnectionStatus))
				.ForMember(serverDto => serverDto.ConnectionId,
					 opt => opt.MapFrom(server => server.ConnectionId))
				.ForMember(serverDto => serverDto.CountListeners,
					 opt => opt.MapFrom(server => server.CountListeners));
		}
	}
}
