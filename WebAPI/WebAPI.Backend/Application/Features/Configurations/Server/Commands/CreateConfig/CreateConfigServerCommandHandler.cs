using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Settings.SignalRServer;
using MediatR;

namespace Application.Features.Configurations.Server.Commands.CreateConfig
{
	public class CreateConfigServerCommandHandler : IRequestHandler<CreateConfigServerCommand, Response<Guid>>
	{
		private readonly IServerConfigRepositoryAsync _repository;
		public CreateConfigServerCommandHandler(IServerConfigRepositoryAsync repository)
		{
			_repository = repository;
		}
		public async Task<Response<Guid>> Handle(CreateConfigServerCommand command, CancellationToken cancellationToken)
		{
			var config = await _repository.GetByIdAsync(command.SystemId);
			if (config != null) throw new APIException($"Config has already been created.");

			config = new ServerSettings
			{
				SystemId = command.SystemId,

				ServerDB = new DBSettings
				{
					DB = command.DB,
					AlarmDB = new AlarmConnection
					{
						ConnectionString = command.AlarmDB
					},
					NotificationDB = new NotifyConnection
					{
						ConnectionString = command.NotificationDB
					}
				},

				ServerHost = new HostSettings
				{
					PolicyName = command.PolicyName,
					Port = command.Port,
					Urls = command.Urls,
					AllowedOrigins = command.AllowedOrigins,
					RouteNotify = command.RouteNotify,
					RouteAlarm = command.RouteAlarm
				},

				ServerHub = new HubSettings
				{
					ServerId = command.SystemId,

					Alarm = new AlarmHubSettings
					{
						DelayMilliseconds = command.AlarmDelayMilliseconds,
						HubMethod = command.AlarmHubMethod,
						TargetClients = command.AlarmTargetClients
					},

					Notify = new NotifyHubSettings
					{
						DelayMilliseconds = command.NotifyDelayMilliseconds,
						HubMethod = command.NotifyHubMethod,
						TargetClients = command.NotifyTargetClients
					}
				},
				ServerKafka = new KafkaSettings
				{
					Consumer = new ConsumerConnection
					{
						BootstrapServers = command.ConsumerBootstrapServer
					},
					Producer = new ProducerConnection
					{
						BootstrapServers = command.ProducerBootstrapServer
					}
				}
			};

			//var configDb = await _repository.GetByIdDataBaseSettingsAsync(command.SystemId);
			//var configDbDto = configDb.AsQueryable().ProjectTo<DBSettings>(_mapper.ConfigurationProvider);
			//var configHost = await _repository.GetByIdHostSettingsAsync(command.SystemId);
			//var configHostDto = configHost.AsQueryable().ProjectTo<DBSettings>(_mapper.ConfigurationProvider);
			//var configHub = await _repository.GetByIdHubSettingsAsync(command.SystemId);
			//var configHubDto = configHub.AsQueryable().ProjectTo<DBSettings>(_mapper.ConfigurationProvider);

			await _repository.AddAsync(config);
			return new Response<Guid>(config.SystemId, true);
		}
	}
}

