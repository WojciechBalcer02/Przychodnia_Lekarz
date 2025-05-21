using System.Windows;
using System.Windows.Controls;

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
            dUser.Text = Properties.Settings.Default.User;


        }
        public void dLoadContent(UserControl control)
        {

            if (RightContentPanel != null)
            {
                RightContentPanel.Children.Clear();
                RightContentPanel.Children.Add(control);
            }
        }
        private void dMyListBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (NavList.SelectedItem is ListBoxItem selectedItem)
            {
                string selectedText = selectedItem.Content.ToString();

                switch (selectedText)
                {
                    case "Strona główna":
                        dLoadContent(new dMainView());
                        break;
                    case "Umów wizytę":
                        dLoadContent(new MakeAppointment());
                        break;
                    case "Kalendarz":
                        dLoadContent(new Calendar());
                        break;
                    case "Cennik":
                        dLoadContent(new Pricing());
                        break;

                    case "Wiadomości":
                        dLoadContent(new Messages());
                        break;
                    case "Ustawienia konta":
                        dLoadContent(new Settings());
                        break;
                    default:
                        break;
                }

            }
              
        }
       
    }
}
