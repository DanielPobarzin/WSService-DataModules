using Interactors.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactors.Settings.ClientConfig
{
	public class DbConnection
	{
		public string DataBase { get; set; }
		public AlarmDataBase Alarm { get; set; }
		public NotifyDataBase Notify { get; set; }
	}

	public class AlarmDataBase
	{
		public string ConnectionString { get; set; }
	}

	public class NotifyDataBase
	{
		public string ConnectionString { get; set; }
	}

	public class ConnectSettings
	{
		public Guid ClientId { get; set; }
		public NotifyConnection Notify { get; set; }
		public AlarmConnection Alarm { get; set; }
	}

	public class NotifyConnection
	{
		public string Url { get; set; }
	}

	public class AlarmConnection
	{
		public string Url { get; set; }
	}

	public class ModeSettings
	{
		public bool UseCache { get; set; }
		public ConnectionMode Mode { get; set; }
	}

}
