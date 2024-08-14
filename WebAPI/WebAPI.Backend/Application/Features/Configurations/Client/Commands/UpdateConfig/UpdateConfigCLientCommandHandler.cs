using Application.Exceptions;
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

			config.SystemId = command.SystemId;
			config.DBSettings = new DBSettings
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
			};

			config.ConnectSettings = new ConnectSettings
			{
				Notify = new NotifyConnection
				{
					Url = command.NotifyUrl
				},
				Alarm = new AlarmConnection
				{
					Url = command.AlarmUrl
				}
			};

			config.ModeSettings = new ModeSettings
			{
				UseCache = command.UseCache,
				Mode = command.Mode
			};

			config.KafkaSettings = new KafkaSettings
			{
				Consumer = new ConsumerConnection
				{
					BootstrapServers = command.ConsumerBootstrapServer
				},
				Producer = new ProducerConnection
				{
					BootstrapServers = command.ProducerBootstrapServer
				}
			};
			await _repository.UpdateAsync(config);
			return new Response<Guid>(config.SystemId, true);
		}
	}
}

