using System;
using csharpworker.Attributes;
using Hazelcast.Core;

namespace csharpworker.TestClasses
{
    public class IntIntMapTest: HazelcastTest
    {

        public int keyCount = 1000;
        public int valueLength = 10;

        private IMap<int, int> map;

        private string[] values;

        [Setup]
        public void Setup()
        {         
            map = client.GetMap<int, int>(name);
        }

        [Prepare]
        public void Prepare()
        {
            Random random = new Random();
            for (int i = 0; i < keyCount; i++)
            {
                map.Put(i, random.Next()); 
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

        private int RandomValue()
        {
            return new Random().Next();
        }
    }
}
