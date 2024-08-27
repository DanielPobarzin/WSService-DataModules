using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using Domain.Settings.SignalRServer;
using MediatR;

namespace Application.Features.Configurations.Server.Commands.UpdateConfig
{
	public class UpdateConfigServerCommandHandler : IRequestHandler<UpdateConfigServerCommand, Response<Guid>>
	{
		private readonly IServerConfigRepositoryAsync _repository;
		public UpdateConfigServerCommandHandler(IServerConfigRepositoryAsync repository)
		{
			_repository = repository;
		}

		/// <summary>
		/// Handles the updating of a server configuration.
		/// </summary>
		/// <param name="command">The command containing the details for updating the server configuration.</param>
		/// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{Guid}"/> with the ID of the updated server configuration.</returns>
		/// <exception cref="APIException">Thrown when the specified configuration is not found.</exception>
		public async Task<Response<Guid>> Handle(UpdateConfigServerCommand command, CancellationToken cancellationToken)
		{
			var config = await _repository.GetByIdAsync(command.SystemId) ?? throw new APIException($"Config Not Found.");

			config.DbConnection = new DBSettings
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
			};

			config.HostSettings = new HostSettings
			{
				PolicyName = command.PolicyName,
				Port = command.Port,
				Urls = command.Urls,
				AllowedOrigins = command.AllowedOrigins,
				RouteNotify = command.RouteNotify,
				RouteAlarm = command.RouteAlarm
			};

			config.HubSettings = new HubSettings
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

			config.Kafka = new KafkaSettings
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
			return new Response<Guid>(config.HubSettings.ServerId, true);
		}
	}
}
