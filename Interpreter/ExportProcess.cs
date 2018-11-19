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
                    NextStepsAccordingToDecision = new Dictionary<DecisionType, string>
                    {
                        {DecisionType.Ok, StepNames.OrderForImplementation },
                        {DecisionType.Decline, StepNames.OrderDeclined }
                    },
                    CurrentStep = StepNames.OrderAccepted
                },
                new Step
                {
                    CurrentStep = StepNames.OrderDeclined
                },
                new Step
                {
                    NextStepsAccordingToDecision = new Dictionary<DecisionType, string>
                    {
                        {DecisionType.Ok, StepNames.OrderMerged },
                        {DecisionType.Decline, StepNames.OrderDeclined }
                    },
                    CurrentStep = StepNames.OrderForImplementation
                },
                new Step
                {
                    NextStepsAccordingToDecision = new Dictionary<DecisionType, string>
                    {
                        {DecisionType.Ok, StepNames.OrderSucces },
                        {DecisionType.Decline, StepNames.OrderDeclined }
                    },
                    CurrentStep = StepNames.OrderMerged
                },
                new Step
                {
                    CurrentStep = StepNames.OrderSucces
                }
            };
            using (var writer = File.CreateText("./process.json"))
            {
                writer.Write(JsonConvert.SerializeObject(steps));
            }
        }
    }
}
