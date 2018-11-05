using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Interpreter;
using Models;
using ExcelLibrary.SpreadSheet;
using ExcelLibrary.CompoundDocumentFormat;
using System.Net.Mail;

namespace FirstDecision {
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class WorkerAssignmentWindow : Window {
        private const string actionRequiredString = "* ";
        private List<WorkerAssignmentData> assignments;
        private List<WorkerData> workersSource;
        private List<string> itemsSource;
        private OrderData data;
        private readonly ulong _tag;

        public WorkerAssignmentWindow(OrderData data, ulong tag) {
            InitializeComponent();

            this.data = data;
            _tag = tag;
            assignments = new List<WorkerAssignmentData>();
            foreach (WorkerData worker in Database.worker.SelectAll()) {
                WorkerAssignmentData assignment = new WorkerAssignmentData()
                {
                    worker = worker,
                    orders = new List<ProductData>()
                };
                assignments.Add(assignment);
            }

            workersSource = Database.worker.SelectAll();
            WorkerAssignment.ItemsSource = workersSource;

            itemsSource = new List<string>();
            foreach (ProductData product in data.Products) {
                itemsSource.Add(actionRequiredString + product.Product);
            }
            listView.ItemsSource = itemsSource;
        }

        private void ListSelectedItemChanged(object sender, MouseButtonEventArgs e) {
            ListViewItem selected = sender as ListViewItem;
            string selection = selected.Content.ToString().Replace(actionRequiredString, string.Empty);
            ProductData product = data.Products.Where(x => x.Product == selection).First();

            ProductName.Text = product.Product;
            ProductQty.Text = product.Quantity.ToString();
            try {
                WorkerAssignmentData assignment = assignments.Where(x => x.orders.Exists(y => y.Product == selection)).First();
                WorkerAssignment.SelectedItem = assignment.worker;
            } catch (Exception) {
                WorkerAssignment.SelectedIndex = -1;
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e) {
            if (!Validate()) {
                MessageBox.Show("Proszę o przypisanie pracownika do realizacji wszystkich części zamówienia!", "Nie przypisane elementy zamówienia");
            }
            else {
                string Body = "W załączniku otrzymał Pan/Pani dokument excel, który musi zostać uzupełniony.";
                string Subject = "Twoja część zamówienia";


                foreach (WorkerAssignmentData assignment in assignments)
                {
                    Database.assignments.InsertElement(assignment);
                    SendAssignment(assignment);

                    if (assignment.orders.Count > 0) { 
                        string fileName = assignment.worker.LastName + "_zamowienie.xls";
                        Workbook workbook = new Workbook();
                        Worksheet worksheet = new Worksheet("First Sheet");
                        worksheet.Cells[0, 0] = new Cell("id");
                        worksheet.Cells[0, 1] = new Cell("Product name");
                        worksheet.Cells[0, 2] = new Cell("Quantity");
                        worksheet.Cells[0, 3] = new Cell("Price");

                        for (int i = 0; i < assignment.orders.Count; i++)
                        {
                            worksheet.Cells[i + 1, 0] = new Cell(i);
                            worksheet.Cells[i + 1, 1] = new Cell(assignment.orders[i].Product);
                            worksheet.Cells[i + 1, 2] = new Cell(assignment.orders[i].Quantity);
                        }

                        workbook.Worksheets.Add(worksheet);
                        workbook.Save(fileName);

                        MailSender esender = new MailSender();
                        MailMessage MyMessage = new MailMessage();
                        MyMessage.Attachments.Add(new MailAttachment(fileName));

                        esender.Send(assignment.worker.Email, Body, Subject, MyMessage.Attachments);
                    }
                }

                ShitHelper.Model.BasicAck(_tag, false);
                //var step = new Process().Next(StepNames.OrderAccepted, DecisionType.Default);
                //ShitHelper.Publish(step.CurrentStep, new ProcessMessage
                //{
                //    Step = step.CurrentStep,
                //    //Attachments = new Dictionary<Data, object>()
                //});
                MessageBox.Show("Zamówienie zostało przekazane do dalszej realizacji.");
                GotoMainWindow();
            }
        }

        private void GotoMainWindow() {
            MainWindow window = new MainWindow();
            window.Show();
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) {
            GotoMainWindow();
        }

        private void WorkerAssignment_DropDownClosed(object sender, EventArgs e) {
            if (listView.SelectedItem == null || WorkerAssignment.SelectedIndex == -1) {
                WorkerAssignment.SelectedIndex = -1;
                return;
            }
            else {
                WorkerData worker = (WorkerData) WorkerAssignment.SelectedItem;
                WorkerAssignmentData assignment = assignments.Where(x => x.worker.Email == worker.Email).First();

                foreach (WorkerAssignmentData existingAssingment in assignments) {
                    existingAssingment.orders.RemoveAll(x => x.Product == ProductName.Text);
                }

                assignment.orders.Add(new ProductData() {Product=ProductName.Text, Quantity=int.Parse(ProductQty.Text)});

                int index = itemsSource.IndexOf(ProductName.Text);
                if (index == -1) index = itemsSource.IndexOf(actionRequiredString + ProductName.Text);
                itemsSource[index] = itemsSource[index].Replace(actionRequiredString, string.Empty);
                listView.Items.Refresh();
            }
        }

        private bool Validate() {
            foreach (ProductData product in data.Products) {
                if (!assignments.Exists(x => x.orders.Exists(y => y.Product == product.Product))) {
                    return false;
                }
            }

            return true;
        }

        private void SendAssignment(WorkerAssignmentData assignment) {
            string subject = "Proszę o realizację zamówienia.";
            StringBuilder builder = new StringBuilder();

            foreach (ProductData product in assignment.orders) {
                builder.Append(product.Product);
                builder.Append(", ilość : ");
                builder.Append(product.Quantity);
                builder.Append("\n");
            }

            MailSender esender = new MailSender();
            esender.Send(assignment.worker.Email, builder.ToString(), subject, null);
        }
    }
}