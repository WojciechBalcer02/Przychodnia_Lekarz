using PolMedUMG.ViewModel;
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

namespace PolMedUMG.View
{
    /// <summary>
    /// Interaction logic for dMainView.xaml
    /// </summary>
    /// 

    public class Doctor
    {

        public string FirstName;
        public string LastName;
        public string Email;
        /*public string Room;
         public string picture;*/



        public Doctor(string firstname, string lastname, string email)
        {
            FirstName = firstname;
            LastName = lastname;
            Email = email;
           /* Room = room;*/
        }
    }

    public class Patient
    {

        public string FirstName;
        public string LastName;
        public string Email;
        /*public string PhoneNumber;
         public string picture;               
         */



        public Patient(string firstname, string lastname, string email)
        {
            FirstName = firstname;
            LastName = lastname;
            Email = email;

        }
    }

    public partial class dMainView : UserControl
    {
        public dMainView()
        {
            InitializeComponent();
            DataContext = new dMainViewViewModel();
        }
        private void txtPatientSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtPatientSearch.Text != "") { LblPeselPlaceholder.Visibility = Visibility.Hidden; }
            else { LblPeselPlaceholder.Visibility = Visibility.Visible; }
        }
        private void txtDoctorSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtDoctorSearch.Text != "") { LblIdPlaceholder.Visibility = Visibility.Hidden; }
            else { LblIdPlaceholder.Visibility = Visibility.Visible; }
        }  
        

    }
}
