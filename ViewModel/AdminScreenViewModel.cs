using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using PolMedUMG.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Input;

namespace PolMedUMG.ViewModel
{
     public class AdminScreenViewModel : INotifyPropertyChanged
    {
        private string _usernameChange;
        private string PhoneNumberError;
        private string PasswordError;
        private string UsernameError;
        private string EmailError;
        private string RoomError;
        private string SpecError;
        private string SurnameError;
        private string NameError;
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
                    if (accoutn_type == "1") { MessageBox.Show("Podany użytkownik nie jest pacjentem"); }
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
                    if (!IsValidUsername(_doctorUsername))
                    {
                        MessageBox.Show(UsernameError);
                        return;
                    }
                    if (!IsValidPhoneNumber(_doctorPhone))
                    {
                        MessageBox.Show(PhoneNumberError);
                        return;
                    }
                    if (!IsValidPassword(_doctorPassword))
                    {
                        MessageBox.Show(PasswordError);
                        return;
                    }
                    if (!IsValidEmail(_doctorEmail))
                    {
                        MessageBox.Show(EmailError);
                        return;
                    }
                    if (!IsValidName(_doctorFirstName))
                    {
                        MessageBox.Show(NameError);
                        return;
                    }
                    if (!IsValidSurname(_doctorLastName))
                    {  
                        MessageBox.Show(SurnameError);
                        return;
                    }
                     if (!IsValidRoomNumber(_doctorOffice))
                     {
                        MessageBox.Show(RoomError);
                        return;
                     }
                     if (IsValidSpecialization(_doctorSpecialty))
                     {
                        MessageBox.Show(SpecError);
                        return;
            
                     }

            using (MySqlConnection conn = new MySqlConnection(SessionManager.connStrSQL))
            {
                conn.Open();
                MySqlCommand query = new MySqlCommand("SELECT COUNT(*) FROM users WHERE uid = @uid;", conn);
                query.Parameters.AddWithValue("@uid", _doctorUsername);
                int doctorCount = Convert.ToInt32(query.ExecuteScalar());

                if (doctorCount > 0)
                {
                    MessageBox.Show("Lekarz o podanej nazwie użytkownika już istnieje");
                    return;
                }

                try
                {
                    string cmd = @"INSERT INTO users (uid, acc_type, mail, firstName,secondName,last_login,pwdHash,pwdSalt) VALUES (@uid, @acc_type, @mail, @firstName,@secondName, @last_login,@pwdHash,@pwdSalt);";
                    using (MySqlCommand querry2 = new MySqlCommand(cmd, conn))
                    {
                        querry2.Parameters.AddWithValue("@firstName", _doctorFirstName);
                        querry2.Parameters.AddWithValue("@secondName", _doctorLastName);
                        querry2.Parameters.AddWithValue("@mail", _doctorEmail);
                        byte[] pwdSalt = HashFunction.GenerateSalt();
                        string pwdHash = HashFunction.HashPassword(DoctorPassword, pwdSalt);
                        querry2.Parameters.AddWithValue("@pwdHash", pwdHash);
                        querry2.Parameters.AddWithValue("@pwdSalt", pwdSalt);
                        querry2.Parameters.AddWithValue("@uid", _doctorUsername);
                        querry2.Parameters.AddWithValue("@acc_type", "1");
                        querry2.Parameters.AddWithValue("@last_login", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                        querry2.ExecuteNonQuery();
                    }
                    string doc = @"UPDATE `doctors` SET `roomNumber`= @room, `specialization`= @spec, `phoneNumber`= @phone WHERE(`doctors`.`uid` = @uid);";
                    using (MySqlCommand querry2 = new MySqlCommand(doc, conn))
                    {
                        querry2.Parameters.AddWithValue("@room", _doctorOffice);
                        querry2.Parameters.AddWithValue("@spec", _doctorSpecialty);
                        querry2.Parameters.AddWithValue("@phone", _doctorPhone);
                        querry2.Parameters.AddWithValue("@uid", _doctorUsername);

                        querry2.ExecuteNonQuery();

                        conn.Close();
                    }

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
            MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(SessionManager.connStrSQL);
            conn.Open();
            MySqlCommand query = new MySqlCommand();
            query.Connection = conn;
            query.CommandText = @"SELECT COUNT(*) FROM users WHERE uid = @uid;";
            query.Parameters.AddWithValue("@uid", _doctorToRemove);
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
                    acctype.Parameters.AddWithValue("@uid", _doctorToRemove);
                    string accoutn_type = acctype.ExecuteScalar().ToString();
                    conn.Close();

                    if (accoutn_type == "2") { MessageBox.Show("Nie można usunąć konta administratora"); }
                    if (accoutn_type == "0") { MessageBox.Show("Podany użytkownik nie jest lekarzem"); }
                    else
                    {
                        try
                        {
                            conn.Open();
                            MySqlCommand cmd = new MySqlCommand();
                            cmd.Connection = conn;
                            cmd.CommandText = "DELETE FROM users WHERE uid = @uid LIMIT 1;";
                            cmd.Parameters.AddWithValue("@uid", _doctorToRemove);
                            cmd.ExecuteNonQuery();
                            conn.Close();
                            MessageBox.Show("Usunięto lekarza: " + _doctorToRemove);
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
                    MessageBox.Show("Lekarz nie istnieje");
                }


            }
        }

            
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }





        private bool IsValidPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                PhoneNumberError = "Nie podano numeru telefonu";
                return false;
            }
            if (phone.Length < 11)
            {
                PhoneNumberError = "Zbyt krótki numer telefonu";
                return false;
            }
            if (!Regex.IsMatch(phone, @"^[+]{1}[0-9]{10,15}$"))
            {
                PhoneNumberError = "Podaj numer w formacie +48XXXXXXXXX";
                return false;
            }
            return true;
        }

