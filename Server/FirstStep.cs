using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Server.Controllers;

namespace Server
{
    public class FirstStep
    {
        private readonly Emails _emails = new Emails();

        public void Send(OrderData data)
        {
            var number = GenerateNumber;
            IncreaseNumber();
            var prezes = _emails.FirstStepWorkers().FirstOrDefault();
            var products = String.Join("\n", data.Products.Select(x => $"{x.Product}: {x.Quantity}"));
            var email = new MailMessage
            {
                To = { prezes},
                From = new MailAddress("DC.jazda@gmail.com"),
                
                Body = $"nowe zamówienie o numerze {number} od {data.Name} {data.LastName}\nzamówienie na:\n{products}",
                Subject = $"nowe zamówienie nr {number}"
            };

            SmtpClient client = new SmtpClient
            {
                Port = 25,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Host = "in-v3.mailjet.com",
                Credentials = new NetworkCredential("b9e3cd0a28a7be86c57ba26dcad4aa08", "6b104384465d5c4a7aee4ab489a53ac5")
            };

            client.Send(email);
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
