using Application.Features.Configurations.Server.Queries.GetConfigServerList;
using Application.Mappings;
using AutoMapper;
using Domain.Settings.SignalRClient;

namespace Application.Features.Configurations.Client.Queries.GetConfigClientList
{
	public class ConfigClientLookupDTO : IMapWith<ClientSettings>
	{
		public Guid Id { get; set; }

		public void Mapping(Profile profile)
		{
			profile.CreateMap<ClientSettings, ConfigClientLookupDTO>()
				.ForMember(clientDto => clientDto.Id,
					 opt => opt.MapFrom(client => client.SystemId));
		}
	}
}
