using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using S22.Imap;

namespace FirstDecision {
    static class EmailRepository {
        private const string HOSTNAME = "imap.gmail.com";
        private const string LOGIN = "dc.jazda";
        private const string PASSWORD = "Dupa#1234";

        private static ImapClient client;

        static EmailRepository() {
            client = new ImapClient(HOSTNAME, 993, LOGIN, PASSWORD, AuthMethod.Login, true);
        }

        public static void Start() {

        }

        public static List<MailMessage> GetAllMessages() {
            return GetFilteredMessages(SearchCondition.All());
        }

        public static List<MailMessage> GetFilteredMessages(SearchCondition condition) {
            IEnumerable<uint> messageIds = client.Search(SearchCondition.All());
            IEnumerable<MailMessage> messages = client.GetMessages(messageIds);

            return messages.ToList();
        }

        public static List<int> GetCompletedTasks() {
            return null;
        }
    }
}
