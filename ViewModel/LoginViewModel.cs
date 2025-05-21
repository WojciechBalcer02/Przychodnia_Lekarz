using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using PolMedUMG.Model;
using PolMedUMG.View;
using System.IO;
using MySql.Data.MySqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using MySqlX.XDevAPI;
using System.Data.Common;

namespace PolMedUMG.ViewModel
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _username;
        private string _password;

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(Login);
        }

        private void Login()
        {
            // Przechowuje informacj� dotycz�ce po��czenia z baz� danych
            string connStrSQL = "server=bwpd1lnfwwmd8zooiosa-mysql.services.clever-cloud.com;uid=uf9nqf7gizjdvxmm;pwd=mV5lVFodqkbncFJJnxqQ;database=bwpd1lnfwwmd8zooiosa";

            try
            {
                MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(connStrSQL);
                conn.Open();

                // Zapytanie do bazy o wybranego u�ytkownika
                MySqlCommand query = new MySqlCommand();
                query.Connection = conn;
                query.CommandText = @"SELECT COUNT(*) FROM users WHERE uid = @uid AND pwd = @pwd;";
                query.Parameters.AddWithValue("@uid", _username);
                query.Parameters.AddWithValue("@pwd", _password);
                int userCount = (int)(long)query.ExecuteScalar();
                conn.Close();
                if (userCount > 0)
                {
                    // Istnieje u�ytkownik z takimi danymi
                    try
                    {
                        conn.Open();
                        // Sprawdzamy jakiego typu jest u�ytkownik
                        MySqlCommand query2 = new MySqlCommand();
                        query2.Connection = conn;
                        query2.CommandText = @"SELECT acc_type FROM users WHERE uid = @uid AND pwd = @pwd;";
                        query2.Parameters.AddWithValue("@uid", _username);
                        query2.Parameters.AddWithValue("@pwd", _password);
                        String acctype = query2.ExecuteScalar().ToString();
                        Properties.Settings.Default.User = _username;
                        conn.Close();

                        // W zale�no�ci od typu u�ytkownika otwieramy odpowiednie okno
                        if (acctype.Equals("2"))
                        {
                            MessageBox.Show("Zalogowano jako admin");
                        }
                        else if (acctype.Equals("1"))
                        {
                            DoctorScreen doctorWindow = new DoctorScreen();
                            doctorWindow.Show();
                            Application.Current.MainWindow.Close();
                        }
                        else
                        {
                            PatientScreen patientWindow = new PatientScreen();
                            patientWindow.Show();
                            Application.Current.MainWindow.Close();
                        }

                    }
                    catch (MySql.Data.MySqlClient.MySqlException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Niepoprawny login lub has�o");
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }



        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}