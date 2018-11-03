
using System.Collections.Generic;

namespace Interpreter
{
    public class Step
    {
        public Dictionary<DecisionType, string> NextStepsAccordingToDecision { get; set; } = new Dictionary<DecisionType, string>();
        public string CurrentStep { get; set; }
        public bool IsStartingPoint { get; set; }
        public bool IsManual { get; set; }
    }
}
