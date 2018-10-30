﻿using System;
using Interpreter;
using Models;
using Newtonsoft.Json;

namespace FirstDecision
{
    public class UIMessageUpdater : IProcessHandler
    {
        public string StepName => StepNames.OrderReceived;
        public Action<OrderData> UpdateUI { get; set; }
        public void Handle(ProcessMessage message, ulong tag)
        {
            var data = JsonConvert.DeserializeObject<OrderData>(message.Attachments[Data.OrderDataFile] as string);
            UpdateUI(data);
        }

        public static UIMessageUpdater UpdaterWithUi { get; set; } = new UIMessageUpdater();
    }
}