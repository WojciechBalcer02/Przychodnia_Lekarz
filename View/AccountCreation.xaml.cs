using System.Windows;
using System.Windows.Controls;
using PolMedUMG.ViewModel;

namespace PolMedUMG.View
{
    public partial class AccountCreation : UserControl
    {
        public AccountCreation()
        {
            InitializeComponent();
        }
        private void btnPatient(object sender, RoutedEventArgs e)
        {
            var Conv = new PatientAccountCreation();

            var parentWindow = Window.GetWindow(this) as LoginScreen;

            if (parentWindow != null)
            {
                parentWindow.LoadContent(Conv);
            }
        }
        private void btnDoctor(object sender, RoutedEventArgs e)
        {

        }
        private void go_Back(object sender, RoutedEventArgs e)
        {
            var Conv = new LoginPrompt();

            var parentWindow = Window.GetWindow(this) as LoginScreen;

            if (parentWindow != null)
            {
                parentWindow.LoadContent(Conv);
            }
        }
    }
}
