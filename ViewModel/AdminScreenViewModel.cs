using MySql.Data.MySqlClient;
using PolMedUMG.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PolMedUMG.ViewModel
{
     public class AdminScreenViewModel : INotifyPropertyChanged
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

        public ICommand PasswordAdd { get; }

        public AdminScreenViewModel()
        {
            PasswordAdd = new RelayCommand(AddPassword);
        }


        public void AddPassword()
        {
            MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(SessionManager.connStrSQL);
            conn.Open();
            MySqlCommand query = new MySqlCommand();
            query.Connection = conn;
            query.CommandText = @"SELECT COUNT(*) FROM users WHERE uid = @uid;";
            query.Parameters.AddWithValue("@uid", _username);
            int userCount = (int)(long)query.ExecuteScalar();
            conn.Close();
            if (userCount > 0)
            {
                try
                {
                    conn.Open();
                     
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = @"UPDATE users SET pwdHash = @hash, pwdSalt = @salt WHERE uid = @uid ";
                    byte[] _salt = HashFunction.GenerateSalt();
                    string _hash = HashFunction.HashPassword(_password, _salt);
                    cmd.Parameters.AddWithValue("@uid", _username);
                    cmd.Parameters.AddWithValue("@hash", _hash);
                    cmd.Parameters.AddWithValue("@salt", _salt);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    MessageBox.Show("Ustawiono hasło użytkownika");
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Użytkownik nie istnieje");
            }


        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
