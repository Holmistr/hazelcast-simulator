using System;
using System.Threading;
using Apache.NMS;
using csharpworker.Operations;

namespace csharpworker.Protocol
{
    public class Server
    {
        private string topic;
        private string brokerUrl;

        private Boolean stop = false;

        public ISession session { get; set; }
        private IMessageConsumer workerConsumer;

        private ServerThread serverThread;

        private Worker worker;

        public Server(Worker worker, string topic)
        {
            this.worker = worker;
            this.topic = topic;
            this.brokerUrl = "stomp:tcp://localhost:9001";
        }

        public void Start()
        {
            Uri connectUri = new Uri(this.brokerUrl);
            Console.WriteLine("About to connect to " + connectUri);

            IConnectionFactory factory = new Apache.NMS.Stomp.ConnectionFactory(connectUri);
            IConnection connection = factory.CreateConnection();

            this.session = connection.CreateSession();

            IDestination workerDestination = session.GetTopic(this.topic);

            // Create a consumer and producer
            this.workerConsumer = session.CreateConsumer(workerDestination, "target='" + worker.address + "'");

            // Start the connection so that messages will be processed.
            connection.Start();

            this.serverThread = new ServerThread(this);
            Thread serverThreadWrapper = new Thread(new ThreadStart(this.serverThread.Run));
            serverThreadWrapper.Start();
        }

        public void Shutdown()
        {
            Console.WriteLine("Shutting down ServerThread");
            stop = true;
            session.Close();
        }

        public void SendCoordinator(PerformanceStatsOperation operation)
        {

            Console.WriteLine("PerformanceStatsOperation: " + operation);
            // Send a message
            IDestination coordinatorDestination = session.GetTopic("coordinator");
            Console.WriteLine("Coordinator Destination object: " + coordinatorDestination);
            IMessageProducer producer = session.CreateProducer(coordinatorDestination);
            Console.WriteLine("Coordinator Producer object: " + producer);
            ITextMessage message = producer.CreateTextMessage("ok");

            string payload = "{" +
                        "'performanceStatsMap': {" +
                            "'" + operation.testId + "':  { " +
                                "'operationCount': " + operation.operationCount + ", " +
                                "'intervalThroughput': " + Math.Round(operation.intervalThroughput) + ", " +
                                "'intervalLatencyAvgNanos': " + Math.Round(operation.intervalLatencyAvgNanos) +
                            "}" + 
                        "}" +
                    "}";

            message.Properties["operationType"] = 1002;
            message.Properties["source"] = operation.source;
            message.Properties["payload"] = payload;

            producer.Send(message);
        }

        class ServerThread
        {

            private Server server;

            public ServerThread(Server server)
            {
                this.server = server;
            }

            public void Run()
            {
                while (!server.stop)
                {
                    Handle();
                }
            }   

            private void Handle()
            {
                ITextMessage message = server.workerConsumer.Receive() as ITextMessage;
                if (message == null)
                {
                    throw new System.InvalidOperationException("Message should not be null");
                }

                int operationType = message.Properties.GetInt("operationType");
                string workerId = message.Properties.GetString("target");
                string payload = message.Properties.GetString("payload");
                ResponsePromise promise = new ResponsePromise(server, message.NMSCorrelationID, message.NMSReplyTo);

                if (operationType == (int)OperationType.TERMINATE_WORKER)
                {
                    this.server.Shutdown();
                }
                else if (operationType == (int)OperationType.CREATE_TEST)
                {
                    Console.WriteLine("Creating test");
                    CreateTestOperation operation = CreateTestOperation.Parse(payload);
                    server.worker.testManager.CreateTest(workerId, operation);

                    promise.ReplyOk();
                }
                else if (operationType == (int)OperationType.START_TEST_PHASE)
                {
                    StartPhaseOperation operation = StartPhaseOperation.Parse(payload);
                    server.worker.testManager.StartTestPhase(operation, promise);
                    Console.WriteLine("Starting phase");
                }
                else if (operationType == (int)OperationType.STOP_TEST)
                {
                    Console.WriteLine("Stopping the test");
                    server.worker.testManager.StopRun();

                    promise.ReplyOk();
                }
                else
                {
                    Console.WriteLine("Doing something else");

                    promise.ReplyOk();
                }
            }
        }
    }
}
