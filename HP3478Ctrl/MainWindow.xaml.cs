using System;
using System.Threading.Tasks;
using System.Windows;
using Ivi.Visa;

namespace HP3478Ctrl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }
        private void EnableButtons(bool state) {
            VerifyButton.IsEnabled = state;
            WriteButton.IsEnabled = state;
            ReadButton.IsEnabled = state;
        }
        private void ReadButton_Click(object sender, RoutedEventArgs e) {
            try {
                DataTextBox.Text = "Reading....";
                EnableButtons(false);
                string calString = HP3478A.ReadCalibration(AddressTextBox.Text);
                DataTextBox.Text = calString;
            } catch (Exception exc) {
                MessageBox.Show("An exception occured: \n\n " + exc.Message, "Error",
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
                    MessageBox.Show("Calibration DOES NOT match provided data.", "Calibration MISMATCH",
                        MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            } catch (Exception exc) {
                MessageBox.Show("An exception occured: \n\n " + exc.Message, "Error",
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
            } catch (Exception exc) {
                MessageBox.Show("An exception occured: \n\n " + exc.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
