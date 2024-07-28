using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Settings.SignalRClient
{
	public class ClientSettings
	{
		public ConnectSettings ConnectSettings { get; set; }
		public DBSettings DBSettings { get; set; }
		public OtherSettings OtherSettings { get; set; }
	}
}
