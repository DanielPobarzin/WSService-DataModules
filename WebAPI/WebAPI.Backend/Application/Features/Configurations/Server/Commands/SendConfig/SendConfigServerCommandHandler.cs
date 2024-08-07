using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using Domain.Settings.SignalRServer;
using MediatR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Application.Features.Configurations.Server.Commands.SendConfig
{
	public class SendConfigServerCommandHandler : IRequestHandler<SendConfigServerCommand, Response<Guid>>
	{
		private readonly IServerConfigRepositoryAsync _repository;
		private readonly IProducerService _producer;
		private readonly IConfiguration _configuration;
		private readonly CancellationToken _cancellingToken;
		private readonly string _topicProduce;
		public SendConfigServerCommandHandler(IServerConfigRepositoryAsync repository, IProducerService producer, IConfiguration configuration, CancellationToken cancellationToken)
		{
			_repository = repository;
			_producer = producer;
			_configuration = configuration;
			_cancellingToken = cancellationToken;
			_topicProduce = _configuration["Kafka:Topic"];
		}
		public async Task<Response<Guid>> Handle(SendConfigServerCommand command, CancellationToken cancellationToken)
		{
			var config = await _repository.GetByIdAsync(command.Id);
			if (config == null) throw new APIException($"Configuration Not Found.");
			var configDto = new ServerSettings
			{
				SystemId = config.SystemId,

				ServerDB = new DBSettings
				{
					DB = config.ServerDB.DB,

					AlarmDB = new AlarmConnection
					{
						ConnectionString = config.ServerDB.AlarmDB.ConnectionString,
					},
					NotificationDB = new NotifyConnection
					{
						ConnectionString = config.ServerDB.NotificationDB.ConnectionString,
					}
				},

				ServerHub = new HubSettings
				{
					ServerId = config.SystemId,

					Alarm = new AlarmHubSettings
					{
						DelayMilliseconds = config.ServerHub.Alarm.DelayMilliseconds,
						HubMethod = config.ServerHub.Alarm.HubMethod,
						TargetClients = config.ServerHub.Alarm.TargetClients
					},

					Notify = new NotifyHubSettings
					{
						DelayMilliseconds = config.ServerHub.Notify.DelayMilliseconds,
						HubMethod = config.ServerHub.Notify.HubMethod,
						TargetClients = config.ServerHub.Notify.TargetClients
					}
				},

				ServerHost = new HostSettings
				{
					PolicyName = config.ServerHost.PolicyName,
					Port = config.ServerHost.Port,
					Urls = config.ServerHost.Urls,
					AllowedOrigins = config.ServerHost.AllowedOrigins,
					RouteNotify = config.ServerHost.RouteNotify,
					RouteAlarm = config.ServerHost.RouteAlarm
				}
			};
			string json = JsonConvert.SerializeObject(configDto, Formatting.Indented);
			await _producer.ProduceMessageProcessAsync(_topicProduce, json);
			return new Response<Guid>(config.SystemId, true);
		}
	}
}

