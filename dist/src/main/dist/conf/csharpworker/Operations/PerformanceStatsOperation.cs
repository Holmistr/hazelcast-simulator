using System;
namespace csharpworker.Operations
{
    public class PerformanceStatsOperation
    {
        public string source { get; }
        public string testId { get; }
        public int operationCount { get; }
        public double intervalThroughput { get; }
        public double intervalLatencyAvgNanos { get; }

        public PerformanceStatsOperation(string source, string testId, int operationCount, double intervalThroughput, double intervalLatencyAvgNanos)
        {
            this.source = source;
            this.testId = testId;
            this.operationCount = operationCount;
            this.intervalThroughput = intervalThroughput;
            this.intervalLatencyAvgNanos = intervalLatencyAvgNanos;
        }

        public override string ToString()
        {
            return string.Format("[PerformanceStatsOperation: source={0}, testId={1}, operationCount={2}, intervalThroughput={3}, intervalLatencyAvgNanos={4}]", source, testId, operationCount, intervalThroughput, intervalLatencyAvgNanos);
        }
    }
}
