using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Interpreter
{
    public class CommonMessageHandler : DefaultBasicConsumer
    {
        public CommonMessageHandler(IModel model) : base(model) { }

        public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
        {
            var message = Encoding.UTF8.GetString(body);
            ShitHelper.Handler.Handle(JsonConvert.DeserializeObject<ProcessMessage>(message), deliveryTag);
        }
    }
}
