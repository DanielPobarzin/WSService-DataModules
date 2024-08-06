using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using Domain.Settings.SignalRServer;
using MediatR;

namespace Application.Features.Configurations.Server.Commands.CreateConfig
{
	public class CreateConfigServerCommandHandler : IRequestHandler<CreateConfigServerCommand, Response<Guid>>
	{
		private readonly IServerConfigRepositoryAsync _repository;
		private readonly IMapper _mapper;
		public CreateConfigServerCommandHandler(IServerConfigRepositoryAsync repository, IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}
		public async Task<Response<Guid>> Handle(CreateConfigServerCommand command, CancellationToken cancellationToken)
		{
			var config = await _repository.GetByIdAsync(command.SystemId);
			if (config != null) throw new APIException($"Config has already been created.");

			config = new ServerSettings
			{
				ServerDB = new DBSettings
				{
					DB = command.DB,
					AlarmDB = command.AlarmDB,
					NotificationDB = command.NotificationDB
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
					AlarmDelayMilliseconds = command.AlarmDelayMilliseconds,
					NotifyDelayMilliseconds = command.NotifyDelayMilliseconds,
					NotifyHubMethod = command.NotifyHubMethod,
					AlarmHubMethod = command.NotifyHubMethod,
					NotifyTargetClients = command.NotifyTargetClients,
					AlarmTargetClients = command.AlarmTargetClients

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
