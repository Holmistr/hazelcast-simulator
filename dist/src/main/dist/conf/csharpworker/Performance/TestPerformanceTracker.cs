using System;
using csharpworker.Operations;

namespace csharpworker.Performance
{
    public class TestPerformanceTracker
    {
        private int iterations = 0;
        private int lastIterations = 0;
        private int totalNumberOfIterations = 0;
        private double lastUpdatedTimeMillis = 0;

        private double intervalThroughput = 0;
        private double intervalLatencyAvgNanos = 0;

        private string testId;
        private string workerId;

        public TestPerformanceTracker(string testId, string workerId)
        {
            this.testId = testId;
            this.workerId = workerId;
        }

        public void AddIteration()
        {
            iterations++;
        }

        public void Update()
        {
            int intervalOperationCount = iterations - lastIterations;

            double currentTimeMillis = (DateTime.Now - DateTime.MinValue).TotalMilliseconds;
            double intervalTimeDelta = currentTimeMillis - lastUpdatedTimeMillis;

            intervalThroughput = (intervalOperationCount * 1000) / intervalTimeDelta;
            intervalLatencyAvgNanos = Math.Abs(intervalThroughput) < 0.01 ? -1 : (1 * 1000 * 1000 * 1000) / intervalThroughput;

            totalNumberOfIterations += intervalOperationCount;
            lastIterations = totalNumberOfIterations;

            lastUpdatedTimeMillis = currentTimeMillis;
        }

        public PerformanceStatsOperation GetPerformanceStats()
        {
            return new PerformanceStatsOperation(
                workerId,
                testId,
                totalNumberOfIterations,
                intervalThroughput,
                intervalLatencyAvgNanos);
        }
    }
}