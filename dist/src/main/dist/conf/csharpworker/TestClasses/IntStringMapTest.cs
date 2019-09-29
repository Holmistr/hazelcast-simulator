using System;
using csharpworker.Attributes;
using csharpworker.TestClasses.Utils;
using Hazelcast.Core;

namespace csharpworker.TestClasses
{
    public class IntStringMapTest: HazelcastTest
    {

        public int keyCount = 1000;
        public int valueLength = 10;

        private IMap<int, string> map;

        private string[] values;

        [Setup]
        public void Setup()
        {         
            map = client.GetMap<int, string>(name);
            values = TestUtils.GenerateAsciiStrings(keyCount, valueLength);
        }

        [Prepare]
        public void Prepare()
        {
            for (int i = 0; i < keyCount; i++)
            {
                map.Put(i, values[i]); 
            }
        }

        [Timestep]
        public void Get()
        {
            Console.WriteLine(map.Get(RandomKey()));         
        }

        [Timestep]
        public void Put()
        {
            Console.WriteLine(map.Put(RandomKey(), RandomValue()));         
        }

        private int RandomKey()
        { 
            return new Random().Next(0, keyCount);
        }

        private string RandomValue()
        {
            return TestUtils.GenerateAsciiString(valueLength);
        }
    }
}
