using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Interpreter
{
    public class Process
    {
        private readonly IList<Step> _steps;
        private static IList<Step> _ourProces = new List<Step>();

        public Process()
        {
            _steps = JsonConvert.DeserializeObject<IList<Step>>(File.ReadAllText("./process.json"));
        }

        public Step Find(String name)
        {
            var current = _steps.FirstOrDefault(x => x.CurrentStep == name);
            if (current == null) throw new NotImplementedException($"ten krok jest nienznany: {name}");
       

            if(name == "Odebrane Zamówienie")
            {
                _ourProces.Add(current);
            }

            return current;
        }

        public Step Next(string name, DecisionType decisionMade)
        {
            var current = Find(name);
            var nextStep = current.NextStepsAccordingToDecision[decisionMade];
            if(nextStep == null) throw new NotImplementedException($"ten krok nie ma zdefiniowanej takiej decyzji: {name} - {decisionMade}");
            var next = _steps.FirstOrDefault(x => x.CurrentStep == nextStep);
            if (next == null) throw new NotImplementedException($"kolejny krok po {name} jest nieznany");

            _ourProces.Add(next);

            if (next.CurrentStep == "Odrzucenie zamówienia" || next.CurrentStep == "Zamównienie zostało przetworzone pomyślnie")
            {
                using (var writer = File.CreateText("./ourProcess.json"))
                {
                    writer.Write(JsonConvert.SerializeObject(_ourProces));
                }
            }
            return next;
        }

        public Step Start => _steps.FirstOrDefault(x => x.IsStartingPoint) ?? throw new NotImplementedException("gdzie jest start procesu");

        public IList<Step> All => new List<Step>(_steps);

        public static string MyStep { get; set; }
    }
}
