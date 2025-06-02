using System.Security.AccessControl;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MySql.Data.MySqlClient;
using PolMedUMG.ViewModel;

namespace PolMedUMG.View
{
    public partial class PatientAccountCreation : UserControl
    {
        public PatientAccountCreation()
        {
            InitializeComponent();
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
        private bool ShowMsg(string msg)
        {
            MessageBox.Show(msg);
            return false;
        }
        private void go_Next(object sender, RoutedEventArgs e)
        {
            string uid = Nickname.Text;
            string pwd = Password.Text;
            string acc_type = "0";
            string mail = Email.Text;
            string firstName = Name.Text;
            string secondName = Surname.Text;
            string last_login = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string address = Address.Text;
            string phone = Phone.Text;
            string pesel = Pesel.Text;


            if (!IsValidUsername(uid))
            {
                MessageBox.Show("Nazwa użytkownika już istnieje lub jest za długa (max 11 znaków)");
                return;
            }
            if (!IsValidEmail(mail))
            {
                MessageBox.Show("Niepoprawny lub zajęty e-mail.");
                return;
            }
            if (!IsValidName(firstName))
            {
                MessageBox.Show("Niepoprawne imię (max 25 znaków, tylko litery).");
                return;
            }
            if (!IsValidSurname(secondName))
            {
                MessageBox.Show("Niepoprawne nazwisko (max 25 znaków, tylko litery).");
                return;
            }
            if (!IsValidPassword(pwd))
            {
                MessageBox.Show("Hasło musi mieć od 1 do 15 znaków.");
                return;
            }
            if (!IsValidPhoneNumber(phone))
            {
                MessageBox.Show("Numer telefonu musi mieć format +48XXXXXXXXX.");
                return;
            }
            if (!IsValidAddress(address))
            {
                MessageBox.Show("Adres nie może być dłuższy niż 60 znaków");
                return;
            }
            if (!IsValidPesel(pesel))
            {
                MessageBox.Show("Istnieje już użytkownik o podanym peselu");
                return;
            }


            // Jeśli wszystko OK — tworzymy konto
            CreateUser(uid, pwd, acc_type, mail, last_login, firstName, secondName,pesel,address,phone);
        }
        private void CreateUser(string uid, string pwd, string acc_type, string mail, string last_login, string firstName, string secondName,string pesel,string address,string phoneNumber)
        {
            /* using (MySqlConnection conn = new MySqlConnection(SessionManager.connStrSQL))
             {
                 try
                 {
                     conn.Open();

                     string sql = @"INSERT INTO users (uid, pwdHash,pwdSalt, acc_type, mail, last_login, firstName, secondName)
                            VALUES (@uid, @pwdHash,@pwdSalt, @acc_type, @mail, @last_login, @firstName, @secondName);";

                     using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                     {
                         cmd.Parameters.AddWithValue("@uid", uid);
                         byte[] salt = HashFunction.GenerateSalt();
                         pwd = HashFunction.HashPassword(pwd, salt);
                         cmd.Parameters.AddWithValue("@pwdHash", pwd);
                         cmd.Parameters.AddWithValue("@pwdSalt", salt);
                         cmd.Parameters.AddWithValue("@acc_type", acc_type);
                         cmd.Parameters.AddWithValue("@mail", mail);
                         cmd.Parameters.AddWithValue("@last_login", last_login);
                         cmd.Parameters.AddWithValue("@firstName", firstName);
                         cmd.Parameters.AddWithValue("@secondName", secondName);

                         cmd.ExecuteNonQuery();
                     }

                     MessageBox.Show("Utworzono konto pacjenta!");
                     var Conv = new LoginPrompt();

                     var parentWindow = Window.GetWindow(this) as LoginScreen;
                     if (parentWindow != null)
                     {
                         parentWindow.LoadContent(Conv);
                     }
                 }
                 catch (Exception ex)
                 {
                     MessageBox.Show("Błąd przy dodawaniu konta: " + ex.Message);
                 }
             }*/


            using (MySqlConnection conn = new MySqlConnection(SessionManager.connStrSQL))
            {

                conn.Open();
                MySqlTransaction transaction = conn.BeginTransaction();
                try
                {


                    string usersql = @"INSERT INTO users (uid, pwdHash, pwdSalt, acc_type, mail, last_login, firstName, secondName) VALUES (@uid, @pwdHash,@pwdSalt, @acc_type, @mail, @last_login, @firstName, @secondName);";

                    using (MySqlCommand cmd = new MySqlCommand(usersql, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@uid", uid);
                        byte[] salt = HashFunction.GenerateSalt();
                        string hash = HashFunction.HashPassword(pwd, salt);
                        cmd.Parameters.AddWithValue("@pwdHash", hash);
                        cmd.Parameters.AddWithValue("@pwdSalt", salt);
                        cmd.Parameters.AddWithValue("@acc_type", acc_type);
                        cmd.Parameters.AddWithValue("@mail", mail);
                        cmd.Parameters.AddWithValue("@last_login", last_login);
                        cmd.Parameters.AddWithValue("@firstName", firstName);
                        cmd.Parameters.AddWithValue("@secondName", secondName);
                        cmd.ExecuteNonQuery();
                    }

                    string patientsql = @"UPDATE patients SET PESEL = @pesel, address = @address, phoneNumber = @phone WHERE uid = @uid";
                    using (MySqlCommand cmd = new MySqlCommand(patientsql, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@pesel", pesel);
                        cmd.Parameters.AddWithValue("@address",address );
                        cmd.Parameters.AddWithValue("@phone", phoneNumber);
                        cmd.Parameters.AddWithValue("@uid", uid);
                        cmd.ExecuteNonQuery();
                    }     
                    transaction.Commit();
                    conn.Close();
                    MessageBox.Show("Utworzono konto pacjenta!");
                    var Conv = new LoginPrompt();

                    var parentWindow = Window.GetWindow(this) as LoginScreen;
                    if (parentWindow != null)
                    {
                        parentWindow.LoadContent(Conv);
                    }


                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Błąd przy dodawaniu konta: " + ex.Message);
                }
            }
        }
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb && (tb.Text == "Imie" || tb.Text == "Nazwisko" || tb.Text == "E-mail" || tb.Text == "Hasło" || tb.Text == "Nazwa użytkownika" || tb.Text == "PESEL" || tb.Text == "Adres" || tb.Text == "Numer telefonu"))
            {
                tb.Text = string.Empty;
                tb.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb && string.IsNullOrWhiteSpace(tb.Text))
            {
                switch (tb.Name)
                {
                    case "Name":
                        tb.Text = "Imie";
                        break;
                    case "Surname":
                        tb.Text = "Nazwisko";
                        break;
                    case "Email":
                        tb.Text = "E-mail";
                        break;
                    case "Password":
                        tb.Text = "Hasło";
                        break;
                    case "Nickname":
                        tb.Text = "Nazwa użytkownika";
                        break;
                    case "Pesel":
                        tb.Text = "PESEL";
                        break;
                    case "Phone":
                        tb.Text = "Numer telefonu";
                        break;
                    case "Address":
                        tb.Text = "Adres";
                        break;
                }
                tb.Foreground = new SolidColorBrush(Colors.Gray);
            }
        }
        private bool IsValidEmail(string email)
        {
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (Regex.IsMatch(email, pattern) == false) return false;
            using (MySqlConnection conn = new MySqlConnection(SessionManager.connStrSQL))
            {
                conn.Open();
                string sql = "SELECT COUNT(*) FROM users WHERE mail = @mail";
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@mail", email);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count == 0;
                }
            }
        }
        private bool IsValidUsername(string uid)
        {
            if (string.IsNullOrWhiteSpace(uid) || uid.Length > 11)
                return false;
            using (MySqlConnection conn = new MySqlConnection(SessionManager.connStrSQL))
            {
                conn.Open();
                string sql = "SELECT COUNT(*) FROM users WHERE uid = @uid";
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@uid", uid);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count == 0;
                }
            }
        }
        private bool IsValidName(string name)
        {
            return Regex.IsMatch(name, @"^[A-Za-zżźćńółęąśŻŹĆĄŚĘŁÓŃ]{1,25}$");
        }
        private bool IsValidSurname(string surname)
        {
            return Regex.IsMatch(surname, @"^[A-Za-zżźćńółęąśŻŹĆĄŚĘŁÓŃ]{1,25}$");
        }
        private bool IsValidPassword(string password)
        {
            return password.Length > 0 && password.Length <= 15;
        }
        private bool IsValidPesel(string pesel)
        {           

            using (MySqlConnection conn = new MySqlConnection(SessionManager.connStrSQL))
            {
                conn.Open();
                string sql = "SELECT COUNT(*) FROM patients WHERE pesel = @pesel";
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@pesel", pesel);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    
                        return count == 0 && Regex.IsMatch(pesel, @"^[0-9]{11}$");    
                  
                }
            }
        }

        private bool IsValidPhoneNumber(string phone)
        {
            return Regex.IsMatch(phone, @"^[+]{1}[0-9]{10,15}$");
        }

        private bool IsValidAddress(string address)
        {
            return Regex.IsMatch(address, @"^[A-Za-zżźćńółęąśŻŹĆĄŚĘŁÓŃ]{5,60}$");
        }

    }
}
