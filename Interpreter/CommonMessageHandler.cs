using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using RabbitMQ.Client;

namespace Interpreter
{
    public class CommonMessageHandler : DefaultBasicConsumer
    {
        public CommonMessageHandler(IModel model) : base(model) { }

        public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
        {
            var message = Deserialize<ProcessMessage>(body);
            ShitHelper.Handler.Handle(message, deliveryTag);
        }

        private T Deserialize<T>(byte[] param)
        {
            using (MemoryStream ms = new MemoryStream(param))
            {
                IFormatter br = new BinaryFormatter();
                return (T)br.Deserialize(ms);
            }
        }
    }
}
