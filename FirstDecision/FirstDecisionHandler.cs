using System.Collections.Generic;
using Interpreter;

namespace FirstDecision
{
    public class FirstDecisionHandler : CommonProcessesHandler
    {
        public override IList<IProcessHandler> Handlers => new List<IProcessHandler>
        {
            UIMessageUpdater.UpdaterWithUi,
            AcceptHandler.UpdaterWithUi
        };
    }
}
