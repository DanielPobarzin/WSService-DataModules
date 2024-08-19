using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using Domain.Settings.SignalRClient;
using MediatR;

namespace Application.Features.Configurations.Client.Commands.UpdateConfig
{
	public class UpdateConfigCLientCommandHandler : IRequestHandler<UpdateConfigClientCommand, Response<Guid>>
	{
		private readonly IClientConfigRepositoryAsync _repository;

		/// <summary>
		/// Initializes a new instance of the <see cref="UpdateConfigCLientCommandHandler"/> class.
		/// </summary>
		/// <param name="repository">The repository for working with clients.</param>
		public UpdateConfigCLientCommandHandler(IClientConfigRepositoryAsync repository)
		{
			_repository = repository;
		}

		/// <summary>
		/// Handles the client config update command.
		/// </summary>
		/// <param name="command">The update command containing the data to be updated.</param>
		/// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
		/// <returns>
		/// An object of type <see cref="Response{Guid}"/> containing the updated client config and the status of the operation.
		/// </returns>
		/// <exception cref="APIException">
		/// Thrown if a client config with the specified identifier is not found.
		/// </exception>
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

