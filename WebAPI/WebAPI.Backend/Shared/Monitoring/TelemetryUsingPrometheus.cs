using System.Diagnostics.Metrics;


namespace Shared.Monitoring
{
	public class TelemetryClientUsingPrometheus
	{
		private readonly Meter _meter;
		private Histogram<long> TotalMessagesSize { get; }
		private Counter<int> TotalCountMessages { get; }
		private Counter<int> TotalCountAlarms { get; }
		private Counter<int> TotalCountNotifications { get; }
		private Histogram<long> Latency { get; }
		private Histogram<long> WorkingMemoryUsage { get; }
		private Histogram<long> PrivateMemoryUsage { get; }
		private ObservableGauge<long> AverageMessageSize { get; }
		private long _averageMessageSize;
		public TelemetryClientUsingPrometheus(Meter meter)
		{
			_meter = meter;
			TotalMessagesSize = _meter.CreateHistogram<long>("TotalMessagesSize", "byte", "The total size of messages that clients have received");
			TotalCountMessages = _meter.CreateCounter<int>("TotalCountMessages", "The total number of messages that clients have received");
			TotalCountAlarms = _meter.CreateCounter<int>("TotalCountAlarms", "The total number of 'Alarm' type messages that clients have received");
			TotalCountNotifications = _meter.CreateCounter<int>("TotalCountNotifications", "The total number of 'Notification' type messages that clients have received");
			Latency = _meter.CreateHistogram<long>("Latency", "Time", "Еhe time delay between sending a message by the server and receiving it by the client");
			WorkingMemoryUsage = _meter.CreateHistogram<long>("WorkingMemoryUsage", "byte", "The use of working memory by the client");
			PrivateMemoryUsage = _meter.CreateHistogram<long>("PrivateMemoryUsage", "byte", "The use of рrivate memory by the client");
			AverageMessageSize = _meter.CreateObservableGauge<long>("AverageMessageSize", () => new[] { new Measurement<long>(_averageMessageSize) }, "byte", "The use of рrivate memory by the client");
		}

		public void AddTotalCountMessages(int countMessage, Guid Id) => TotalCountMessages.Add(countMessage, KeyValuePair.Create<string, object?>("ClientId", Id));
		public void AddTotalCountAlarms(int countMessage, Guid Id) => TotalCountAlarms.Add(countMessage, KeyValuePair.Create<string, object?>("ClientId", Id));
		public void AddTotalCountNotifications(int countMessage, Guid Id) => TotalCountNotifications.Add(countMessage, KeyValuePair.Create<string, object?>("ClientId", Id));
		public void RecordTotalMessagesSize(long totalSize) => TotalMessagesSize.Record(totalSize);
		public void RecordLatency(TimeSpan latency) => Latency.Record((long)latency.TotalMilliseconds);
		public void RecordWorkingMemoryUsage(long memory) => WorkingMemoryUsage.Record(memory);
		public void RecordPrivateMemoryUsage(long memory) => PrivateMemoryUsage.Record(memory);
		public void ChangeAverageMessageSize(long averageMessageSize) => _averageMessageSize = averageMessageSize;
	}

	public class TelemetryServerUsingPrometheus
	{
		private readonly Meter _meter;
		public ObservableGauge<int> CountListeners { get; set; }
		private Histogram<long> TotalMessagesSize { get; }
		private Counter<int> TotalCountMessages { get; }
		private Counter<int> TotalCountAlarms { get; }
		private Counter<int> TotalCountNotifications { get; }
		private Histogram<long> Latency { get; }
		private Histogram<long> WorkingMemoryUsage { get; }
		private Histogram<long> PrivateMemoryUsage { get; }
		private ObservableGauge<long> AverageMessageSize { get; }
		private long _averageMessageSize;
		private int _countListeners;
		public TelemetryServerUsingPrometheus(Meter meter)
		{
			_meter = meter;
			TotalMessagesSize = _meter.CreateHistogram<long>("TotalMessagesSize", "byte", "The total size of messages that Server have received");
			TotalCountMessages = _meter.CreateCounter<int>("TotalCountMessages", "The total number of messages that Server have received");
			TotalCountAlarms = _meter.CreateCounter<int>("TotalCountAlarms", "The total number of 'Alarm' type messages that Server have received");
			TotalCountNotifications = _meter.CreateCounter<int>("TotalCountNotifications", "The total number of 'Notification' type messages that Server have sent");
			Latency = _meter.CreateHistogram<long>("Latency", "Time", "The time delay between receiving a message by the server and sending it");
			WorkingMemoryUsage = _meter.CreateHistogram<long>("WorkingMemoryUsage", "byte", "The use of working memory by the Server");
			PrivateMemoryUsage = _meter.CreateHistogram<long>("PrivateMemoryUsage", "byte", "The use of рrivate memory by the Server");
			AverageMessageSize = _meter.CreateObservableGauge<long>("AverageMessageSize", () => new[] { new Measurement<long>(_averageMessageSize) }, "byte", "The use of рrivate memory by the Server");
			CountListeners = _meter.CreateObservableGauge<int>("CountListeners", () => new[] { new Measurement<int>(_countListeners) }, "Number of connections to the server");
		}

		public void AddTotalCountMessages(int countMessage, Guid Id) => TotalCountMessages.Add(countMessage, KeyValuePair.Create<string, object?>("ServerId", Id));
		public void AddTotalCountAlarms(int countMessage, Guid Id) => TotalCountAlarms.Add(countMessage, KeyValuePair.Create<string, object?>("ServerId", Id));
		public void AddTotalCountNotifications(int countMessage, Guid Id) => TotalCountNotifications.Add(countMessage, KeyValuePair.Create<string, object?>("ServerId", Id));
		public void RecordTotalMessagesSize(long totalSize) => TotalMessagesSize.Record(totalSize);
		public void RecordLatency(TimeSpan latency) => Latency.Record((long)latency.TotalMilliseconds);
		public void RecordWorkingMemoryUsage(long memory) => WorkingMemoryUsage.Record(memory);
		public void RecordPrivateMemoryUsage(long memory) => PrivateMemoryUsage.Record(memory);
		public void ChangeAverageMessageSize(long averageMessageSize) => _averageMessageSize = averageMessageSize;
		public void ChangeCountListeners(int CountListeners) => _countListeners = CountListeners;
	}
}



