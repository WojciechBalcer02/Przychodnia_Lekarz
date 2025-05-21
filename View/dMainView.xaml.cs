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
    public partial class dMainView : UserControl
    {
        public dMainView()
        {
            InitializeComponent();
        }
        private void dtxtPatientSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (dtxtPatientSearch.Text != "") { dLblPeselPlaceholder.Visibility = Visibility.Hidden; }
            else { dLblPeselPlaceholder.Visibility = Visibility.Visible; }
        }
        private void dtxtDoctorSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (dtxtDoctorSearch.Text != "") { dLblIdPlaceholder.Visibility = Visibility.Hidden; }
            else { dLblIdPlaceholder.Visibility = Visibility.Visible; }
        }        
    }
}
