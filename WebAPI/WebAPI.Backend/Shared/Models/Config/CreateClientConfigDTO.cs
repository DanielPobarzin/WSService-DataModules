using Application.Features.Configurations.Client.Commands.CreateConfig;
using Application.Mappings;
using AutoMapper;
using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Config
{
	public class CreateClientConfigDTO : IMapWith<CreateConfigClientCommand>
	{
		[Required]
		public Guid SystemId { get; set; }
		[Required]
		public string DB { get; set; }
		[Required]
		public string AlarmDB { get; set; }
		[Required]
		public string NotificationDB { get; set; }
		[Required]
		public string NotifyUrl { get; set; }
		[Required]
		public string AlarmUrl { get; set; }
		public bool UseCache { get; set; }
		[Required]
		public ConnectionMode Mode { get; set; }
		[Required]
		public string ConsumerBootstrapServer { get; set; }
		[Required]
		public string ProducerBootstrapServer { get; set; }

		public void Mapping(Profile profile)
		{
			profile.CreateMap<CreateClientConfigDTO, CreateConfigClientCommand>()
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
					 opt => opt.MapFrom(config => config.Mode))
				.ForMember(configCommand => configCommand.ProducerBootstrapServer,
					 opt => opt.MapFrom(config => config.ProducerBootstrapServer))
				.ForMember(configCommand => configCommand.ConsumerBootstrapServer,
					 opt => opt.MapFrom(config => config.ConsumerBootstrapServer));
		}
	}
}
