using Application.Features.Configurations.Server.Commands.CreateConfig;
using Application.Mappings;
using AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models.Config
{
    public class CreateServerConfigDTO : IMapWith<CreateConfigServerCommand>
    {
		[Required]
		public Guid SystemId { get; set; }
		[Required]
		public int Port { get; set; }
		[Required]
		public string Urls { get; set; }
        public string PolicyName { get; set; }
        public string AllowedOrigins { get; set; }
		[Required]
		public string RouteNotify { get; set; }
		[Required]
		public string RouteAlarm { get; set; }
        public int AlarmDelayMilliseconds { get; set; }
        public int NotifyDelayMilliseconds { get; set; }
		[Required]
		public string NotifyHubMethod { get; set; }
		[Required]
		public string AlarmHubMethod { get; set; }
        public string NotifyTargetClients { get; set; }
        public string AlarmTargetClients { get; set; }
		[Required]
		public string DB { get; set; }
		[Required]
		public string AlarmDB { get; set; }
		[Required]
		public string NotificationDB { get; set; }
		[Required]
		public string ConsumerBootstrapServer { get; set; }
		[Required]
		public string ProducerBootstrapServer { get; set; }
		public void Mapping(Profile profile)
        {
            profile.CreateMap<CreateServerConfigDTO, CreateConfigServerCommand>()
                .ForMember(configCommand => configCommand.SystemId,
                     opt => opt.MapFrom(config => config.SystemId))
                .ForMember(configCommand => configCommand.DB,
                     opt => opt.MapFrom(config => config.DB))
                .ForMember(configCommand => configCommand.AlarmDB,
                     opt => opt.MapFrom(config => config.AlarmDB))
                .ForMember(configCommand => configCommand.NotificationDB,
                     opt => opt.MapFrom(config => config.NotificationDB))
                .ForMember(configCommand => configCommand.Port,
                     opt => opt.MapFrom(config => config.Port))
                .ForMember(configCommand => configCommand.Urls,
                     opt => opt.MapFrom(config => config.Urls))
                .ForMember(configCommand => configCommand.PolicyName,
                     opt => opt.MapFrom(config => config.PolicyName))
                .ForMember(configCommand => configCommand.RouteAlarm,
                     opt => opt.MapFrom(config => config.RouteAlarm))
                .ForMember(configCommand => configCommand.RouteNotify,
                     opt => opt.MapFrom(config => config.RouteNotify))
                .ForMember(configCommand => configCommand.AlarmDelayMilliseconds,
                     opt => opt.MapFrom(config => config.AlarmDelayMilliseconds))
                .ForMember(configCommand => configCommand.NotifyDelayMilliseconds,
                     opt => opt.MapFrom(config => config.NotifyDelayMilliseconds))
                .ForMember(configCommand => configCommand.NotifyHubMethod,
                     opt => opt.MapFrom(config => config.NotifyHubMethod))
                .ForMember(configCommand => configCommand.AlarmHubMethod,
                     opt => opt.MapFrom(config => config.AlarmHubMethod))
                .ForMember(configCommand => configCommand.NotifyTargetClients,
                     opt => opt.MapFrom(config => config.NotifyTargetClients))
                .ForMember(configCommand => configCommand.AlarmTargetClients,
                     opt => opt.MapFrom(config => config.AlarmTargetClients))
				 .ForMember(configCommand => configCommand.ConsumerBootstrapServer,
					 opt => opt.MapFrom(config => config.ConsumerBootstrapServer))
				.ForMember(configCommand => configCommand.ProducerBootstrapServer,
					 opt => opt.MapFrom(config => config.ProducerBootstrapServer));
		}
    }
}
