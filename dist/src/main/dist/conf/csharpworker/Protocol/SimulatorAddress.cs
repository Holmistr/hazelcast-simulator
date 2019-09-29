using System;
namespace csharpworker.Protocol
{
    public class SimulatorAddress
    {
        private string agentIndex;
        private string workerIndex;

        public SimulatorAddress(string agentIndex, string workerIndex)
        {
            this.agentIndex = agentIndex;
            this.workerIndex = workerIndex;
        }

        public override string ToString()
        {
            return agentIndex + "_" + workerIndex;
        }
    }
}
