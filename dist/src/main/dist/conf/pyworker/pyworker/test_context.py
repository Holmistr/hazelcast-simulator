class TestContext():
    def __init__(self):
        self._is_stopped = False

    def stop(self):
        self._is_stopped = True

    def is_stopped(self):
        return self._is_stopped
