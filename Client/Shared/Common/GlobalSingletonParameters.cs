
using Interactors.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Common
{
	public class GlobalSingletonParameters
	{
		private GlobalSingletonParameters() { }
		private ConnectionCommand _connectionCommand;
		private ConnectionMode _connectionMode;

		private static readonly Lazy<GlobalSingletonParameters> lazy =
		new Lazy<GlobalSingletonParameters>(() => new GlobalSingletonParameters());
		public static GlobalSingletonParameters Instance { get { return lazy.Value; } }

		public event EventHandler<ConnectionCommand> ConnectionCommandChanged;

		public ConnectionCommand ConnectionCommand
		{
			get { return _connectionCommand; }
			set
			{
				if (_connectionCommand != value)
				{
					_connectionCommand = value;
					OnConnectionCommandChanged(_connectionCommand);
				}
			}
		}
		public ConnectionMode ConnectionMode
		{
			get { return _connectionMode; }
			set
			{
				if (_connectionMode != value)
				{
					_connectionMode = value;
				}
			}
		}
		protected virtual void OnConnectionCommandChanged(ConnectionCommand newCommand)
		{
			ConnectionCommandChanged?.Invoke(this, newCommand);
		}
	}
}
