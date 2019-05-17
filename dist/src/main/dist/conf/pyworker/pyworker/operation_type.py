from enum import Enum


class OperationType(Enum):

    PERFORMANCE_STATE = 1002

    TERMINATE_WORKER = 4001
    CREATE_TEST = 4002
    START_TEST_PHASE = 4004
    STOP_TEST = 4005