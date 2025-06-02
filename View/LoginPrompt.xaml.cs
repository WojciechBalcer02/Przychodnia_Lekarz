using System.Windows;
using System.Windows.Controls;
using PolMedUMG.ViewModel;

namespace PolMedUMG.View
{
    public partial class LoginPrompt : UserControl
    {
        public LoginPrompt()
        {
            InitializeComponent();
            this.DataContext = new LoginViewModel(this);
        }
        private void btnLog_In(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
            {
                vm.Password = passwordInput.Password;
            }
        }
        private void btn_pass_reset(object sender, RoutedEventArgs e)
        {
            var Conv = new PasswordRecovery();

            var parentWindow = Window.GetWindow(this) as LoginScreen;

            if (parentWindow != null)
            {
                parentWindow.LoadContent(Conv);
            }
        }
        private void btn_acc_create(object sender, RoutedEventArgs e)
        {
            var Conv = new AccountCreation();

            var parentWindow = Window.GetWindow(this) as LoginScreen;

            if (parentWindow != null)
            {
                parentWindow.LoadContent(Conv);
            }
        }
    }
}
