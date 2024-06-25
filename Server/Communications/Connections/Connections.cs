using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communications.Connections
{
	public class Connections<T> where T : Hub
	{
		public ConcurrentDictionary<string, HubCallerContext> All { get; } = new();
	}
}
