using Application.Features.Servers.Commands.UpdateServer;
using Application.Mappings;
using AutoMapper;
using Domain.Enums;

namespace Shared.Models.Server
{
	public class UpdateServerDTO : IMapWith<UpdateServerCommand>
	{
		public Guid Id { get; set; }
		public WorkStatus WorkStatus { get; set; }
		public ConnectionStatus ConnectionStatus { get; set; }
		public string? ConnectionId { get; set; }
		public int CountListeners { get; set; }

		public void Mapping(Profile profile)
		{
			profile.CreateMap<UpdateServerDTO, UpdateServerCommand>()
				.ForMember(serverDto => serverDto.Id,
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
