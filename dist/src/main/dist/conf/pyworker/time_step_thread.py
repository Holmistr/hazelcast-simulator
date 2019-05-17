import threading


class TimeStepThread(threading.Thread):
    def __init__(self, time_step_method, test_context, performance_tracker, promise):
        threading.Thread.__init__(self)

        self._time_step_method = time_step_method
        self._test_context = test_context
        self._promise = promise
        self._performance_tracker = performance_tracker

    def run(self):
        while not self._test_context.is_stopped():
            self._time_step_method()
            self._performance_tracker.add_iteration()

        self._promise.reply_ok()


