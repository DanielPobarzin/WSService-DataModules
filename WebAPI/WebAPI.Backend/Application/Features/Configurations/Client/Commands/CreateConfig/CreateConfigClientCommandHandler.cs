using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using Domain.Settings.SignalRClient;
using MediatR;

namespace Application.Features.Configurations.Client.Commands.CreateConfig
{
	public class CreateConfigClientCommandHandler : IRequestHandler<CreateConfigClientCommand, Response<Guid>>
	{
		private readonly IClientConfigRepositoryAsync _repository;

		/// <summary>
		/// Constructor for CreateConfigClientCommandHandler class. 
		/// Initializes a new instance of the <see cref="CreateConfigClientCommandHandler"/> class.
		/// </summary>
		/// <param name="repository">The repository interface for working with clients.</param>
		public CreateConfigClientCommandHandler(IClientConfigRepositoryAsync repository)
		{
			_repository = repository;
		}

		/// <summary>
		/// Handles the command to create a new client config.
		/// </summary>
		/// <param name="command">The command containing client configuration data to be added.</param>
		/// <param name="cancellationToken">Cancellation token for the asynchronous operation.</param>
		/// <returns>
		/// An instance of <see cref="Response{Guid}"/> containing the added client configuration and the operation status.
		/// </returns>
		/// <exception cref="APIException">Thrown if a client configuration with the specified ID already exists.</exception>
		public async Task<Response<Guid>> Handle(CreateConfigClientCommand command, CancellationToken cancellationToken)
		{
			var config = await _repository.GetByIdAsync(command.SystemId);
			if (config != null) throw new APIException($"Config has already been created.");

			var newconfig = new ClientSettings();
			newconfig.SystemId = command.SystemId;

			newconfig.DBSettings = new DBSettings
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

			newconfig.ModeSettings = new ModeSettings
			{
				ClientId = newconfig.SystemId,
				UseCache = command.UseCache,
				Mode = command.Mode
			};

			newconfig.ConnectSettings = new ConnectSettings
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

			newconfig.KafkaSettings = new KafkaSettings
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
			await _repository.AddAsync(newconfig);
			return new Response<Guid>(newconfig.SystemId, true);
		}
	}
}
