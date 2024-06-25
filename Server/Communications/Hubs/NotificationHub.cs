using Communications.Connections;
using Communications.DTO;
using Entities.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communications.Hubs
{
	public class NotificationHub : Hub
	{
		private Connections<NotificationHub> connection;

		public NotificationHub(Connections<NotificationHub> connection) 
		{
			this.connection = connection;
		}
		public override async Task OnConnectedAsync()
		{
			this.connection.All.TryAdd(Context.ConnectionId, Context);
			Log.Information("New connection: {@userId}", Context.ConnectionId);
			await Groups.AddToGroupAsync(Context.ConnectionId, "User");
			await Clients.Others.SendAsync("Notify", $"{Context.ConnectionId} is connected.");
			await Clients.Others.SendAsync("Notify", $"{Context.ConnectionId} joined the User group.");
			await Clients.Caller.SendAsync("Notify", $"You have joined the User group.");
			await base.OnConnectedAsync();
		}
		public override async Task OnDisconnectedAsync(Exception exception)
		{
			this.connection.All.TryRemove(Context.ConnectionId, out _);
			Log.Information("Disconnecting: {ConnectionId}", Context.ConnectionId);
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, "User");
			await Clients.Others.SendAsync("Notify", $"{Context.ConnectionId} is disconnected.");
			await base.OnDisconnectedAsync(exception);
		}

		public async Task OnReconnectedAsync()
		{
			Log.Information("Reconnecting: {ConnectionId}", Context.ConnectionId);
			await Clients.Others.SendAsync("Notify", $"{Context.ConnectionId} is reconnected.");
		}
	}
}