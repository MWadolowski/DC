using System.Collections.Generic;
using System.Linq;

namespace Interpreter
{
    public abstract class CommonProcessesHandler
    {
        public abstract IList<IProcessHandler> Handlers { get; }

        private readonly Process _process = new Process();

        public void Handle(ProcessMessage message, ulong tag)
        {
            var handlerToUse = Handlers.FirstOrDefault(x => x.StepName == message.Step);
            if(handlerToUse == null) return;
            handlerToUse.Handle(message);
            var stepData = _process.Find(message.Step);
            if (!stepData.IsManual)
            {
                ShitHelper.Model.BasicAck(tag, false);
            }
        }
    }
}
