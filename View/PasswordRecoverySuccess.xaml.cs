using System.Windows;
using System.Windows.Controls;
using PolMedUMG.ViewModel;

namespace PolMedUMG.View
{
    public partial class PasswordRecoverySuccess : UserControl
    {
        private string _email;


        public PasswordRecoverySuccess(string email)
        {
            InitializeComponent();

            _email = email;

            InfoTextBlock.Text = $"Na adres {email} wysłaliśmy wiadomość, która udostępniła nowe, tymczasowe hasło ważne przez 15 minut.";

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
        public void resend_password(object sender, RoutedEventArgs e)
        {
            PasswordRecovery.SendNewPassword(_email);
        }
    }
}
