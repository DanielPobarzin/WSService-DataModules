﻿using Application.Mappings;
using AutoMapper; 
using Domain.Settings.SignalRServer;

namespace Application.Features.Configurations.Server.Queries.GetConfigServerDetails
{
	public class ServerConfigDetailsViewModel : IMapWith<ServerSettings>
	{
		public Guid SystemId { get; set; }
		public int Port { get; set; }
		public string Urls { get; set; }
		public string PolicyName { get; set; }
		public string AllowedOrigins { get; set; }
		public string RouteNotify { get; set; }
		public string RouteAlarm { get; set; }
		public int AlarmDelayMilliseconds { get; set; }
		public int NotifyDelayMilliseconds { get; set; }
		public string NotifyHubMethod { get; set; }
		public string AlarmHubMethod { get; set; }
		public string NotifyTargetClients { get; set; }
		public string AlarmTargetClients { get; set; }
		public string DB { get; set; }
		public string AlarmDB { get; set; }
		public string NotificationDB { get; set; }
		public string ConsumerBootstrapServer { get; set; }
		public string ProducerBootstrapServer { get; set; }

		public void Mapping(Profile profile)
		{

			profile.CreateMap<ServerSettings, ServerConfigDetailsViewModel>()
					.ForMember(configVm => configVm.SystemId,
							   opt => opt.MapFrom(config => config.SystemId))
					.ForMember(configVm => configVm.Port,
							   opt => opt.MapFrom(config => config.ServerHost.Port))
					.ForMember(configVm => configVm.Urls,
							   opt => opt.MapFrom(config => config.ServerHost.Urls))
					.ForMember(configVm => configVm.PolicyName,
							   opt => opt.MapFrom(config => config.ServerHost.PolicyName))
					.ForMember(configVm => configVm.AllowedOrigins,
							   opt => opt.MapFrom(config => config.ServerHost.AllowedOrigins))
					.ForMember(configVm => configVm.RouteNotify,
							   opt => opt.MapFrom(config => config.ServerHost.RouteNotify))
					.ForMember(configVm => configVm.RouteAlarm,
							   opt => opt.MapFrom(config => config.ServerHost.RouteAlarm))
					.ForMember(configVm => configVm.AlarmDelayMilliseconds,
							   opt => opt.MapFrom(config => config.ServerHub.Alarm.DelayMilliseconds))
					.ForMember(configVm => configVm.NotifyDelayMilliseconds,
							   opt => opt.MapFrom(config => config.ServerHub.Notify.DelayMilliseconds))
					.ForMember(configVm => configVm.NotifyHubMethod,
							   opt => opt.MapFrom(config => config.ServerHub.Notify.HubMethod))
					.ForMember(configVm => configVm.AlarmHubMethod,
							   opt => opt.MapFrom(config => config.ServerHub.Alarm.HubMethod))
					.ForMember(configVm => configVm.NotifyTargetClients,
							   opt => opt.MapFrom(config => config.ServerHub.Notify.TargetClients))
					.ForMember(configVm => configVm.AlarmTargetClients,
							   opt => opt.MapFrom(config => config.ServerHub.Alarm.TargetClients))
					.ForMember(configVm => configVm.DB,
							   opt => opt.MapFrom(config => config.ServerDB.DB))
					.ForMember(configVm => configVm.AlarmDB,
							   opt => opt.MapFrom(config => config.ServerDB.AlarmDB))
					.ForMember(configVm => configVm.NotificationDB,
							   opt => opt.MapFrom(config => config.ServerDB.NotificationDB))
					.ForMember(configVm => configVm.ConsumerBootstrapServer,
							   opt => opt.MapFrom(config => config.ServerKafka.Consumer.BootstrapServers))
					.ForMember(configVm => configVm.ProducerBootstrapServer,
							   opt => opt.MapFrom(config => config.ServerKafka.Producer.BootstrapServers));
		}
	}
}
