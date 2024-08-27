using Application.Mappings;
using AutoMapper;
using Domain.Settings.SignalRClient;

namespace Application.Features.Configurations.Client.Queries.GetConfigClientList
{
	public class ConfigClientLookupDTO : IMapWith<CLientSettings>
	{
		public Guid Id { get; set; }

		public void Mapping(Profile profile)
		{
			profile.CreateMap<CLientSettings, ConfigClientLookupDTO>()
				.ForMember(clientDto => clientDto.Id,
					 opt => opt.MapFrom(client => client.ClientSettings.ClientId));
		}
	}
}
