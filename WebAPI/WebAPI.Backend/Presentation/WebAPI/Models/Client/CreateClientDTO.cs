using Application.Features.Clients.Commands.AddClient;
using Application.Mappings;
using AutoMapper;
using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models.Client
{
	public class AddClientDTO : IMapWith<AddClientCommand>
    {
		[Required]
		public Guid Id { get; set; }
		[Required]
		public WorkStatus WorkStatus { get; set; }
		[Required]
		public ConnectionStatus ConnectionStatus { get; set; }
		public string? ConnectionId { get; set; }
		public void Mapping(Profile profile)
		{
			profile.CreateMap<AddClientDTO, AddClientCommand>()
				.ForMember(clientDto => clientDto.ClientId,
					 opt => opt.MapFrom(client => client.Id))
				.ForMember(clientDto => clientDto.WorkStatus,
					 opt => opt.MapFrom(client => client.WorkStatus))
				.ForMember(clientDto => clientDto.ConnectionStatus,
					 opt => opt.MapFrom(client => client.ConnectionStatus))
				.ForMember(clientDto => clientDto.ConnectionId,
					 opt => opt.MapFrom(client => client.ConnectionId));
		}
	}
}
