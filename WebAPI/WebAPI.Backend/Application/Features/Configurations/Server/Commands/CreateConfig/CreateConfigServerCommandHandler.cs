using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using Domain.Settings.SignalRServer;
using MediatR;

namespace Application.Features.Configurations.Server.Commands.CreateConfig
{
	public class CreateConfigServerCommandHandler : IRequestHandler<CreateConfigServerCommand, Response<Guid>>
	{
		private readonly IServerConfigRepositoryAsync _repository;

		/// <summary>
		/// Initializes a new instance of the <see cref="CreateConfigServerCommandHandler"/> class.
		/// </summary>
		/// <param name="repository">The repository used for accessing server configurations.</param>
		public CreateConfigServerCommandHandler(IServerConfigRepositoryAsync repository)
		{
			_repository = repository;
		}

		/// <summary>
		/// Handles the creation of a new server configuration.
		/// </summary>
		/// <param name="command">The command containing the details for creating the server configuration.</param>
		/// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{Guid}"/> with the ID of the created server configuration.</returns>
		/// <exception cref="APIException">Thrown when the configuration has already been created.</exception>
		public async Task<Response<Guid>> Handle(CreateConfigServerCommand command, CancellationToken cancellationToken)
		{
			var config = await _repository.GetByIdAsync(command.SystemId);
			if (config != null) throw new APIException($"Config has already been created.");

			config = new ServerSettings
			{
				DbConnection = new DBSettings
				{
					DataBase = command.DB,
					Alarm = new AlarmConnection
					{
						ConnectionString = command.AlarmDB
					},
					Notify = new NotifyConnection
					{
						ConnectionString = command.NotificationDB
					}
				},

				HubSettings = new HubSettings
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
				HostSettings = new HostSettings
				{
					PolicyName = command.PolicyName,
					Port = command.Port,
					Urls = command.Urls,
					AllowedOrigins = command.AllowedOrigins,
					RouteNotify = command.RouteNotify,
					RouteAlarm = command.RouteAlarm
				},

				
				Kafka = new KafkaSettings
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
			await _repository.AddAsync(config);
			return new Response<Guid>(config.HubSettings.ServerId, true);
		}
	}
}

