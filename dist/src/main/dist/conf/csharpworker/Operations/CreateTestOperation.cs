using System;
using System.Collections.Generic;
using Apache.NMS;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace csharpworker.Operations
{
    public class CreateTestOperation: SimulatorOperation
    {
    
        public string testId { get; }
        public Dictionary<string, string> properties { get; }

        private CreateTestOperation(string testId, Dictionary<string, string> properties)
        {
            this.testId = testId;
            this.properties = properties;
        }

        public static CreateTestOperation Parse(string payload)
        {
            JObject parsedObject = JObject.Parse(payload);
            Dictionary<string, string> deserializedProperties = JsonConvert.DeserializeObject<Dictionary<string, string>>(parsedObject.GetValue("properties").ToString());

            return new CreateTestOperation(parsedObject.GetValue("testId").ToString(), deserializedProperties);        
        }       

        public override string ToString()
        {
            return string.Format("[CreateTestOperation: testId={0}, properties={1}]", testId, properties);
        }
    }
}
