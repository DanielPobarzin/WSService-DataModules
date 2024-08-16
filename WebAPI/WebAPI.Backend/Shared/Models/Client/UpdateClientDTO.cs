using Application.Features.Clients.Commands.UpdateClient;
using Application.Mappings;
using AutoMapper;
using Domain.Enums;

namespace Shared.Models.Client
{
	public class UpdateClientDTO : IMapWith<UpdateClientCommand>
	{
		public Guid Id { get; set; }
		public WorkStatus WorkStatus { get; set; }
		public ConnectionStatus ConnectionStatus { get; set; }
		public string? ConnectionId { get; set; }
		public void Mapping(Profile profile)
		{
			profile.CreateMap<AddClientDTO, UpdateClientCommand>()
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