using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using Domain.Settings.SignalRClient;
using MediatR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Application.Features.Configurations.Client.Commands.SendConfig
{
	public class SendConfigClientCommandHandler : IRequestHandler<SendConfigClientCommand, Response<Guid>>
	{
		private readonly IClientConfigRepositoryAsync _repository;
		private readonly IProducerService _producer;
		private readonly IConfiguration _configuration;
		private readonly CancellationToken _cancellingToken;
		private readonly string _topicProduce;
		public SendConfigClientCommandHandler(IClientConfigRepositoryAsync repository, IProducerService producer, IConfiguration configuration, CancellationToken cancellationToken)
		{
			_repository = repository;
			_producer = producer;
			_configuration = configuration;
			_cancellingToken = cancellationToken;
			_topicProduce = _configuration["Kafka:Topic"];
		}
		public async Task<Response<Guid>> Handle(SendConfigClientCommand command, CancellationToken cancellationToken)
		{
			var config = await _repository.GetByIdAsync(command.Id);
			if (config == null) throw new APIException($"Configuration Not Found.");
			var configDto = new ClientSettings
			{
				SystemId = config.SystemId,
				DBSettings = new DBSettings
				{
					DataBase = config.DBSettings.DataBase,
					Alarm = new AlarmDataBase
					{
						ConnectionString = config.DBSettings.Alarm.ConnectionString,
					},
					Notify = new NotifyDataBase
					{
						ConnectionString = config.DBSettings.Notify.ConnectionString,
					}
				},

				ConnectSettings = new ConnectSettings
				{
					Notify = new NotifyConnection
					{
						Url = config.ConnectSettings.Notify.Url,
					},
					Alarm = new AlarmConnection
					{
						Url = config.ConnectSettings.Alarm.Url,
					}
				},

				ModeSettings = new ModeSettings
				{
					UseCache = config.ModeSettings.UseCache,
					Mode = config.ModeSettings.Mode
				}
			};
			string json = JsonConvert.SerializeObject(configDto, Formatting.Indented);
			await _producer.ProduceMessageProcessAsync(_topicProduce, json);
			return new Response<Guid>(config.SystemId, true);
		}
	}
}
