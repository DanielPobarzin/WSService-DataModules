using Application.Mappings;
using AutoMapper;
using Domain.Entities;

namespace Application.Features.Clients.Queries.GetClient.GetAll
{
	/// <summary>
	/// Data Transfer Object (DTO) for looking up client information.
	/// </summary>
	public class ClientLookupDTO : IMapWith<Client>
	{
		public Guid ClientId { get; set; }

		public void Mapping(Profile profile)
		{
			profile.CreateMap<Client, ClientLookupDTO>()
				.ForMember(clientDto => clientDto.ClientId,
					opt => opt.MapFrom(client => client.Id));
		}
	}
}
