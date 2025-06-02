using PolMedUMG.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PolMedUMG.Model;
using System.Diagnostics;

namespace PolMedUMG.View
{
    /// <summary>
    /// Logika interakcji dla klasy Specialists.xaml
    /// </summary>
    public partial class Specialists : UserControl
    {
        public Specialists()
        {
            InitializeComponent();
            DataContext = new SpecialistsViewModel();
        }

        private void Specialist_Click(object sender, MouseButtonEventArgs e)
        {
            // Pobierz kliknięty kafelek i model Specialist:
            if (!(sender is FrameworkElement element && element.DataContext is Specialist spec))
                return;

            // Załaduj historię rozmów z tego lekarzem:
            var repo = new MessageRepository();
            // GetMessagesFrom zwraca listę ConvMessages posortowaną od najstarszych
            var history = repo
                .GetMessagesFrom(spec.Name, SessionManager.CurrentUsername)
                .OrderBy(m => m.Date)
                .ToList();

            // parametry do MessagesOpenConv: jeśli brak wiadomości - ustaw domyślnie
            var lastMsg = history.LastOrDefault();
            var date = lastMsg?.Date ?? DateTime.Now;
            var img = lastMsg?.DoctorImage ?? "default_doctor.png";

            var chatControl = new MessagesOpenConv(
                date,
                spec.Name,
                img,
                lastMsg
            );

            var parentWindow = Window.GetWindow(this) as PatientScreen;
            if (parentWindow != null)
            {
                // Zmiana zaznaczenia opcji w menu przed przeniesieniem do widoku czatu
                foreach (ListBoxItem item in parentWindow.NavList.Items)
                {
                    if (item.Content.ToString() == "Wiadomości")
                    {
                        parentWindow.NavList.SelectedItem = item;
                        break;
                    }
                }

                parentWindow.LoadContent(chatControl);
            }
            else
            {
                MessageBox.Show("Nie można załadować czatu, spróbuj ponownie później lub skontaktuj się z administratorem", "Wystapił błąd");
                Debug.WriteLine("Nie znaleziono PatientScreen, nie można załadować czatu.");
            }
        }

    }
}
