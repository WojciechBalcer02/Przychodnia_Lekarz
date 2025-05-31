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
            SessionManager.connStrSQL = "server=mysql-2e56cd6f-krzychu1324533-54ee.i.aivencloud.com;port=22051;uid=avnadmin;pwd=AVNS_OVYnYntZX_NGb7O_HZJ;database=defaultdb";
            //      nowy                "server=bb97fob4mmaybcvttjjk-mysql.services.clever-cloud.com;uid=uirqsom4re7q6gwn;pwd=ODh2O0u6eNj3uUkXsLYO;database=bb97fob4mmaybcvttjjk"
            //      stary               "server=bwpd1lnfwwmd8zooiosa-mysql.services.clever-cloud.com;uid=uf9nqf7gizjdvxmm;pwd=mV5lVFodqkbncFJJnxqQ;database=bwpd1lnfwwmd8zooiosa"
            SessionManager.CurrentUsername = _username;
            try
            {
                MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(SessionManager.connStrSQL);
                conn.Open();

                // Zapytanie do bazy o wybranego użytkownika
                MySqlCommand query = new MySqlCommand();
                query.Connection = conn;
                query.CommandText = @"SELECT COUNT(*) FROM users WHERE uid = @uid;";
                query.Parameters.AddWithValue("@uid", _username);
                int userCount = (int)(long)query.ExecuteScalar();
                conn.Close();

                conn.Open();//Zapytanie do bazy o hash oraz salt danego użytkownika
                MySqlCommand hashcheck = new MySqlCommand();
                hashcheck.Connection = conn;
                hashcheck.CommandText = @"SELECT pwdHash,pwdSalt FROM users WHERE uid = @uid;";
                hashcheck.Parameters.AddWithValue("@uid", _username);
                MySqlDataAdapter adapter = new MySqlDataAdapter(hashcheck);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                conn.Close();
                if (userCount > 0 && HashFunction.VerifyPassword(_password, dataTable.Rows[0].Field<string>(0), dataTable.Rows[0].Field<byte[]>(1)))
                {
                    // Istnieje dany użytkownik i hasło zgadza się z tym w bazie
                    try
                    {
                        conn.Open();
                        // Sprawdzamy jakiego typu jest u�ytkownik
                        MySqlCommand query2 = new MySqlCommand();
                        query2.Connection = conn;
                        query2.CommandText = @"SELECT acc_type FROM users WHERE uid = @uid;";
                        query2.Parameters.AddWithValue("@uid", _username);
                        String acctype = query2.ExecuteScalar().ToString();

                        SessionManager.accType = acctype;

                        // wyslanie do bazy daty logowania
                        MySqlCommand updateLoginTime = new MySqlCommand();
                        updateLoginTime.Connection = conn;
                        updateLoginTime.CommandText = @"UPDATE users SET last_login = @loginTime WHERE uid = @uid;";
                        updateLoginTime.Parameters.AddWithValue("@loginTime", DateTime.Now);
                        updateLoginTime.Parameters.AddWithValue("@uid", _username);
                        updateLoginTime.ExecuteNonQuery();

                        conn.Close();
                        // W zale�no�ci od typu u�ytkownika otwieramy odpowiednie okno
                        if (acctype.Equals("2"))
                        {
                            AdminScreen adminWindow = new AdminScreen();
                            adminWindow.Show();
                            Application.Current.MainWindow.Close();
                        }
                        else if (acctype.Equals("1"))
                        {
                            DoctorScreen doctorWindow = new DoctorScreen();
                            doctorWindow.Show();
                            Application.Current.MainWindow.Close();
                        }
                        else if (acctype.Equals("0"))
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
                    MessageBox.Show("Niepoprawny login lub hasło");
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