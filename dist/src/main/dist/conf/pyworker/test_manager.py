import threading

from performance_monitor import PerformanceMonitor
from test_container import TestContainer
from test_context import TestContext


class TestManager:

    def __init__(self, coordinator_sender):
        self._tests = []
        self._test_container = []
        self._performance_monitor = []
        self._coordinator_sender = coordinator_sender

    def create_test(self, create_operation):
        self._test_container = TestContainer(create_operation.get_test_id(),
                                             create_operation.get_properties(),
                                             TestContext())

        self._performance_monitor = PerformanceMonitor(self._test_container, self._coordinator_sender)
        self._performance_monitor.start()

    def start_test_phase(self, start_phase_operation, promise):
        thread = _TestPhaseThread(self._test_container, start_phase_operation.get_test_phase(), promise)
        thread.start()

    def stop_run(self):
        self._test_container.get_test_context().stop()


class _TestPhaseThread(threading.Thread):

    def __init__(self, test_container, test_phase, promise):
        threading.Thread.__init__(self)

        self._test_container = test_container
        self._test_phase = test_phase
        self._promise = promise

    def run(self):
        self._test_container.invoke(self._test_phase, self._promise)

        if not self._test_phase == "RUN":
            self._promise.reply_ok()








