using Application.Mappings;
using AutoMapper;
using Domain.Enums;
using Domain.Settings.SignalRClient;

namespace Application.Features.Configurations.Client.Queries.GetConfigClientDetails
{
	/// <summary>
	/// Represents the view model for client configuration details, mapping from <see cref="ClientSettings"/>.
	/// </summary>
	public class ClientConfigDetailsViewModel : IMapWith<CLientSettings>
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
			profile.CreateMap<CLientSettings, ClientConfigDetailsViewModel>()
					.ForMember(configVm => configVm.SystemId,
							   opt => opt.MapFrom(config => config.ClientSettings.ClientId))
					.ForMember(configVm => configVm.DB,
							   opt => opt.MapFrom(config => config.DBConnection.DataBase))
					.ForMember(configVm => configVm.AlarmDB,
							   opt => opt.MapFrom(config => config.DBConnection.Alarm.ConnectionString))
					.ForMember(configVm => configVm.NotificationDB,
							   opt => opt.MapFrom(config => config.DBConnection.Notify.ConnectionString))
					.ForMember(configVm => configVm.NotifyUrl,
							   opt => opt.MapFrom(config => config.ConnectionSettings.Notify.Url))
					.ForMember(configVm => configVm.AlarmUrl,
							   opt => opt.MapFrom(config => config.ConnectionSettings.Alarm.Url))
					.ForMember(configVm => configVm.UseCache,
							   opt => opt.MapFrom(config => config.ClientSettings.UseCache))
					.ForMember(configVm => configVm.Mode,
							   opt => opt.MapFrom(config => config.ClientSettings.Mode))
					.ForMember(configVm => configVm.ConsumerBootstrapServer,
							   opt => opt.MapFrom(config => config.Kafka.Consumer.BootstrapServers))
					.ForMember(configVm => configVm.ProducerBootstrapServer,
							   opt => opt.MapFrom(config => config.Kafka.Producer.BootstrapServers));
		}
	}
}
