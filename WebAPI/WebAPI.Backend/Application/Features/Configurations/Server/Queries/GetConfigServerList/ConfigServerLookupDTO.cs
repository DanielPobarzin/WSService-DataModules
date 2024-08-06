using AutoMapper;
using Domain.Settings.SignalRServer;

namespace Application.Features.Configurations.Server.Queries.GetConfigServerList
{
	public class ConfigServerLookupDTO
	{
		public Guid Id { get; set; }

		public void Mapping(Profile profile)
		{
			profile.CreateMap<ServerSettings, ConfigServerLookupDTO>()
				.ForMember(serverDto => serverDto.Id,
					 opt => opt.MapFrom(server => server.SystemId));
		}
	}
}
