using System;
using Apache.NMS;

namespace csharpworker.Protocol
{
    public class ResponsePromise
    {
        private Server server;
        private string correlationId;
        private IDestination replyTo;

        public ResponsePromise(Server server, string correlationId, IDestination replyTo)
        {
            this.server = server;
            this.correlationId = correlationId;
            this.replyTo = replyTo;
        }

        public void ReplyOk()
        {
            SendMessage("ok", "ok");
        }

        private void SendMessage(string body, string payload)
        {
            // Send a message
            IMessageProducer producer = server.session.CreateProducer(replyTo);
            ITextMessage message = producer.CreateTextMessage(body);
            if (replyTo != null)
            {
                message.NMSDestination = replyTo;
            }

            if (correlationId != null)
            {
                message.NMSCorrelationID = correlationId;
            }
            message.Properties["payload"] = payload;

            producer.Send(message);
        }
    }
}
