using Communications.Connections;
using Communications.Helpers;
using Entities.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Communications.Hubs
{
	public class AlarmHub : Hub
	{
		private Connections<AlarmHub> connections;
		private readonly List<Alarm>? _alarms;
		private readonly IConfiguration _configuration;
		private IMemoryCache memoryCache;
		private TransformToDTOHelper transformToDTOHelper;
		private JsonCacheHelper jsonCacheHelper;

		public AlarmHub(List<Alarm>? alarms,
		   Connections<AlarmHub> connections,
		   TransformToDTOHelper transformToDTOHelper,
		   JsonCacheHelper jsonCacheHelper,
		   IConfiguration configuration,
		   IMemoryCache memoryCache)
		{
			_alarms = alarms;
			_configuration = configuration;
			this.memoryCache = memoryCache;
			this.connections = connections;
			this.transformToDTOHelper = transformToDTOHelper;
			this.jsonCacheHelper = jsonCacheHelper;
		}

		public async Task Send(Guid clientId)
		{
			var cacheAlarm = await jsonCacheHelper.ReadFromFileCache<Alarm>(clientId);
			if (cacheAlarm.Any() && bool.Parse(_configuration["HubSettings:Alarm:UseCache"]))
			{
				foreach (var alarm in cacheAlarm)
					memoryCache.Set($"{Context.ConnectionId}_{alarm.Id}", alarm);
			}
			while (connections.GetConnection(Context.ConnectionId) != null)
			{
				try
				{
					foreach (var alarm in _alarms)
					{
						switch (bool.Parse(_configuration["HubSettings:Alarm:UseCache"]))
						{
							case true:

								var alarmDTO = await transformToDTOHelper.TransformToAlarmDTO(alarm, Guid.Parse(_configuration["HubSettings:ServerId"]));
								memoryCache.TryGetValue($"{Context.ConnectionId}_{alarm.Id}", out Alarm? cachedAlarm);

								if (cachedAlarm == null)
								{
									await Clients.Client(Context.ConnectionId).SendAsync(_configuration["HubSettings:Notify:HubMethod"], alarmDTO);
									memoryCache.Set($"{Context.ConnectionId}_{alarm.Id}", alarm);

									Log.Information($"Alarm {alarmDTO.Signal.Id} has been sent."
												+ "\nSender:\t\t" + $" Server - {alarmDTO.ServerId}"
												+ "\nRecipient:\t" + $" Client - {clientId}");
								}
							break;

							case false:

								alarmDTO = await transformToDTOHelper.TransformToAlarmDTO(alarm, Guid.Parse(_configuration["HubSettings:ServerId"]));
								await Clients.Client(Context.ConnectionId).SendAsync(_configuration["HubSettings:Notify:HubMethod"], alarmDTO);

								Log.Information($"Alarm {alarmDTO.Signal.Id} has been sent."
											+ "\nSender:\t\t" + $" Server - {alarmDTO.ServerId}"
											+ "\nRecipient:\t" + $" Client - {clientId}");
							break;
						}
					}
				}
				catch (Exception ex)
				{
					Log.Error($"Exception with data: {ex.Message}");
				}
				await Task.Delay(Convert.ToInt32(_configuration["HubSettings:Alarm:DelayMilliseconds"]));
			}
			await jsonCacheHelper.WriteToFileCache(_alarms, clientId);
		}
		public override async Task OnConnectedAsync()
		{
			connections.AddConnection(Context.ConnectionId, Context.ConnectionId);
			Log.Information("New connection: {@userId}", Context.ConnectionId);
			await Groups.AddToGroupAsync(Context.ConnectionId, "Alarm");
			await Clients.Others.SendAsync("Notify", $"{Context.ConnectionId} is connected. Type of message : alarm.");
			await Clients.Others.SendAsync("Notify", $"{Context.ConnectionId} joined the Alarm group.");
			await Clients.Client(Context.ConnectionId).SendAsync("Notify", $"You have joined the Alarm group.");
			await base.OnConnectedAsync();
		}

		public override async Task OnDisconnectedAsync(Exception exception)
		{
			connections.RemoveConnection(Context.ConnectionId);
			Log.Information("Disconnecting: {ConnectionId}", Context.ConnectionId);
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, "User");
			await Clients.Others.SendAsync("Notify", $"{Context.ConnectionId} is disconnected from the alarm hub.");
			await base.OnDisconnectedAsync(exception);
		}

		public async Task OnReconnectedAsync(Guid clientId)
		{
			Log.Information($"Reconnecting client {clientId}: {Context.ConnectionId}");
			await Clients.Others.SendAsync("Notify", $"{Context.ConnectionId} is reconnected.");
			await Send(clientId);
		}

	}
}
	