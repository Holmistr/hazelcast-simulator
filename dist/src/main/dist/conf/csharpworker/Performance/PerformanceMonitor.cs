using System;
using System.Threading;
using csharpworker.Protocol;
using csharpworker.Test;

namespace csharpworker.Performance
{
    public class PerformanceMonitor
    {

        private TestContainer testContainer;
        private Server server;

        public PerformanceMonitor(TestContainer testContainer, Server server)
        {
            this.testContainer = testContainer;
            this.server = server;
        }

        public void Run()
        {
            while (!testContainer.testContext.isStopped)
            {
                TestPerformanceTracker tracker = testContainer.performanceTracker; 
                tracker.Update();

                server.SendCoordinator(tracker.GetPerformanceStats());
                Thread.Sleep(1000);              
            }        
        }
    }
}
