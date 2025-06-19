using System.Windows;
using System.Windows.Input;

namespace PolMedUMG.View
{
    /// <summary>
    /// Logika interakcji dla klasy CalendarVisitDetails.xaml
    /// </summary>
    public partial class dCalendarVisitDetails : Window
    {
        public dCalendarVisitDetails(Model.Visit visit)
        {
            InitializeComponent();
            PatientName.Text = $"Pacjent: {visit.PatientName}";
            PESEL.Text = $"PESEL pacjenta: {visit.PESEL}";
            DateOfVisitText.Text = $"Data wizyty:\n{visit.DateOfVisit}";
            CauseOfVisitText.Text = $"Powód wizyty: {visit.CauseOfVisit}";
            AdditionalInfoText.Text = $"Dodatkowe informacje:\n{visit.AdditionalInfo?.Replace("\\n", Environment.NewLine)}";
            PhoneNumberText.Text = $"Numer telefonu: {visit.PhoneNumber}";
            ServiceNameText.Text = $"Nazwa usługi: {visit.ServiceName}";
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}