        private bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                PasswordError = "Nie podano hasła";
                return false;
            }
            if (password.Length < 8)
            {
                PasswordError = "Zbyt krótkie hasło (min. 8)";
                return false;
            }
            if (password.Length > 25)
            {
                PasswordError = "Zbyt długie hasło (max 25)";
                return false;
            }
            string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$";
            if (!Regex.IsMatch(password, pattern))
            {
                PasswordError = "Hasło musi zawierać: małą i wielką literę, cyfrę oraz znak specjalny";
                return false;
            }
            return true;
        }

        private bool IsValidUsername(string uid)
        {
            if (string.IsNullOrWhiteSpace(uid))
            {
                UsernameError = "Nie podano nazwy użytkownika";
                return false;
            }
            if (uid.Length < 3)
            {
                UsernameError = "Nazwa użytkownika jest zbyt krótka (min. 3)";
                return false;
            }
            if (uid.Length > 25)
            {
                UsernameError = "Nazwa użytkownika jest zbyt długa (max 25)";
                return false;
            }
            if (!Regex.IsMatch(uid, @"^[a-zA-Z0-9_-]+$"))
            {
                UsernameError = "Dozwolone znaki: litery, cyfry, _ i -";
                return false;
            }

            using (MySqlConnection conn = new MySqlConnection(SessionManager.connStrSQL))
            {
                conn.Open();
                string sql = "SELECT COUNT(*) FROM users WHERE uid = @uid";
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@uid", _doctorUsername);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    if (count != 0)
                    {
                        UsernameError = "Taka nazwa użytkownika już istnieje";
                        return false;
                    }
                }
            }
            return true;
        }
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                EmailError = "Nie podano adresu e-mail";
                return false;
            }
            if (email.Length > 40)
            {
                EmailError = "Adres e-mail jest za długi (40)";
                return false;
            }
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(email, pattern))
            {

                EmailError = "Adres e-mail ma złą składnię";
                return false;

            }


            using (MySqlConnection conn = new MySqlConnection(SessionManager.connStrSQL))
            {
                conn.Open();
                string sql = "SELECT COUNT(*) FROM users WHERE mail = @mail";
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@mail", email);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    if (count != 0)
                    {
                        EmailError = "Użytkownik z takim adresem e-mail już istnieje";
                        return false;
                    }
                    ;
                }
                return true;
            }
        }
        private bool IsValidName(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || name == "Imie")
            {
                NameError = "Nie podano imienia";
                return false;
            }
            if (name.Length < 3)
            {
                NameError = "Zbyt krótkie imię (min. 3)";
                return false;
            }
            if (name.Length > 25)
            {
                NameError = "Zbyt długie imię (max 25)";
                return false;
            }
            if (!Regex.IsMatch(name, @"^[A-Za-zżźćńółęąśŻŹĆĄŚĘŁÓŃ]+$"))
            {
                NameError = "Dozwolone są tylko litery i polskie znaki";
                return false;
            }
            return true;
        }

        private bool IsValidSpecialization(string spec)
        {
            if (string.IsNullOrWhiteSpace(spec) || spec == "Imie")
            {
                SpecError = "Nie podano specjalizacji";
                return false;
            }
            if (spec.Length < 4)
            {
                SpecError = "Zbyt krótka specjalizacja (min. 4)";
                return false;
            }
            if (spec.Length > 50)
            {
                SpecError = "Zbyt długa specjalizacja (max 50)";
                return false;
            }
            if (!Regex.IsMatch(spec, @"^[A-Za-zżźćńółęąśŻŹĆĄŚĘŁÓŃ]+$"))
            {
                SpecError = "Dozwolone są tylko litery i polskie znaki";
                return false;
            }
            return true;
        }

        private bool IsValidSurname(string surname)
        {
            if (string.IsNullOrWhiteSpace(surname) || surname == "Nazwisko")
            {
                SurnameError = "Nie podano nazwiska";
                return false;
            }
            if (surname.Length < 3)
            {
                SurnameError = "Zbyt krótkie nazwisko (min. 3)";
                return false;
            }
            if (surname.Length > 25)
            {
                SurnameError = "Zbyt długie nazwisko (max 25)";
                return false;
            }
            if (!Regex.IsMatch(surname, @"^[A-Za-zżźćńółęąśŻŹĆĄŚĘŁÓŃ]+(-[A-Za-zżźćńółęąśŻŹĆĄŚĘŁÓŃ]+)?$"))
            {
                SurnameError = "Niepoprawny format nazwiska";
                return false;
            }
            return true;
        }

        private bool IsValidRoomNumber(string room)
        {
            if (string.IsNullOrWhiteSpace(room))
            {
                RoomError = "Nie podano numeru gabinetu";
                return false;
            }
            if (room.Length > 5)
            {
                RoomError = "Numer gabinet jest zbyt długi (max.5)";
                return false;
            }
            if (room.Length <1 )
            {
                RoomError = "Numer gabinet jest zbyt krótki (min.1)";
                return false;
            }
            if (!Regex.IsMatch(room, @"^[0-9]{1,5}$"))
            {
                RoomError = "Numer gabientu morze zawierać tylko cyfry";
                return false;
            }

            using (MySqlConnection conn = new MySqlConnection(SessionManager.connStrSQL))
            {
                conn.Open();
                string sql = "SELECT COUNT(*) FROM doctors WHERE roomNumber = @room";
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@room", room);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    if (count != 0)
                    {
                        RoomError = "Taka nazwa użytkownika już istnieje";
                        return false;
                    }
                }
            }
            return true;
        }





    }





}
