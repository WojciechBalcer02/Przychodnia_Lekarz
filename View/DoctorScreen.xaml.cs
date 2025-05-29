using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PolMedUMG.View
{
    /// <summary>
    /// Interaction logic for DoctorScreen.xaml
    /// </summary>
    public partial class DoctorScreen : Window
    {
        public DoctorScreen( )
        {
            InitializeComponent();
            txtblckUserName.Text = SessionManager.CurrentUsername;

            DataContext = this;

            LoadContent(new dMainView()); //Ładowanie domyślnego widoku
        }
        public void LoadContent(UserControl control)
        {

            if (RightContentPanel != null)
            {
                RightContentPanel.Children.Clear();
                RightContentPanel.Children.Add(control);
            }
        }
        private void MyListBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (NavList.SelectedItem is ListBoxItem selectedItem)
            {
                string selectedText = selectedItem.Content.ToString();

                switch (selectedText)
                {
                    case "Strona główna":
                        LoadContent(new dMainView());
                        break;
                    case "Umów wizytę":
                        LoadContent(new dMakeAppointment());
                        break;
                    case "Kalendarz":
                        LoadContent(new dCalendar());
                        break;
                    case "Cennik usług":
                        LoadContent(new dPricing());
                        break;
                    case "Wiadomości":
                        LoadContent(new dMessages());
                        break;
                    case "Ustawienia konta":
                        LoadContent(new dSettings());
                        break;
                    default:
                        break;
                }

            }
              
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e) //Minimalizuje ekran 
        {
            WindowState = WindowState.Minimized;
        }

        private void btnMinimize_Close(object sender, RoutedEventArgs e) //Wyłącza aplikację 
        {
            Application.Current.Shutdown();
        }

        private void btnMinimize_FullScreen(object sender, RoutedEventArgs e) //
        {
            if (WindowState != WindowState.Maximized) WindowState = WindowState.Maximized;
            else WindowState = WindowState.Normal;
        }

    }
}
