using System;
using System.Reflection;
using csharpworker.Performance;
using csharpworker.Protocol;
using csharpworker.TestClasses;

namespace csharpworker.Test
{
    public class TimeStepThread
    {
        private TestContext testContext;
        private ResponsePromise promise;
        private TestPerformanceTracker performanceTracker;

        private HazelcastTest testInstance;
        private MethodInfo[] methods;
        private TimeStepMethodPicker methodPicker;

        public TimeStepThread(HazelcastTest testInstance, MethodInfo[] methods, TimeStepMethodPicker methodPicker, TestContext testContext, ResponsePromise promise, TestPerformanceTracker performanceTracker)
        {
            this.testInstance = testInstance;
            this.methods = methods;
            this.methodPicker = methodPicker;
            this.testContext = testContext;
            this.promise = promise;
            this.performanceTracker = performanceTracker;
        }   

        public void Run()
        {
            while (!testContext.isStopped)
            {
                methods[methodPicker.Pick()].Invoke(testInstance, null);
                performanceTracker.AddIteration();
            }

            promise.ReplyOk();
        }
    }
}
