using Interpreter;
using Models;

namespace FirstDecision
{
    public class DenyHandler : IProcessHandler
    {
        public string StepName => StepNames.OrderDeclined;

        public void Handle(ProcessMessage message, ulong tag)
        {
            var order = message.Attachments[Data.OrderDataFile] as OrderData;
            var number = order.Number;
            var reason = message.Attachments[Data.DenialReason] as string;
            string Body = "Przepraszamy, ale nie jesteśmy zainteresowani Państwa ofertą. " + reason;
            string Subject = "Odmowa ofery " + number;

            MailSender esender = new MailSender();
            esender.Send(order.Email, Body, Subject);
            ShitHelper.Model.BasicAck(tag, false);
        }
    }
}
