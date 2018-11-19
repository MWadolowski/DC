using System.Collections.Generic;
using Interpreter;

namespace Server
{
    public class ServerHandler : CommonProcessesHandler
    {
        public override IList<IProcessHandler> Handlers => new List<IProcessHandler>
        {
            new OrderDoneHandler()
        };
    }
}
