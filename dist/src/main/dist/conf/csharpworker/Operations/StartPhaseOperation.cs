using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace csharpworker.Operations
{
    public class StartPhaseOperation: SimulatorOperation
    {
        public string phase { get;  }
        private string testId;

        private StartPhaseOperation(string phase, string testId)
        {
            this.phase = phase;
            this.testId = testId;
        }

        public static StartPhaseOperation Parse(string payload)
        {
            Dictionary<string, string> deserializedPayload = JsonConvert.DeserializeObject<Dictionary<string, string>>(payload);

            return new StartPhaseOperation(deserializedPayload["testPhase"], deserializedPayload["testId"]);
        }
    }
}
