using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Models;

namespace FirstDecision {
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class WorkerAssignmentWindow : Window {

        private List<WorkerAssignmentData> assignments;
        private List<WorkerData> workersSource;
        private List<string> itemsSource;
        private OrderData data;

        public WorkerAssignmentWindow(OrderData data) {
            InitializeComponent();

            this.data = data;
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
                itemsSource.Add("* " + product.Product);
            }
            listView.ItemsSource = itemsSource;
        }

        private void ListSelectedItemChanged(object sender, MouseButtonEventArgs e) {
            ListViewItem selected = sender as ListViewItem;
            string selection = selected.Content.ToString().Replace("* ", "");
            ProductData product = data.Products.Where(x => x.Product == selection).First();

            ProductName.Text = product.Product;
            ProductQty.Text = product.Quantity.ToString();
            try {
                WorkerAssignmentData assignment = assignments.Where(x => x.orders.Exists(y => y.Product == selection)).First();
                WorkerAssignment.SelectedItem = workersSource.Where(x => x.Email == assignment.worker.Email).First();
                int index = itemsSource.IndexOf(product.Product);
                itemsSource[index] = itemsSource[index].Replace("* ", "");
                listView.Items.Refresh();
            } catch (Exception) {
                WorkerAssignment.SelectedIndex = -1;
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e) {
            if (!Validate()) {
                MessageBox.Show("Proszę o przypisanie pracownika do realizacji wszystkich części zamówienia!", "Nie przypisane elementy zamówienia");
            }
            else {
                foreach (WorkerAssignmentData assignment in assignments) {
                    Database.assignments.InsertElement(assignment);
                    SendAssignment(assignment);
                }

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