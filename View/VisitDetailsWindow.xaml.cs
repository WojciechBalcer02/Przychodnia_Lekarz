using PolMedUMG.Model;
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
    public partial class VisitDetailsWindow : Window
    {
        public VisitDetailsWindow(Visit vis)
        {
            InitializeComponent();
            DoctorName.Text = $"Lekarz: \n{vis.Doctor}";
            DateOfVisitText.Text = $"Data wizyty:\n{vis.Date}";
            RoomNumber.Text = $"Gabinet lekarza:\n{vis.RoomNumber}";
            CauseOfVisitText.Text = $"Powód wizyty: \n{vis.causeOfVisit}";
            DescriptionText.Text = $"Opis wizyty: \n{vis.Description}";
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}