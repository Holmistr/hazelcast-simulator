using System;
using Hazelcast.Client;
using Hazelcast.Config;
using Hazelcast.Core;

namespace csharpworker.TestClasses
{
    public class HazelcastTest
    {
        protected IHazelcastInstance client;
        protected string name = "default";

        public HazelcastTest()
        {
            client = HazelcastClient.NewHazelcastClient("../client-hazelcast.xml");
        }       
    }
}
