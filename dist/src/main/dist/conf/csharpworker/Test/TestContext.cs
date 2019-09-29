using System;
namespace csharpworker.Test
{
    public class TestContext
    {
        public bool isStopped { get; set;  } = false;

        public TestContext()
        {
        }

        public void Stop()
        {
            isStopped = true;
        }
    }
}
