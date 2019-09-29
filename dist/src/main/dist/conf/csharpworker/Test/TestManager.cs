using System;
using System.Threading;
using csharpworker.Operations;
using csharpworker.Performance;
using csharpworker.Protocol;

namespace csharpworker.Test
{
    public class TestManager
    {
        private Server server;
        private TestContainer testContainer;
        private PerformanceMonitor performanceMonitor;

        public TestManager(Server server)
        {
            this.server = server;
        }

        public void CreateTest(string workerId, CreateTestOperation operation)
        {
            testContainer = new TestContainer(operation.testId, workerId, operation.properties, new TestContext());

            this.performanceMonitor = new PerformanceMonitor(testContainer, server);
            Thread performanceMonitorWrapper = new Thread(new ThreadStart(performanceMonitor.Run));
            performanceMonitorWrapper.Start();
        }

        public void StartTestPhase(StartPhaseOperation operation, ResponsePromise promise)
        {
            TestPhaseThread testPhaseThread = new TestPhaseThread(testContainer, operation.phase, promise);
            Thread testPhaseThreadWrapper = new Thread(new ThreadStart(testPhaseThread.Run));
            testPhaseThreadWrapper.Start();
        }

        public void StopRun()
        {
            testContainer.testContext.Stop();        
        }
    }

    class TestPhaseThread
    {

        private TestContainer testContainer;
        private string testPhase;
        private ResponsePromise promise;

        public TestPhaseThread(TestContainer testContainer, string testPhase, ResponsePromise promise)
        {
            this.testContainer = testContainer;
            this.testPhase = testPhase;
            this.promise = promise;        
        }

        public void Run()
        {
            testContainer.Invoke(testPhase, promise);

            if (testPhase != "RUN")
            {
                promise.ReplyOk();
            }        
        }    
    }
}
