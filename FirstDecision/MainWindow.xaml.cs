using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using Models;
using Newtonsoft.Json;

namespace FirstDecision {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private OrderData order = null;

        public MainWindow() {
            InitializeComponent();
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

        private void LoadFromFile(string path) {
            FileStream file = null;
            StreamReader reader = null;

            try {
                string content = "";
                file = new FileStream(path, FileMode.Open);
                reader = new StreamReader(file);
                content = reader.ReadToEnd();
                order = JsonConvert.DeserializeObject<OrderData>(content);

                dataGrid.ItemsSource = order.Products;
                dataGrid.Columns[0].Width = 316;
                dataGrid.Columns[1].Width = 65;

                nameBox.Text = order.Name + " " + order.LastName;
                emailBox.Text = order.Email;
                numberBox.Text = order.Number.ToString();
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
                MessageBox.Show("Format wskazanego pliku nie jest obsługiwany. Obsługiwany format plików to .JSON.", "Nie wspierany typ pliku", MessageBoxButton.OK);
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
                MessageBox.Show("Tu kazać klientowi iść w cholerę.", "GTFO", MessageBoxButton.OK);
                ResetFields();
            }
            else {
                MessageBox.Show("Najpierw załaduj plik!", "Brak zamówienia.", MessageBoxButton.OK);
            }
        }

        private void acceptButton_Click(object sender, RoutedEventArgs e) {
            if (order != null) {
                MessageBox.Show("Tu wysłać zamówienie dalej.", "KKTHXBYE", MessageBoxButton.OK);
                ResetFields();
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
        }
    }
}
