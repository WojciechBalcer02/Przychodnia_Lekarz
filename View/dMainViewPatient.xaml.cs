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

        private void SendMessage(object sender, RoutedEventArgs e)//Otwiera okienko wiadomości z pacjentem
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

        private void MakeVisit(object sender, RoutedEventArgs e) //Otwiera ekran tworzenia wizyty
        {
            try
            {
                DoctorScreen mainWindow = null;
                foreach (Window window in Application.Current.Windows)
                {
                    if (window is DoctorScreen doctorScreen)
                    {
                        mainWindow = doctorScreen;
                        break;
                    }
                }
                if (mainWindow != null)
                {
                    mainWindow.LoadContent(new dMakeAppointment());
                    UpdateNavSelection(mainWindow);
                    mainWindow.Activate();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd: {ex.Message}", "Błąd");
            }
        }

        private void UpdateNavSelection(DoctorScreen window)
        {
            var field = typeof(DoctorScreen).GetField("NavList",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            if (field?.GetValue(window) is ListBox navList)
            {
                foreach (ListBoxItem item in navList.Items)
                {
                    if (item.Content?.ToString() == "Umów wizytę")
                    {
                        navList.SelectedItem = item;
                        break;
                    }
                }
            }
        }
    }
}
