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
            AddDoctorFunc = new RelayCommand(AddDoctor);
            RemoveDoctorFunc = new RelayCommand(RemoveDoctor);
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


        //Lekarz


        private string _doctorPassword;
        private string _doctorSpecialty;
        private string _doctorToRemove;
        private string _doctorFirstName;
        private string _doctorLastName;
        private string _doctorEmail;
        private string _doctorPhone;
        private string _doctorOffice;
        private string _doctorUsername;

        public string DoctorFirstName
        {
            get => _doctorFirstName;
            set { _doctorFirstName = value; OnPropertyChanged(); }
        }

        public string DoctorLastName
        {
            get => _doctorLastName;
            set { _doctorLastName = value; OnPropertyChanged(); }
        }

        public string DoctorEmail
        {
            get => _doctorEmail;
            set { _doctorEmail = value; OnPropertyChanged(); }
        }

        public string DoctorPhone
        {
            get => _doctorPhone;
            set { _doctorPhone = value; OnPropertyChanged(); }
        }

        public string DoctorOffice
        {
            get => _doctorOffice;
            set { _doctorOffice = value; OnPropertyChanged(); }
        }

        public string DoctorPassword
        {
            get => _doctorPassword;
            set { _doctorPassword = value; OnPropertyChanged(); }
        }

        public string DoctorSpecialty
        {
            get => _doctorSpecialty;
            set { _doctorSpecialty = value; OnPropertyChanged(); }
        }

        public string DoctorToRemove
        {
            get => _doctorToRemove;
            set { _doctorToRemove = value; OnPropertyChanged(); }
        }

        public string DoctorUsername
        {
            get => _doctorUsername;
            set { _doctorUsername = value; OnPropertyChanged();  }
        }

        public ICommand AddDoctorFunc { get; }
        public ICommand RemoveDoctorFunc { get; }

                public void AddDoctor()
        {
            using (MySqlConnection conn = new MySqlConnection(SessionManager.connStrSQL))
            {
                conn.Open();
                MySqlCommand query = new MySqlCommand("SELECT COUNT(*) FROM doctors WHERE email = @Email;", conn);
                query.Parameters.AddWithValue("@Email", _doctorEmail);
                int doctorCount = Convert.ToInt32(query.ExecuteScalar());

                if (doctorCount > 0)
                {
                    MessageBox.Show("Lekarz o podanym adresie e-mail już istnieje");
                    return;
                }

                try
                {
                    MySqlCommand cmd = new MySqlCommand(@"
                        INSERT INTO doctors 
                            (first_name, last_name, email, phone, office_number, password, specialty, username) 
                        VALUES 
                            (@FirstName, @LastName, @Email, @Phone, @Office, @Password, @Specialty, @Username);", conn);

                    cmd.Parameters.AddWithValue("@FirstName", _doctorFirstName);
                    cmd.Parameters.AddWithValue("@LastName", _doctorLastName);
                    cmd.Parameters.AddWithValue("@Email", _doctorEmail);
                    cmd.Parameters.AddWithValue("@Phone", _doctorPhone);
                    cmd.Parameters.AddWithValue("@Office", _doctorOffice);
                    cmd.Parameters.AddWithValue("@Password", _doctorPassword);
                    cmd.Parameters.AddWithValue("@Specialty", _doctorSpecialty);
                    cmd.Parameters.AddWithValue("@Username", _doctorUsername);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Dodano nowego lekarza");
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Błąd podczas dodawania lekarza: " + ex.Message);
                }
            }
        }


        public void RemoveDoctor()
        {
            using (MySqlConnection conn = new MySqlConnection(SessionManager.connStrSQL))
            {
                conn.Open();
                MySqlCommand query = new MySqlCommand("SELECT COUNT(*) FROM doctors WHERE username = @Username;", conn);
                query.Parameters.AddWithValue("@Username", _doctorToRemove);
                int doctorCount = Convert.ToInt32(query.ExecuteScalar());

                if (doctorCount == 0)
                {
                    MessageBox.Show("Lekarz o podanym loginie nie istnieje");
                    return;
                }

                try
                {
                    MySqlCommand deleteCmd = new MySqlCommand("DELETE FROM doctors WHERE username = @Username LIMIT 1;", conn);
                    deleteCmd.Parameters.AddWithValue("@Username", _doctorToRemove);
                    deleteCmd.ExecuteNonQuery();

                    MessageBox.Show("Usunięto lekarza: " + _doctorToRemove);
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Błąd podczas usuwania lekarza: " + ex.Message);
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
