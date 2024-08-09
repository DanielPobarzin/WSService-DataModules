﻿using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using Domain.Settings.SignalRClient;
using MediatR;

namespace Application.Features.Configurations.Client.Commands.UpdateConfig
{
	public class UpdateConfigCLientCommandHandler : IRequestHandler<UpdateConfigClientCommand, Response<Guid>>
	{
		private readonly IClientConfigRepositoryAsync _repository;
		private readonly IMapper _mapper;
		public UpdateConfigCLientCommandHandler(IClientConfigRepositoryAsync repository, IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}
		public async Task<Response<Guid>> Handle(UpdateConfigClientCommand command, CancellationToken cancellationToken)
		{
			var config = await _repository.GetByIdAsync(command.SystemId);
			if (config == null) throw new APIException($"Config Not Found.");
			
			config = new ClientSettings
			{
				SystemId = command.SystemId,
				DBSettings = new DBSettings
				{
					DataBase = command.DB,
					Alarm = new AlarmDataBase
					{
						ConnectionString = command.AlarmDB
					},
					Notify = new NotifyDataBase
					{
						ConnectionString = command.NotificationDB
					}
				},

				ConnectSettings = new ConnectSettings
				{
					Notify = new NotifyConnection
					{
						Url = command.NotifyUrl
					},
					Alarm = new AlarmConnection
					{
						Url = command.AlarmUrl
					}
				},

				ModeSettings = new ModeSettings
				{
					UseCache = command.UseCache,
					Mode = command.Mode
				}
			};
			
			await _repository.UpdateAsync(config);
			return new Response<Guid>(config.SystemId, true);
		}
	}
}
