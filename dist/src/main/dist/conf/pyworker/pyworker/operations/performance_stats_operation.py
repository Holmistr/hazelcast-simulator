from ..operation_type import OperationType


class PerformanceStatsOperation():
    def __init__(self, source, test_id, operation_count, interval_throughput, interval_latency_avg_nanos):
        self._source = source
        self._test_id = test_id
        self._operation_count = operation_count
        self._interval_throughput = interval_throughput
        self._interval_latency_avg_nanos = interval_latency_avg_nanos

    def serialize(self):
        payload = {
            "operationType": OperationType.PERFORMANCE_STATE.value,
            "source": self._source,
            "payload": {
                "performanceStatsMap": {
                    self._test_id: {
                        "operationCount": self._operation_count,
                        "intervalThroughput": self._interval_throughput,
                        "intervalLatencyAvgNanos": self._interval_latency_avg_nanos
                    }
                }
            }
        }

        return payload