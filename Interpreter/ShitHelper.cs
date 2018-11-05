using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Interpreter
{
    public static class ShitHelper
    {
        private static readonly ConnectionFactory Client = new ConnectionFactory
        {
            UserName = "gqqowide",
            Password = "dfJZ6cTQ7QwQoAiDABOL4AUtefb1HOGa",
            HostName = "flamingo-01.rmq.cloudamqp.com",
            VirtualHost = "gqqowide"
        };

        private static readonly IConnection Conn = Client.CreateConnection();

        public static readonly IModel Model = GetModel;

        private const string ExchangeName = "DC";

        private static IModel GetModel
        {
            get
            {
                var model = Conn.CreateModel();
                string exchangeName = ExchangeName;
                model.ExchangeDeclare(exchangeName, "direct", true, false);
                foreach (var stepName in GetConstants)
                {
                    var queue = model.QueueDeclare(stepName, true, false, false);
                    model.QueueBind(queue, exchangeName, stepName);
                }
                model.BasicQos(0, 1, false);
                return model;
            }
        }

        public static void Publish<T>(string stepName, T obj)
        {
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
            Model.BasicPublish(ExchangeName, stepName, false, null, body);
        }

        public static CommonProcessesHandler Handler
        {
            get => _myHandler;
            set
            {
                if (_myHandler == null) _myHandler = value;
            }
        }
        private static CommonProcessesHandler _myHandler;

        private static List<string> GetConstants
        {
            get
            {
                var process = new Process();
                return process.All.Select(x => x.CurrentStep).ToList();
            }
        }
    }
}
