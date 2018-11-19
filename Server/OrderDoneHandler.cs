using Interpreter;
using Models;
using Newtonsoft.Json;

namespace Server
{
    public class OrderDoneHandler : IProcessHandler
    {
        public string StepName => StepNames.OrderSucces;
        public void Handle(ProcessMessage message, ulong tag)
        {
            var db = new HistoryBase();
            var data = JsonConvert.DeserializeObject<OrderData>(message.Attachments[Data.OrderDataFile] as string);
            db.Save(data);
            ShitHelper.Model.BasicAck(tag, false);
        }
    }
}
