using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Models;

namespace FirstDecision {
    /// <summary>
    /// Interaction logic for FinalDecision.xaml
    /// </summary>
    public partial class FinalDecision : Window {
        private List<int> ids = new List<int>();

        public FinalDecision() {
            InitializeComponent();

            OrdersList.ItemsSource = ids;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e) {
            ids.Clear();

            List<MailMessage> messages = EmailRepository.GetAllMessages();
            List<WorkerAssignmentData> assignments = Database.assignments.SelectAll();

            List<int> orderIds = assignments.Select(x => x.orderId).Distinct().ToList();

            foreach (int orderId in orderIds) {
                IEnumerable<MailMessage> messagesFiltered = messages.Where(x => x.Subject.EndsWith("Zamówienie nr " + orderId));
                if (!messages.Any()) return;
                List<MailMessage> messagesForOrder = messagesFiltered.ToList();

                List<WorkerAssignmentData> assignmentsForOrder = assignments.Where(x => x.orderId == orderId).ToList();

                if (messagesForOrder.Count < assignmentsForOrder.Count) {
                    //Ilość wiadomości 
                    continue;
                }

                //sprawdzenie czy każdy pracownik wysłał maila
                if (!assignmentsForOrder.Any(x => messagesForOrder.Any(y => y.Sender != null && y.Sender.Address == x.worker.Email))) {
                    ids.Add(orderId);
                }
            }

            OrdersList.Items.Refresh();
        }
    }
}
