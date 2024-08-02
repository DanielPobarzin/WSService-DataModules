
using Interactors.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Common
{
	/// <summary>
	/// Represents a singleton class that holds global parameters for connection commands and modes.
	/// </summary>
	public class GlobalSingletonParameters
	{
		// Private constructor to prevent instantiation from outside.
		private GlobalSingletonParameters() { }

		private ConnectionCommand _connectionCommand;
		private ConnectionMode _connectionMode;

		private static readonly Lazy<GlobalSingletonParameters> lazy =
			new Lazy<GlobalSingletonParameters>(() => new GlobalSingletonParameters());

		/// <summary>
		/// Gets the singleton instance of the <see cref="GlobalSingletonParameters"/> class.
		/// </summary>
		public static GlobalSingletonParameters Instance { get { return lazy.Value; } }

		/// <summary>
		/// Occurs when the property changes.
		/// </summary>
		public event EventHandler<ConnectionCommand> ConnectionCommandChanged;

		/// <summary>
		/// Gets or sets the current connection command.
		/// </summary>
		/// <value>
		/// The current <see cref="ConnectionCommand"/>.
		/// </value>
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

		/// <summary>
		/// Gets or sets the current connection mode.
		/// </summary>
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

		/// <summary>
		/// Raises the <see cref="ConnectionCommandChanged"/> event.
		/// </summary>
		/// <param name="newCommand">The new connection command that has been set.</param>
		protected virtual void OnConnectionCommandChanged(ConnectionCommand newCommand)
		{
			ConnectionCommandChanged?.Invoke(this, newCommand);
		}
	}

}
