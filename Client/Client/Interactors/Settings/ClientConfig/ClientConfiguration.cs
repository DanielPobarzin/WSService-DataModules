using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactors.Settings.ClientConfig
{
	public class ClientConfiguration
	{
		public DbConnection DbConnection { get; set; }
		public ConnectSettings ConnectSettings { get; set; }
		public ModeSettings ModeSettings { get; set; }
	}
}
