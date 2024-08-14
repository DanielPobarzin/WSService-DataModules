using Application.Mappings;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;

namespace Application.Features.Servers.Queries.GetServer.GetDetails
{
	public class ServerDetailsViewModel : IMapWith<Server>
	{
		public Guid ServerId { get; set; }
		public WorkStatus WorkStatus { get; set; }
		public ConnectionStatus ConnectionStatus { get; set; }
		public string? ConnectionId { get; set; }
		public int CountListeners { get; set; }

		public void Mapping(Profile profile)
		{
			profile.CreateMap<Server, ServerDetailsViewModel>()
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
