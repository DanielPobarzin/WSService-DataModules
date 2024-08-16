using Domain.Enums;
using Prometheus;
using System.Diagnostics.Metrics;


namespace Application.Parameters.ShareMetrics
{
	public class TelemetryClientUsingPrometheus
	{
		private readonly Meter _meter;
		private Histogram<long> TotalMessagesSize {  get; }
		private Counter<int> TotalCountMessages { get; }
		private Counter<int> TotalCountAlarms { get; }
		private Counter<int> TotalCountNotifications { get; }
		private Histogram<TimeSpan> Latency { get; }
		private Histogram<long> WorkingMemoryUsage { get; }
		private Histogram<long> PrivateMemoryUsage { get; }
		private ObservableGauge<long> AverageMessageSize { get; }
		private long _averageMessageSize;
		public TelemetryClientUsingPrometheus(Meter meter)
		{
			_meter = meter;
			TotalMessagesSize = _meter.CreateHistogram<long>("TotalMessagesSize","byte", "The total size of messages that clients have received");
			TotalCountMessages = _meter.CreateCounter<int>("TotalCountMessages", "The total number of messages that clients have received");
			TotalCountAlarms = _meter.CreateCounter<int>("TotalCountAlarms", "The total number of 'Alarm' type messages that clients have received");
			TotalCountNotifications = _meter.CreateCounter<int>("TotalCountNotifications", "The total number of 'Notification' type messages that clients have received");
			Latency = _meter.CreateHistogram<TimeSpan>("Latency", "Time", "Еhe time delay between sending a message by the server and receiving it by the client");
			WorkingMemoryUsage = _meter.CreateHistogram<long>("WorkingMemoryUsage", "byte", "The use of working memory by the client");
			PrivateMemoryUsage = _meter.CreateHistogram<long>("PrivateMemoryUsage", "byte", "The use of рrivate memory by the client");
			AverageMessageSize = _meter.CreateObservableGauge<long>("AverageMessageSize", () => new[] { new Measurement<long>(_averageMessageSize) }, "byte", "The use of рrivate memory by the client");
		}

		public void AddTotalCountMessages(int countMessage, Guid Id) => TotalCountMessages.Add(countMessage, KeyValuePair.Create<string, object?>("ClientId", Id));
		public void AddTotalCountAlarms(int countMessage, Guid Id) => TotalCountAlarms.Add(countMessage, KeyValuePair.Create<string, object?>("ClientId", Id));
		public void AddTotalCountNotifications(int countMessage, Guid Id) => TotalCountAlarms.Add(countMessage, KeyValuePair.Create<string, object?>("ClientId", Id));
		public void RecordTotalMessagesSize(long totalSize) => TotalMessagesSize.Record(totalSize);
		public void RecordLatency(TimeSpan latency) => Latency.Record(latency);
		public void RecordWorkingMemoryUsage(long memory) => WorkingMemoryUsage.Record(memory);
		public void RecordPrivateMemoryUsage(long memory) => PrivateMemoryUsage.Record(memory);
		public void AddComputerComponet(long averageMessageSize) =>  _averageMessageSize = averageMessageSize;
	}
}

