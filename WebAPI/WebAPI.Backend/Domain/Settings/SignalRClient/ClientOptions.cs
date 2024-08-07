﻿using Domain.Common;
using Domain.Entities;
using Domain.Enums;

namespace Domain.Settings.SignalRClient
{
	public class DBSettings : BaseConfig
	{
		public string DataBase { get; set; }
		public AlarmDataBase Alarm { get; set; }
		public NotifyDataBase Notify { get; set; }
	}

	public class AlarmDataBase : Entity
	{
		public string ConnectionString { get; set; }
	}

	public class NotifyDataBase : Entity
	{
		public string ConnectionString { get; set; }
	}

	public class ConnectSettings : BaseConfig
	{
		public Guid ClientId { get; set; }
		public NotifyConnection Notify { get; set; }
		public AlarmConnection Alarm { get; set; }
	}

	public class NotifyConnection : Entity
	{
		public string Url { get; set; }
	}

	public class AlarmConnection : Entity
	{
		public string Url { get; set; }
	}

	public class ModeSettings : BaseConfig
	{
		public bool UseCache { get; set; }
		public ConnectionMode Mode { get; set; }
	}
}
