using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Interpreter
{
    public class ExportProcess
    {
        public void Export()
        {
            var steps = new List<Step>
            {
                new Step
                {
                    CurrentStep = StepNames.OrderReceived,
                    IsStartingPoint = true,
                    NextStepsAccordingToDecision = new Dictionary<DecisionType, string>
                    {
                        {DecisionType.Ok, StepNames.OrderAccepted },
                        {DecisionType.Decline, StepNames.OrderDeclined }
                    },
                    IsManual = true
                },
                new Step
                {
                    CurrentStep = StepNames.OrderAccepted
                },
                new Step
                {
                    CurrentStep = StepNames.OrderDeclined
                }
            };
            using (var writer = File.CreateText("./process.json"))
            {
                writer.Write(JsonConvert.SerializeObject(steps));
            }
        }
    }
}
