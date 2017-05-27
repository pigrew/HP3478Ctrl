using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Ivi.Visa;

namespace HP3478Ctrl {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }
        public Task<string> GetCalData() {
            return Task.Run(() => HP3478A.ReadCalibration(AddressTextBox.Text));
        }
        private void EnableButtons(bool state) {
            VerifyButton.IsEnabled = state;
            WriteButton.IsEnabled = state;
            ReadButton.IsEnabled = state;
        }
        private async void ReadButton_Click(object sender, RoutedEventArgs e) {
            try {
                DataTextBox.Text = "Reading....";
                EnableButtons(false);
                string calString = await GetCalData();
                DataTextBox.Text = calString;
            } catch (VisaException exc) {
                MessageBox.Show("VISA error happened: \n\n " + exc.Message, "VISA Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            } finally {
                EnableButtons(true);
            }
        }

        private void VerifyButton_Click(object sender, RoutedEventArgs e) {
            try {
                string oldCal = DataTextBox.Text;
                string currentCal = HP3478A.ReadCalibration(AddressTextBox.Text);
                if (HP3478A.CalibrationsEqual(oldCal, currentCal)) {
                    MessageBox.Show("Calibration matches provided data.", "Calibration Verified",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                } else {
                    MessageBox.Show("Calibration DOES NOT match provided data.", "Calibration MISTATCH",
                        MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            } catch (VisaException exc) {
                MessageBox.Show("VISA error happened: \n\n " + exc.Message, "VISA Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            } finally {
                EnableButtons(true);
            }
        }

        private void WriteButton_Click(object sender, RoutedEventArgs e) {
            try {
                string calStr = DataTextBox.Text;
                bool valid = HP3478A.IsValidCalString(calStr);
                if (!valid) {
                    MessageBox.Show("Provided calibration string is NOT valid.", "Calibration invalid",
                        MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                HP3478A.WriteCalibration(AddressTextBox.Text, calStr);
            } catch (FormatException fex) {
                MessageBox.Show("Formatting error happened: \n\n " + fex.Message, "Format Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            } catch (VisaException exc) {
                MessageBox.Show("VISA error happened: \n\n " + exc.Message, "VISA Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
