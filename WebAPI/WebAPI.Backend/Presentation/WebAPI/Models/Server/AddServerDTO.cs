using Application.Features.Servers.Commands.AddServer;
using Application.Features.Servers.Queries.GetServer.GetDetails;
using Application.Mappings;
using AutoMapper;
using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models.Server
{
	public class AddServerDTO : IMapWith<AddServerCommand>
    {
		[Required]
		public Guid ServerId { get; set; }
		[Required]
		public WorkStatus WorkStatus { get; set; }
		[Required]
		public ConnectionStatus ConnectionStatus { get; set; }
		public string? ConnectionId { get; set; }
		[Required]
		public int CountListeners { get; set; }

		public void Mapping(Profile profile)
		{
			profile.CreateMap<AddServerDTO, AddServerCommand>()
				.ForMember(serverDto => serverDto.ServerId,
					 opt => opt.MapFrom(server => server.ServerId))
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
