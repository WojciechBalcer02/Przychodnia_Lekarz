using MySql.Data.MySqlClient;
using PolMedUMG.ViewModel;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
namespace PolMedUMG.View
{
    /// <summary>
   /// Logika interakcji dla klasy Calendar.xaml
   /// </summary>
    public partial class Pricing : UserControl
    {
        public Pricing()
        {
            InitializeComponent();

            var vm = new PricingViewModel();
            vm.LoadData();
            this.DataContext = vm;
        }
    }
}