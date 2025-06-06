using PolMedUMG.Model;
using PolMedUMG.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Shapes;

namespace PolMedUMG.View
{
    public partial class dMainViewPatient : Window
    {
        private Patient currentPatient;

        public dMainViewPatient(Patient patient)
        {
            InitializeComponent();
            currentPatient = patient;
            PatientNameText.Text = "Imie: " + patient.FirstName;
            PatientSurnameText.Text = "Nazwisko: " + patient.LastName;
            PatientEmailText.Text = "Email: " + patient.Email;
            PatientPhoneNumberText.Text = "Numer telefonu: " + patient.phoneNumber;
            PatientAddressText.Text = "Adres: " + patient.Address;
            PatientPeselText.Text = "PESEL: "+ patient.PESEL;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (currentPatient == null) return;
            dMainViewPatientConv LookupPatientConvWindow = new dMainViewPatientConv(currentPatient);
            LookupPatientConvWindow.Show();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}
