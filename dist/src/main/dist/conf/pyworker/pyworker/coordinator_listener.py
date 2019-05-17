import stomp

from operation_type import OperationType
from operations.create_test_operation import CreateTestOperation
from operations.start_phase_operation import StartPhaseOperation
from response_promise import ResponsePromise


class CoordinatorListener(stomp.ConnectionListener):
    def __init__(self, worker, connection, test_manager):
        self._connection = connection
        self._worker = worker
        self._test_manager = test_manager

    def on_error(self, headers, message):
        print('received an error "%s"' % headers)

    def on_message(self, headers, message):
        #print('received a message "%s"' % headers)

        operation_type = int(headers["operationType"])

        promise = ResponsePromise(connection=self._connection,
                                  correlation_id=(headers["correlation-id"] if "correlation-id" in headers else None),
                                  reply_to=(headers["reply-to"] if "reply-to" in headers else None))

        if operation_type == OperationType.CREATE_TEST.value:
            create_operation = CreateTestOperation.parse(headers)
            print("I received CREATE_TEST operation! Let's create some test!" + create_operation.get_test_id())
            self._test_manager.create_test(create_operation)
            promise.reply_ok()
        elif operation_type == OperationType.TERMINATE_WORKER.value:
            print("Let's terminate this worker.")
            self._worker.stop()
            #promise.reply_ok()
        elif operation_type == OperationType.START_TEST_PHASE.value:
            start_phase_operation = StartPhaseOperation.parse(headers)
            print("Start Test Phase operation: " + start_phase_operation.get_test_phase())
            self._test_manager.start_test_phase(start_phase_operation, promise)
            # promise.reply_ok()
        elif operation_type == OperationType.STOP_TEST.value:
            print("Stop Test run operation")
            self._test_manager.stop_run()
            promise.reply_ok()
        else:
            print("I received a different operation with operationType " + headers["operationType"])
            promise.reply_ok()



