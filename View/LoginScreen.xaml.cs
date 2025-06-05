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
using System.Data.SqlClient;
using PolMedUMG.ViewModel;

namespace PolMedUMG.View
{
    /// <summary>
    /// Logika interakcji dla klasy LoginScreen.xaml
    /// </summary>
    public partial class LoginScreen : Window
    {
        public LoginScreen()
        {
            SessionManager.connStrSQL = "server=mysql-2e56cd6f-krzychu1324533-54ee.i.aivencloud.com;port=22051;uid=avnadmin;pwd=AVNS_OVYnYntZX_NGb7O_HZJ;database=defaultdb";
            InitializeComponent();
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnMinimize_Close(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnMinimize_FullScreen(object sender, RoutedEventArgs e)
        {
            if (WindowState != WindowState.Maximized) WindowState = WindowState.Maximized;
            else WindowState = WindowState.Normal;
        }

        public void LoadContent(UserControl control)
        {
            if (Content != null)
            {
                Content.Children.Clear();
                Content.Children.Add(control);
            }
        }

       
    }
}
