import time
import os
import stomp

from coordinator_listener import CoordinatorListener
from coordinator_sender import CoordinatorSender
from test_manager import TestManager


class Worker():
    def __init__(self):
        self._stop = False

    def start(self):
        self._create_worker_pid_file()

        print("Started the Python worker!")

        self._start_broker_connection()

        while not self._stop:
            time.sleep(1)

        self._shutdown()

    def _start_broker_connection(self):
        hosts = [('localhost', 9001)]
        self._connection = stomp.Connection(host_and_ports=hosts)
        self._connection.set_listener('coordinatorListener', CoordinatorListener(connection=self._connection,
                                                                                 worker=self,
                                                                                 test_manager=TestManager(CoordinatorSender(self._connection))))
        self._connection.start()
        self._connection.connect(wait=True)
        self._connection.subscribe(destination='/topic/workers', id=1, ack='auto')

    def _create_worker_pid_file(self):
        pid = os.getpid()

        f = open("worker.pid", "w")
        f.write(str(pid))
        f.close()

    def stop(self):
        print("Stopping the worker.")

        self._stop = True

    def _shutdown(self):
        print("Shutting down the worker.")

        self._connection.disconnect()
