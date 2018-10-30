using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interpreter;
using Models;
using Newtonsoft.Json;

namespace FirstDecision
{
    public class UIMessageUpdater : IProcessHandler
    {
        public string StepName => StepNames.OrderReceived;
        public void Handle(ProcessMessage message, ulong tag)
        {
            var data = JsonConvert.DeserializeObject<OrderData>(message.Attachments[Data.OrderDataFile] as string);
        }

        public static UIMessageUpdater UpdaterWithUi { get; set; } = new UIMessageUpdater();
    }
}
