using System.Collections.Generic;
using System.Linq;

namespace Interpreter
{
    public abstract class CommonProcessesHandler
    {
        public abstract IList<IProcessHandler> Handlers { get; }
        
        public void Handle(ProcessMessage message, ulong tag)
        {
            var handlerToUse = Handlers.FirstOrDefault(x => x.StepName == message.Step);
            handlerToUse?.Handle(message, tag);
        }
    }
}
