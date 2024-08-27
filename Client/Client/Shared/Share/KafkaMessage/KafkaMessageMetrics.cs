﻿using Entities.Enums;

namespace Shared.Share.KafkaMessage
{
	public class KafkaMessageMetrics : KafkaMessageBase
    {
		private KafkaMessageMetrics() { }
		private WorkStatus _workStatus;
		private ConnectionStatus _connectionStatus;
		private long _totalMessagesSize;
		private int _totalCountMessages;
		private int _countAlarms;
		private int _countNotifications;
		private long _averageMessageSize;
		private long _workingMemoryUsage;
		private long _privateMemoryUsage;
		private TimeSpan _latency;

		private static readonly Lazy<KafkaMessageMetrics> lazy =
		new Lazy<KafkaMessageMetrics>(() => new KafkaMessageMetrics());
		public static KafkaMessageMetrics Instance { get { return lazy.Value; } }

		public WorkStatus WorkStatus
		{
			get { return _workStatus; }
			set { _workStatus = value; }
		}
		public ConnectionStatus ConnectionStatus
		{
			get { return _connectionStatus; }
			set { _connectionStatus = value; }
		}
		public long TotalMessagesSize
		{
			get { return _totalMessagesSize; }
			set	{ _totalMessagesSize = value; }
		}
		public int TotalCountMessages
		{
			get { return _totalCountMessages; }
			set { _totalCountMessages = value; }
		}
		public int CountAlarms
		{
			get { return _countAlarms; }
			set { _countAlarms = value; }
		}
		public int CountNotifications
		{
			get { return _countNotifications; }
			set { _countNotifications = value; }
		}
		public long AverageMessageSize
		{
			get { return _averageMessageSize; }
			set { _averageMessageSize = _totalMessagesSize / _totalCountMessages; }
		}
		public long WorkingMemoryUsage
		{
			get { return _workingMemoryUsage; }
			set { _workingMemoryUsage = value; }
		}
		public long PrivateMemoryUsage
		{
			get { return _privateMemoryUsage; }
			set { _privateMemoryUsage = value; }
		}

		public TimeSpan Latency
		{
			get { return _latency; }
			set { _latency = value; }
		}
	}
}