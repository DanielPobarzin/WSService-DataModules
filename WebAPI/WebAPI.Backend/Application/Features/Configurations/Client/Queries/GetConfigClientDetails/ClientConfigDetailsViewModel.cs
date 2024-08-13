using Application.Features.Configurations.Server.Queries.GetConfigServerDetails;
using Application.Mappings;
using AutoMapper;
using Domain.Enums;
using Domain.Settings.SignalRClient;

namespace Application.Features.Configurations.Client.Queries.GetConfigClientDetails
{
	public class ClientConfigDetailsViewModel : IMapWith<ClientSettings>
	{
		public Guid SystemId { get; set; }
		public string DB { get; set; }
		public string AlarmDB { get; set; }
		public string NotificationDB { get; set; }
		public string NotifyUrl { get; set; }
		public string AlarmUrl { get; set; }
		public bool UseCache { get; set; }
		public ConnectionMode Mode { get; set; }
		public string ConsumerBootstrapServer { get; set; }
		public string ProducerBootstrapServer { get; set; }

		public void Mapping(Profile profile)
		{
			profile.CreateMap<ClientSettings, ClientConfigDetailsViewModel>()
					.ForMember(configVm => configVm.SystemId,
							   opt => opt.MapFrom(config => config.SystemId))
					.ForMember(configVm => configVm.DB,
							   opt => opt.MapFrom(config => config.DBSettings.DataBase))
					.ForMember(configVm => configVm.AlarmDB,
							   opt => opt.MapFrom(config => config.DBSettings.Alarm.ConnectionString))
					.ForMember(configVm => configVm.NotificationDB,
							   opt => opt.MapFrom(config => config.DBSettings.Notify.ConnectionString))
					.ForMember(configVm => configVm.NotifyUrl,
							   opt => opt.MapFrom(config => config.ConnectSettings.Notify.Url))
					.ForMember(configVm => configVm.AlarmUrl,
							   opt => opt.MapFrom(config => config.ConnectSettings.Alarm.Url))
					.ForMember(configVm => configVm.UseCache,
							   opt => opt.MapFrom(config => config.ModeSettings.UseCache))
					.ForMember(configVm => configVm.Mode,
							   opt => opt.MapFrom(config => config.ModeSettings.Mode))
					.ForMember(configVm => configVm.ConsumerBootstrapServer,
							   opt => opt.MapFrom(config => config.KafkaSettings.Consumer.BootstrapServers))
					.ForMember(configVm => configVm.ProducerBootstrapServer,
							   opt => opt.MapFrom(config => config.KafkaSettings.Producer.BootstrapServers));
		}
	}
}
