using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Interpreter;
using Microsoft.Win32;
using Models;
using Newtonsoft.Json;

namespace FirstDecision {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private OrderData order = null;
        private ulong? messageId;

        public MainWindow() {
            InitializeComponent();
            Process.MyStep = StepNames.OrderReceived;
            Database.start();
            var model = ShitHelper.Model;
            var consumer = new CommonMessageHandler(model);
            ShitHelper.Handler = new FirstDecisionHandler();
            UIMessageUpdater.UpdaterWithUi.UpdateUi = UpdateUi;
            AcceptHandler.UpdaterWithUi.UpdateUi = ChooseWorkers;
            model.BasicConsume(StepNames.OrderReceived, false, String.Empty, false, false, null, consumer);
            model.BasicConsume(StepNames.OrderAccepted, false, String.Empty, false, false, null, consumer);
            model.BasicConsume(StepNames.OrderDeclined, false, String.Empty, false, false, null, consumer);
        }

        private void Window_Drop(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                string[] files = (string[]) e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 1) {
                    MessageBox.Show("Obsługa wielu plików na raz nie jest możliwa. \nProsimy o przetwarzanie pojedynczych plików.", "Przetwarzanie wielu plików.", MessageBoxButton.OK);
                }
                else {
                    LoadFromFile(files[0]);
                }
            }
        }

        private void UpdateUi(OrderData newOrder, ulong? id)
        {
            Dispatcher.Invoke(() =>
            {
                order = newOrder;
                dataGrid.ItemsSource = order.Products;
                dataGrid.Columns[0].Width = 316;
                dataGrid.Columns[1].Width = 65;

                nameBox.Text = order.Name + " " + order.LastName;
                emailBox.Text = order.Email;
                numberBox.Text = order.Number.ToString();
                messageId = id;
            });
        }

        private void LoadFromFile(string path) {
            FileStream file = null;
            StreamReader reader = null;

            try {
                string content = "";
                file = new FileStream(path, FileMode.Open);
                reader = new StreamReader(file);
                content = reader.ReadToEnd();
                order = JsonConvert.DeserializeObject<OrderData>(content);
                UpdateUi(order, null);
                reader.Close();
            }
            catch (Exception) {
                if (file != null) {
                    file.Close();
                }

                if (reader != null) {
                    reader.Close();
                }

                ResetFields();
                MessageBox.Show("Format wskazanego pliku nie jest obsługiwany lub wskazano niewłaściwy plik zamówienia.", "Nie wspierany typ pliku", MessageBoxButton.OK);
            }
        }

        private void loadFromFile_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.ShowDialog();

            if (dialog.FileName.Length > 0) {
                LoadFromFile(dialog.FileName);
            }
        }

        private void rejectButton_Click(object sender, RoutedEventArgs e) {
            if (order != null) {
                string notify = "Automatyczny email z odmową na zamówienie " + numberBox.Text + " zostaje wysłany." + " Powód, dla którego projekt został odrzucony: " + commentTextBox.Text;

                MessageBox.Show(notify, "Odmowa", MessageBoxButton.OK);
                
                PushProcess(DecisionType.Decline, new Dictionary<Data, object>
                {
                    {Data.DenialReason, commentTextBox.Text}
                });
                ResetFields();
            }
            else {
                MessageBox.Show("Najpierw załaduj plik!", "Brak zamówienia.", MessageBoxButton.OK);
            }
        }

        private void PushProcess(DecisionType decision, Dictionary<Data, object> attachs)
        {
            if (messageId.HasValue) ShitHelper.Model.BasicAck(messageId.Value, false);
            var nextStep = new Process().Next(Process.MyStep, decision);
            ShitHelper.Publish(nextStep.CurrentStep, new ProcessMessage
            {
                Step = nextStep.CurrentStep,
                Attachments = attachs
            });
        }

        private void ChooseWorkers(OrderData sentOrder, ulong? tag)
        {
            Dispatcher.Invoke(() =>
            {
                messageId = tag;
                order = sentOrder;
                WorkerAssignmentWindow workerAssignmentWindow = new WorkerAssignmentWindow(order, messageId.Value);
                workerAssignmentWindow.Show();
                //przejście do okienka z wyborem pracowników
                ResetFields();
                Close();
            });
        }

        private void acceptButton_Click(object sender, RoutedEventArgs e) {
            if (order != null)
            {
                PushProcess(DecisionType.Ok, new Dictionary<Data, object>
                {
                    {Data.OrderDataFile, JsonConvert.SerializeObject(order) }
                });
            }
            else {
                MessageBox.Show("Najpierw załaduj plik!", "Brak zamówienia.", MessageBoxButton.OK);
            }
        }

        private void ResetFields() {
            order = null;
            dataGrid.ItemsSource = null;
            nameBox.Text = string.Empty;
            emailBox.Text = string.Empty;
            numberBox.Text = string.Empty;
            commentTextBox.Text = string.Empty;
        }
    }
}
