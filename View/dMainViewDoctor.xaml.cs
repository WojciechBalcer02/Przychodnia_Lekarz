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

       

        public dMainViewDoctor(Doctor doktor) 
        {
            InitializeComponent();
            DoctorNameText.Text = doktor.FirstName;
            DoctorSurnameText.Text = doktor.LastName;
            DoctorEmailText.Text = doktor.Email;
        }
        

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
