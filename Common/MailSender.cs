using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace Models
{
    public class MailSender
    {
        public void Send(string to, string body, string subject, IList<Attachment> attachments = null)
        {
            var email = new MailMessage
            {
                To = { to },
                From = new MailAddress("DC.jazda@gmail.com"),
                Body = body,
                Subject = subject
            };
            foreach (var attachment in attachments ?? new List<Attachment>()) email.Attachments.Add(attachment);

            SmtpClient client = new SmtpClient
            {
                Port = 25,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Host = "in-v3.mailjet.com",
                Credentials = new NetworkCredential("b9e3cd0a28a7be86c57ba26dcad4aa08", "6b104384465d5c4a7aee4ab489a53ac5"),
            };

            client.Send(email);
        }
    }
}
