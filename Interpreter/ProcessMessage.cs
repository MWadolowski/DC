using System.Collections.Generic;

namespace Interpreter
{
    public class ProcessMessage
    {
        /// <summary>
        /// No co się odpali po odebraniu wiadomości
        /// </summary>
        public string Step { get; set; }
        public Dictionary<Data, object> Attachments { get; set; }
    }
}
