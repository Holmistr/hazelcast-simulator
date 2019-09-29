using System;
using System.Collections.Generic;

namespace csharpworker.Test
{
    public class TimeStepMethodPicker
    {

        private List<Range> ranges = new List<Range>();
        private Random random = new Random();

        public TimeStepMethodPicker(Dictionary<int, double> probabilitiesMap)
        {
            Console.WriteLine("Creating ranges");

            double lastProbabilityIntervalUpperBound = 0.0;
            foreach (KeyValuePair<int, double> entry in probabilitiesMap)
            {
                if (entry.Value == 0)
                {
                    continue;
                }

                Range range = new Range(entry.Key, lastProbabilityIntervalUpperBound, lastProbabilityIntervalUpperBound + entry.Value);
                ranges.Add(range);

                lastProbabilityIntervalUpperBound += entry.Value;
            }
        }

        public int Pick()
        {
            double randomPick = random.NextDouble();

            foreach (Range range in ranges)
            {
                if (range.lowerBound <= randomPick && randomPick <= range.upperBound)
                {
                    return range.rangeId;
                }
            }

            return 0;
        }
    }

   class Range
    {
        public int rangeId { get; }
        public double lowerBound { get; }
        public double upperBound { get; }

        public Range(int rangeId, double lowerBound, double upperBound)
        {
            this.rangeId = rangeId;
            this.lowerBound = lowerBound;
            this.upperBound = upperBound;
        }

        public override string ToString()
        {
            return string.Format("[Range: rangeId={0}, lowerBound={1}, upperBound={2}]", rangeId, lowerBound, upperBound);
        }
    }
}
