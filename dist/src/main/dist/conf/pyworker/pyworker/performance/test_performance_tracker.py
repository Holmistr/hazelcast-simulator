import time

from ..operations.performance_stats_operation import PerformanceStatsOperation


class TestPerformanceTracker():
    def __init__(self):
        self._iterations = 0

        self._last_iterations = 0
        self._total_number_of_iterations = 0
        self._last_updated_time_millis = 0

        self._interval_throughput = 0
        self._interval_latency_avg_nanos = 0

    def add_iteration(self):
        self._iterations = self._iterations + 1

    def update(self):
        interval_operation_count = self._iterations - self._last_iterations

        current_time_millis = time.time() * 1000
        interval_time_delta = current_time_millis - self._last_updated_time_millis

        self._interval_throughput = (interval_operation_count * 1000) / interval_time_delta
        self._interval_latency_avg_nanos = -1 if (self._interval_throughput == 0) else (1 * 1000 * 1000 * 1000) / self._interval_throughput
        #print(self._interval_throughput)
        #print(self._interval_latency_avg_nanos)

        self._total_number_of_iterations = self._total_number_of_iterations + interval_operation_count
        self._last_iterations = self._total_number_of_iterations

        self._last_updated_time_millis = current_time_millis

    def get_performance_stats(self):
        return PerformanceStatsOperation("A1_W2",
                                         "IntStringMapTest",
                                         self._total_number_of_iterations,
                                         self._interval_throughput,
                                         self._interval_latency_avg_nanos)
