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
    public partial class dMainViewDoctor : Window
    {

        private dDoctor currentDoctor;

        public dMainViewDoctor(dDoctor doktor) 
        {
            InitializeComponent();
            currentDoctor = doktor;
            DoctorNameText.Text = "Imie: "+doktor.FirstName;
            DoctorSurnameText.Text = "Nazwisko: "+doktor.LastName;
            DoctorEmailText.Text = "Email: "+doktor.Email;
            DoctorRoomNumberText.Text = "Numer gabientu: "+doktor.roomNumber;
            DoctorPhoneNumberText.Text = "Numer telefonu: "+doktor.phoneNumber;
            DoctorSpecializationText.Text = "Specjalizacja: "+doktor.specialization;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (currentDoctor == null) return;
            else if ((currentDoctor != null) && (currentDoctor.uid != SessionManager.CurrentUsername))
            {
                dMainViewDoctorConv LookupDoctorConvWindow = new dMainViewDoctorConv(currentDoctor);
                LookupDoctorConvWindow.Show();
            }
            else 
            {
                MessageBox.Show("Nie można rozpocząć rozmowy z samym sobą","Błąd");
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
