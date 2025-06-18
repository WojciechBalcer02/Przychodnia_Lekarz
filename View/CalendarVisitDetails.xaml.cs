using System.Windows;

namespace PolMedUMG.View
{
    /// <summary>
    /// Logika interakcji dla klasy CalendarVisitDetails.xaml
    /// </summary>
    public partial class CalendarVisitDetails : Window
    {
        public CalendarVisitDetails(Model.Visit visit)
        {
            InitializeComponent();

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
    }
}