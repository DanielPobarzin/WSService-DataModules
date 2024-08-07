using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using Domain.Settings.SignalRServer;
using MediatR;

namespace Application.Features.Configurations.Server.Commands.UpdateConfig
{
	public class UpdateConfigServerCommandHandler : IRequestHandler<UpdateConfigServerCommand, Response<Guid>>
	{
		private readonly IServerConfigRepositoryAsync _repository;
		private readonly IMapper _mapper;
		public UpdateConfigServerCommandHandler(IServerConfigRepositoryAsync repository, IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}
		public async Task<Response<Guid>> Handle(UpdateConfigServerCommand command, CancellationToken cancellationToken)
		{
			var config = await _repository.GetByIdAsync(command.SystemId);
			if (config == null) throw new APIException($"Config Not Found.");

			config.SystemId = command.SystemId;

			config.ServerDB = new DBSettings
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
			};

			config.ServerHost = new HostSettings
			{
				PolicyName = command.PolicyName,
				Port = command.Port,
				Urls = command.Urls,
				AllowedOrigins = command.AllowedOrigins,
				RouteNotify = command.RouteNotify,
				RouteAlarm = command.RouteAlarm
			};

			config.ServerHub = new HubSettings
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

			};

			await _repository.UpdateAsync(config);
			return new Response<Guid>(config.SystemId, true);
		}
	}
}
