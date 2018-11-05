using System;
using Interpreter;
using Models;
using Newtonsoft.Json;

namespace FirstDecision
{
    public class UIMessageUpdater : IProcessHandler
    {
        public string StepName => StepNames.OrderReceived;
        public Action<OrderData, ulong?> UpdateUi { get; set; }
        public void Handle(ProcessMessage message, ulong tag)
        {
            var data = JsonConvert.DeserializeObject<OrderData>(message.Attachments[Data.OrderDataFile] as string);
            UpdateUi(data, tag);
        }

        public static UIMessageUpdater UpdaterWithUi { get; set; } = new UIMessageUpdater();
    }
}
