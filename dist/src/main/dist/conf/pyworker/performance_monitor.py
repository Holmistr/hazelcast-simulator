import threading

import time

from operations.performance_stats_operation import PerformanceStatsOperation


class PerformanceMonitor(threading.Thread):
    def __init__(self, test_container, coordinator_sender):
        threading.Thread.__init__(self)

        self._test_container = test_container
        self._coordinator_sender = coordinator_sender

    def run(self):
        while not self._test_container.get_test_context().is_stopped():
            tracker = self._test_container.get_performance_tracker()
            tracker.update()

            self._coordinator_sender.send(tracker.get_performance_stats().serialize())
            time.sleep(1)
