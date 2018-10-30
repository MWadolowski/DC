using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using Interpreter;
using Models;
using Newtonsoft.Json;

namespace Server
{
    public class FirstStep
    {
        private readonly Emails _emails = new Emails();
        private readonly MailSender _mailSender = new MailSender();

        public void Send(OrderData data)
        {
            var process = new Process();
            var current = process.Start;
            var number = GenerateNumber;
            IncreaseNumber();
            data.Number = number;
            var prezes = _emails.FirstStepWorkers().FirstOrDefault();
            var products = String.Join("\n", data.Products.Select(x => $"{x.Product}: {x.Quantity}"));
            _mailSender.Send(prezes, 
                $"nowe zamówienie o numerze {number} od {data.Name} {data.LastName}\nzamówienie na:\n{products}", 
                $"nowe zamówienie nr {number}", new List<Attachment>
                {
                    Attachment.CreateAttachmentFromString(JsonConvert.SerializeObject(data), $"zamówienie_{number}.json")
                });
            ShitHelper.Publish(current.CurrentStep, new ProcessMessage
            {
                Step = current.CurrentStep,
                Attachments = new Dictionary<Data, object>
                {
                    { Data.OrderDataFile, JsonConvert.SerializeObject(data) }
                }
            });
        }

        private int GenerateNumber
        {
            get
            {
                var path = Path;
                if (!File.Exists(path))
                {
                    var stream = File.CreateText(path);
                    stream.Write(1);
                    stream.Dispose();
                    return 1;
                }

                using (var stream = File.OpenText(path))
                {
                    var text = stream.ReadToEnd();
                    return Int32.Parse(text);
                }
            }
        }

        private void IncreaseNumber()
        {
            var number = GenerateNumber;
            File.Delete(Path);
            using (var stream = File.CreateText(Path))
            {
                stream.Write(++number);
            }
        }

        private string Path => "./latestNumber.txt";
    }
}
