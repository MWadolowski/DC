using System.Net.Mail;

namespace FirstDecision
{
    internal class MailAttachment : Attachment
    {
        public MailAttachment(string fileName) : base(fileName)
        {
        }
    }
}