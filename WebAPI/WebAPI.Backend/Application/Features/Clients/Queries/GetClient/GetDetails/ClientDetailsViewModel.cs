using Application.Mappings;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;

namespace Application.Features.CLients.Queries.GetClient.GetDetails
{
	public class ClientDetailsViewModel : IMapWith<Client>
	{
		public Guid ClientId { get; set; }
		public WorkStatus WorkStatus { get; set; }
		public ConnectionStatus ConnectionStatus { get; set; }
		public string? ConnectionId { get; set; }

		public void Mapping(Profile profile)
		{
			profile.CreateMap<Client, ClientDetailsViewModel>()
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
