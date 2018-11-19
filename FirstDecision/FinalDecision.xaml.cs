using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Interpreter;
using Models;

namespace FirstDecision {
    /// <summary>
    /// Interaction logic for FinalDecision.xaml
    /// </summary>
    public partial class FinalDecision : Window {
        private List<int> ids = new List<int>();
        private List<int> unfinishedIds = new List<int>();

        private List<MailMessage> messages = new List<MailMessage>();

        private OrderData parsedData;
        private List<FinalProductData> parsedProductList;

        public FinalDecision() {
            InitializeComponent();

            ((GridView) OrdersList.View).Columns[0].Header = "Ukończone";
            ((GridView) OrdersList.View).Columns[0].Width = 100;
            ((GridView) UnfinishedOrdersList.View).Columns[0].Header = "Nieukończone";
            ((GridView) UnfinishedOrdersList.View).Columns[0].Width = 100;

            SetEditorFromOrder(null);

            OrdersList.ItemsSource = ids;
            UnfinishedOrdersList.ItemsSource = unfinishedIds;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e) {
            ids.Clear();
            unfinishedIds.Clear();

            messages = EmailRepository.GetAllMessages();
            List<WorkerAssignmentData> assignments = Database.assignments.SelectAll();

            List<int> orderIds = assignments.Select(x => x.orderId).Distinct().ToList();

            foreach (int orderId in orderIds) {
                IEnumerable<MailMessage> messagesFiltered = messages.Where(x => x.Subject.EndsWith("Zamówienie nr " + orderId));
                if (!messagesFiltered.Any()) {
                    unfinishedIds.Add(orderId);
                    continue;
                }

                List<MailMessage> messagesForOrder = messagesFiltered.ToList();

                List<WorkerAssignmentData> assignmentsForOrder = assignments.Where(x => x.orderId == orderId).ToList();

                if (messagesForOrder.Count < assignmentsForOrder.Count) {
                    unfinishedIds.Add(orderId);
                    continue;
                }

                //sprawdzenie czy każdy pracownik wysłał maila
                bool unfinished = false;
                foreach (WorkerAssignmentData workerAssignment in assignmentsForOrder) {
                    if (!messagesForOrder.Exists(x => x.From != null && workerAssignment.worker.Email == x.From.Address)) {
                        unfinishedIds.Add(orderId);
                        unfinished = true;
                        break;
                    }
                }

                if (!unfinished) {
                    ids.Add(orderId);
                }

                //rozumiem ze tutaj jest juz sytuacja w której excele są zmergowane??? Jesli tak to ten kod jes ok
                var step = new Process().Next(StepNames.OrderForImplementation, DecisionType.Ok);
                ShitHelper.Publish(step.CurrentStep, new ProcessMessage
                {
                    Step = step.CurrentStep,
                });
            }

            OrdersList.Items.Refresh();
            UnfinishedOrdersList.Items.Refresh();
        }

        private void SetEditorFromOrder(OrderData order) {
            if (order == null) {
                Pieces.Text = "";
                Credentials.Text = "";
                Email.Text = "";
                OrderId.Text = "";
                IsReady.IsChecked = false;
                Accept.IsEnabled = false;
                Reject.IsEnabled = false;
                OrderContent.ItemsSource = new List<FinalProductData>();
                OrderContent.Items.Refresh();

                parsedProductList = new List<FinalProductData>();
                parsedData = null;
            }
            else {
                List<FinalProductData> finalProductData;
                bool finished;
                int assignmentCount;
                int assignmentFinished;

                GetCurrentOrderData(order.Number, out finalProductData, out finished, out assignmentCount, out assignmentFinished);

                Pieces.Text = assignmentFinished + " / " + assignmentCount;
                Credentials.Text = order.Name + " " + order.LastName;
                Email.Text = order.Email;
                OrderId.Text = order.Number.ToString();
                IsReady.IsChecked = finished;
                Accept.IsEnabled = finished;
                Reject.IsEnabled = true;
                OrderContent.ItemsSource = finalProductData;
                OrderContent.Items.Refresh();

                parsedProductList = finalProductData;
                parsedData = order;
            }
        }

        private void ChangeSelectedItem(object sender, MouseButtonEventArgs e) {
            ListViewItem selected = sender as ListViewItem;
            int orderId = (int) selected.Content;
            OrderData order = Database.orders.SelectAll().Where(x => x.Number == orderId).First();

            SetEditorFromOrder(order);
        }

        private void GetCurrentOrderData(int orderId, out List<FinalProductData> productData, out bool finished,
            out int assignmentCount, out int assignmentFinished) {

            List<WorkerAssignmentData> assignments = Database.assignments.SelectAll().Where(x => x.orderId == orderId).ToList();
            List<MailMessage> relevantMessages = null;

            if (messages.Any(x => x.Subject.EndsWith("Zamówienie nr " + orderId))) {
                relevantMessages = messages.Where(x => x.Subject.EndsWith("Zamówienie nr " + orderId)).ToList();
                productData = new List<FinalProductData>();

                foreach (string address in relevantMessages.Select(x => x.From.Address).Distinct()) {
                    MailMessage message = relevantMessages.Where(x => x.From.Address == address).Last();
                    productData.AddRange(ExcelManager.ReadExcelFromMessage(message));
                }

                assignmentCount = assignments.Count;
                assignmentFinished = relevantMessages.GroupBy(x => x.From.Address).Count();
                finished = assignments.Count == assignmentFinished;
            }
            else {
                assignmentCount = assignments.Count;
                assignmentFinished = 0;
                productData = new List<FinalProductData>();
                finished = false;
            }
        }

        private void Accept_Click(object sender, RoutedEventArgs e) {
            //ZMIENNE Z KTÓRYCH MOŻESZ KORZYSTAĆ :: 
            //parsedData -> przetwarzane zamówienie, typ OrderData
            //parsedProductList -> produkty wzbogacone o informacje z excela. NIE KORZYSTAJ Z parsedOrder.Products !!!!!!!!!1

            MessageBox.Show("Tutaj należy dodać dalszą część.", "Wysłanie wiadomości");

            var step = new Process().Next(StepNames.OrderMerged, DecisionType.Ok);
            ShitHelper.Publish(step.CurrentStep, new ProcessMessage
            {
                Step = step.CurrentStep,
            });

            SetEditorFromOrder(null);
        }

        private void Reject_Click(object sender, RoutedEventArgs e) {
            string to = parsedData.Email;
            string subject = "Zamówienie nr. " + parsedData.Number;
            string content = "Przepraszamy, ale zamówienia zostało anulowane.";

            MailSender mailSender = new MailSender();
            mailSender.Send(to, content, subject);

            Database.assignments.SelectAll().RemoveAll(x=>x.orderId == parsedData.Number);
            Database.assignments.SaveData();
            Database.orders.SelectAll().RemoveAll(x => x.Number == parsedData.Number);
            Database.orders.SaveData();

            unfinishedIds.Remove(parsedData.Number);
            ids.Remove(parsedData.Number);

            UnfinishedOrdersList.Items.Refresh();
            OrdersList.Items.Refresh();

            MessageBox.Show("Odmowa została wysłana.", "Wysłanie wiadomości");
            var step = new Process().Next(StepNames.OrderMerged, DecisionType.Decline);
            ShitHelper.Publish(step.CurrentStep, new ProcessMessage
            {
                Step = step.CurrentStep,
            });

            SetEditorFromOrder(null);

        }
    }
}
