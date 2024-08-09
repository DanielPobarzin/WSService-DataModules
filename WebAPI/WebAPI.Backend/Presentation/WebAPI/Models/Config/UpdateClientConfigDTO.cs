using Application.Features.Configurations.Client.Commands.UpdateConfig;
using AutoMapper;
using Domain.Enums;

namespace WebAPI.Models.Config
{
    public class UpdateClientConfigDTO
    {
        public Guid SystemId { get; set; }
        public string DB { get; set; }
        public string AlarmDB { get; set; }
        public string NotificationDB { get; set; }
        public string NotifyUrl { get; set; }
        public string AlarmUrl { get; set; }
        public bool UseCache { get; set; }
        public ConnectionMode Mode { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<UpdateClientConfigDTO, UpdateConfigClientCommand>()
                .ForMember(configCommand => configCommand.SystemId,
                     opt => opt.MapFrom(config => config.SystemId))
                .ForMember(configCommand => configCommand.DB,
                     opt => opt.MapFrom(config => config.DB))
                .ForMember(configCommand => configCommand.AlarmDB,
                     opt => opt.MapFrom(config => config.AlarmDB))
                .ForMember(configCommand => configCommand.NotificationDB,
                     opt => opt.MapFrom(config => config.NotificationDB))
                .ForMember(configCommand => configCommand.NotifyUrl,
                     opt => opt.MapFrom(config => config.NotifyUrl))
                .ForMember(configCommand => configCommand.AlarmUrl,
                     opt => opt.MapFrom(config => config.AlarmUrl))
                .ForMember(configCommand => configCommand.UseCache,
                     opt => opt.MapFrom(config => config.UseCache))
                .ForMember(configCommand => configCommand.Mode,
                     opt => opt.MapFrom(config => config.Mode));
        }
    }
}
