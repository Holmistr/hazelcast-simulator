using System;
namespace csharpworker.Operations
{
    public enum OperationType
    {
        PERFORMANCE_STATE = 1002,

        TERMINATE_WORKER = 4001,
        CREATE_TEST = 4002,
        START_TEST_PHASE = 4004,
        STOP_TEST = 4005

    }
}
