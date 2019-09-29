using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using csharpworker.Protocol;
using csharpworker.Test;

namespace csharpworker
{
    public class Worker
    {

        private Server server;
        public string address { get; }
        public TestManager testManager { get; }

        public Worker(string address) 
        {
            this.address = address;

            server = new Server(this, "workers");
            testManager = new TestManager(server);
        }

        public void Start() 
        {
            server.Start();

            WritePidFile();
        }

        private void WritePidFile()
        {
            int pid = Process.GetCurrentProcess().Id;
            Console.WriteLine("Worker PID: " + pid);
            string[] lines = { pid.ToString() };
            System.IO.File.WriteAllLines("../worker.pid", lines);
        }

        static void Main(string[] args)
        {
            ImportWorkerParametersAsEnvVariables();

            Worker worker = new Worker(Environment.GetEnvironmentVariable("WORKER_ADDRESS"));
            worker.Start();

            Console.WriteLine("Exiting C# worker");
        }

        static void ImportWorkerParametersAsEnvVariables()
        {         
            foreach (var row in File.ReadAllLines("../parameters"))
            {
                string propertyName = row.Split('=')[0];
                string propertyValue = string.Join("=", row.Split('=').Skip(1).ToArray());

                Environment.SetEnvironmentVariable(propertyName, propertyValue, EnvironmentVariableTarget.Machine);
            }
        }
    }
}
