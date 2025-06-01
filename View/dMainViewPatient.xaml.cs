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
using System.Windows.Shapes;

namespace PolMedUMG.View
{
    /// <summary>
    /// Interaction logic for dMainViewDoctor.xaml
    /// </summary>
    public partial class dMainViewPatient : Window
    {



        public dMainViewPatient(Patient patient)
        {
            InitializeComponent();
            PatientNameText.Text = "Imie: "+patient.FirstName;
            PatientSurnameText.Text = "Nazwisko: "+patient.LastName;
            PatientEmailText.Text = "Email: "+patient.Email;
            PatientPhoneNumberText.Text = "Numer telefonu: "+patient.phoneNumber;
            PatientAddressText.Text = "Adres: "+patient.Address;

        }


        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

