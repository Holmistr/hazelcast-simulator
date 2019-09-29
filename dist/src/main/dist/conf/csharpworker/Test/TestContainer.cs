using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using csharpworker.Attributes;
using csharpworker.Performance;
using csharpworker.Protocol;
using csharpworker.TestClasses;

namespace csharpworker.Test
{
    public class TestContainer
    {
        private string testId;
        private Dictionary<string, string> properties;
        public TestContext testContext { get; }
        public TestPerformanceTracker performanceTracker { get; }

        private HazelcastTest testInstance;

        public TestContainer(string testId, string workerId, Dictionary<string, string> properties, TestContext testContext)
        {
            this.testId = testId;
            this.properties = properties;
            this.testContext = testContext;

            CreateTestInstance();
            BindProperties();

            this.performanceTracker = new TestPerformanceTracker(testId, workerId);
        }

        public void Invoke(string testPhase, ResponsePromise promise)
        {
            if (testPhase == "RUN")
            {
                int threadCount = int.Parse(properties["threadCount"]);

                MethodInfo[] timestepMethods = GetMethodsWithAttribute(typeof(Timestep));

                for (int i = 0; i < threadCount; i++)
                {
                    TimeStepMethodPicker methodPicker = CreateProbabilityRangesForTimeStepMethods(properties, timestepMethods);

                    TimeStepThread timeStepThread = new TimeStepThread(testInstance, timestepMethods, methodPicker, testContext, promise, performanceTracker);
                    Thread timeStepThreadWrapper = new Thread(new ThreadStart(timeStepThread.Run));
                    timeStepThreadWrapper.Start();
                }
            }
            else if (testPhase == "SETUP")
            {
                MethodInfo[] setupMethods = GetMethodsWithAttribute(typeof(Setup));
                setupMethods[0].Invoke(testInstance, null);
            }
            else if (testPhase == "LOCAL_PREPARE")
            {
                MethodInfo[] prepareMethods = GetMethodsWithAttribute(typeof(Prepare));
                prepareMethods[0].Invoke(testInstance, null);
            }
        }

        private MethodInfo[] GetMethodsWithAttribute(Type attribute)
        {
            MethodInfo[] methods = testInstance.GetType().GetMethods().Where(methodInfo => methodInfo.GetCustomAttributes(attribute, true).Length > 0).ToArray();
            return methods;
        }

        private void BindProperties()
        {
            foreach (KeyValuePair<string, string> kvp in properties)
            {
                //textBox3.Text += ("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            }

            FieldInfo[] fields = testInstance.GetType().GetFields();
            foreach (FieldInfo field in fields)
            {
                if (properties.ContainsKey(field.Name))
                {
                    String value = properties[field.Name];

                    if (Type.GetTypeCode(field.FieldType) == TypeCode.Int32)
                    {
                        int intValue = Int32.Parse(value);
                        field.SetValue(testInstance, intValue);                 
                    } 
                    else
                    {
                        field.SetValue(testInstance, value);
                    }
                }
            }
        }

        private void CreateTestInstance()
        {
            string className = properties["class"];
            Type type = Type.GetType(className);
            var instantiatedObject = Activator.CreateInstance(type);
            testInstance = (HazelcastTest)instantiatedObject;
        }

        private TimeStepMethodPicker CreateProbabilityRangesForTimeStepMethods(Dictionary<string, string> properties, MethodInfo[] methods)
        {
            Dictionary<int, double> probabilityPerMethod = new Dictionary<int, double>();
            for (int i = 0; i < methods.Length; i++)
            {
                MethodInfo method = methods[i];
                string methodProbabilityProperty = method.Name.ToLower() + "Prob";

                if (properties.ContainsKey(methodProbabilityProperty))
                {
                    probabilityPerMethod[i] = double.Parse(properties[methodProbabilityProperty], System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"));                 
                }
            }

            TimeStepMethodPicker methodPicker = new TimeStepMethodPicker(probabilityPerMethod);
            return methodPicker;
        }
    }
}