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
        private string _usernameChange;
        private string _passwordChange;
        private string _usernameRemove;



        public string UsernameChange
        {
            get => _usernameChange;
            set { _usernameChange = value; OnPropertyChanged(); }
        }

        public string PasswordChange
        {
            get => _passwordChange;
            set { _passwordChange = value; OnPropertyChanged(); }
        }

        public string UserRemove
        {
            get => _usernameRemove;
            set { _usernameRemove = value; OnPropertyChanged(); }
        }


        public ICommand PasswordAddFunc { get; }
        public ICommand RemoveUserFunc { get; }

        public AdminScreenViewModel()
        {
            PasswordAddFunc = new RelayCommand(AddPassword);
            RemoveUserFunc = new RelayCommand(RemoveUser);
        }


        public void AddPassword()
        {
            MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(SessionManager.connStrSQL);
            conn.Open();
            MySqlCommand query = new MySqlCommand();
            query.Connection = conn;
            query.CommandText = @"SELECT COUNT(*) FROM users WHERE uid = @uid;";
            query.Parameters.AddWithValue("@uid", _usernameChange);
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
                    string _hash = HashFunction.HashPassword(_passwordChange, _salt);
                    cmd.Parameters.AddWithValue("@uid", _usernameChange);
                    cmd.Parameters.AddWithValue("@hash", _hash);
                    cmd.Parameters.AddWithValue("@salt", _salt);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    MessageBox.Show("Ustawiono nowe hasło użytkownika");
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

        public void RemoveUser()
        {
            MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(SessionManager.connStrSQL);
            conn.Open();
            MySqlCommand query = new MySqlCommand();
            query.Connection = conn;
            query.CommandText = @"SELECT COUNT(*) FROM users WHERE uid = @uid;";
            query.Parameters.AddWithValue("@uid", _usernameRemove);
            int userCount = (int)(long)query.ExecuteScalar();
            conn.Close();
            if (userCount == 1)
            {
                try
                {
                    conn.Open();
                    MySqlCommand acctype = new MySqlCommand();
                    acctype.Connection = conn;
                    acctype.CommandText = "SELECT acc_type FROM users WHERE uid = @uid;";
                    acctype.Parameters.AddWithValue("@uid", _usernameRemove);
                    string accoutn_type = acctype.ExecuteScalar().ToString();
                    conn.Close();

                    if (accoutn_type == "2") { MessageBox.Show("Nie można usunąć konta administratora"); }
                    else
                    {
                        try
                        {
                            conn.Open();
                            MySqlCommand cmd = new MySqlCommand();
                            cmd.Connection = conn;
                            cmd.CommandText = "DELETE FROM users WHERE uid = @uid LIMIT 1;";
                            cmd.Parameters.AddWithValue("@uid", _usernameRemove);
                            cmd.ExecuteNonQuery();
                            conn.Close();
                            MessageBox.Show("Usunięto użytkownika: "+ _usernameRemove);
                        }
                        catch (MySql.Data.MySqlClient.MySqlException ex)
                        {
                            MessageBox.Show(ex.Message);
                        }

                    }
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else if (userCount < 1)
            {
                {
                    MessageBox.Show("Użytkownik nie istnieje");
                }


            }
        }
            
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
