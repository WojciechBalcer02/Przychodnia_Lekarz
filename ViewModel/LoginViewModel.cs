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
using System.Diagnostics;

namespace PolMedUMG.ViewModel
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _username;
        private string _password;


        private LoginPrompt _view;

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

        private string _errorMessage;

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        public ICommand LoginCommand { get; }

        public LoginViewModel(LoginPrompt view)
        {
            LoginCommand = new RelayCommand(Login);
            _view = view;
        }

        private void Login()
        {

            //      ich baza                 "server=bb97fob4mmaybcvttjjk-mysql.services.clever-cloud.com;uid=uirqsom4re7q6gwn;pwd=ODh2O0u6eNj3uUkXsLYO;database=bb97fob4mmaybcvttjjk"
            //      nasza baza               "server=server=mysql-2e56cd6f-krzychu1324533-54ee.i.aivencloud.com;port=22051;uid=avnadmin;pwd=AVNS_OVYnYntZX_NGb7O_HZJ;database=defaultdb"

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
                if (userCount > 0)
                {
                    // Istnieje dany użytkownik
                    try
                    {
                        conn.Open();
                        //Zapytanie do bazy o hash oraz salt danego użytkownika
                        MySqlCommand hashcheck = new MySqlCommand();
                        hashcheck.Connection = conn;
                        hashcheck.CommandText = @"SELECT pwdHash,pwdSalt FROM users WHERE uid = @uid;";
                        hashcheck.Parameters.AddWithValue("@uid", _username);
                        MySqlDataAdapter adapter = new MySqlDataAdapter(hashcheck);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        Boolean hashCheckPassed = HashFunction.VerifyPassword(_password, dataTable.Rows[0].Field<string>(0), dataTable.Rows[0].Field<byte[]>(1));

                        // Sprawdzamy jakiego typu jest u�ytkownik
                        MySqlCommand query2 = new MySqlCommand();
                        query2.Connection = conn;
                        query2.CommandText = @"SELECT acc_type FROM users WHERE uid = @uid;";
                        query2.Parameters.AddWithValue("@uid", _username);
                        String acctype = query2.ExecuteScalar().ToString();

                        SessionManager.accType = acctype;

                        MySqlCommand recpass = new MySqlCommand();
                        recpass.Connection = conn;
                        recpass.CommandText = @"SELECT newPass, dateOfGeneration FROM PassRecovery WHERE username = @username ORDER BY dateOfGeneration DESC LIMIT 1;";
                        recpass.Parameters.AddWithValue("@username", _username);
                        string recoveryPassword = null;
                        DateTime dateOfGeneration = DateTime.MinValue;

                        using (var reader = recpass.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                recoveryPassword = reader["newPass"].ToString();
                                dateOfGeneration = Convert.ToDateTime(reader["dateOfGeneration"]);

                            }
                            else
                            {
                                recoveryPassword = null;
                                dateOfGeneration = DateTime.MinValue;
                            }
                        }
                        TimeSpan timeSinceGeneration = DateTime.Now - dateOfGeneration;
                        if (_password == recoveryPassword && timeSinceGeneration.TotalMinutes <= 15)
                        {
                            Debug.WriteLine("działa fantastycznie");
                        }

                        if (hashCheckPassed == true || (_password == recoveryPassword && timeSinceGeneration.TotalMinutes <= 15))
                        {

                            // wyslanie do bazy daty logowania
                            MySqlCommand updateLoginTime = new MySqlCommand();
                            updateLoginTime.Connection = conn;
                            updateLoginTime.CommandText = @"UPDATE users SET last_login = @loginTime WHERE uid = @uid;";
                            updateLoginTime.Parameters.AddWithValue("@loginTime", DateTime.Now);
                            updateLoginTime.Parameters.AddWithValue("@uid", _username);
                            updateLoginTime.ExecuteNonQuery();

                            if (_password == recoveryPassword && timeSinceGeneration.TotalMinutes <= 15)
                            {
                                MySqlCommand deleteRecoveryPasswords = new MySqlCommand();
                                deleteRecoveryPasswords.Connection = conn;
                                deleteRecoveryPasswords.CommandText = @"DELETE FROM PassRecovery WHERE username = @uid;";
                                deleteRecoveryPasswords.Parameters.AddWithValue("@uid", _username);
                                deleteRecoveryPasswords.ExecuteNonQuery();

                                MySqlCommand updatePassword = new MySqlCommand();
                                updatePassword.Connection = conn;
                                updatePassword.CommandText = @"UPDATE users SET pwdHash = @Hash, pwdSalt = @salt WHERE uid = @uid;";
                                byte[] recoveryHash = HashFunction.GenerateSalt();
                                updatePassword.Parameters.AddWithValue("@Hash", HashFunction.HashPassword(_password, recoveryHash));
                                updatePassword.Parameters.AddWithValue("@salt", recoveryHash);
                                updatePassword.Parameters.AddWithValue("@uid", _username);
                                updatePassword.ExecuteNonQuery();

                                MessageBox.Show("Twoje domyślne hasło zostało zmienione!");
                            }

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

                        else 
                        {
                            if (hashCheckPassed==false)
                            {
                                ErrorMessage = "Podano złe hasło.";
                            }
                            if (_password == recoveryPassword && timeSinceGeneration.TotalMinutes > 15) // przedawnione haslo
                            {
                                ErrorMessage = "Hasło przywracające uległo przedawnieniu";
                                MySqlCommand deleteRecoveryPasswords = new MySqlCommand();
                                deleteRecoveryPasswords.Connection = conn;
                                deleteRecoveryPasswords.CommandText = @"DELETE FROM PassRecovery WHERE username = @uid;";
                                deleteRecoveryPasswords.Parameters.AddWithValue("@uid", _username);
                                deleteRecoveryPasswords.ExecuteNonQuery();
                            }

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
