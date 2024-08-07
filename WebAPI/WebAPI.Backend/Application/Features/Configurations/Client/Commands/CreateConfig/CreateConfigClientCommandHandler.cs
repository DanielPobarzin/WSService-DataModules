using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using Domain.Settings.SignalRClient;
using MediatR;

namespace Application.Features.Configurations.Client.Commands.CreateConfig
{
	public class CreateConfigClientCommandHandler : IRequestHandler<CreateConfigClientCommand, Response<Guid>>
	{
		private readonly IClientConfigRepositoryAsync _repository;
		private readonly IMapper _mapper;
		public CreateConfigClientCommandHandler(IClientConfigRepositoryAsync repository, IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}
		public async Task<Response<Guid>> Handle(CreateConfigClientCommand command, CancellationToken cancellationToken)
		{
			var config = await _repository.GetByIdAsync(command.SystemId);
			if (config != null) throw new APIException($"Config has already been created.");

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
			await _repository.AddAsync(config);
			return new Response<Guid>(config.SystemId, true);
		}
	}
}
