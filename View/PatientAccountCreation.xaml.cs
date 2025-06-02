using System.Security.AccessControl;
using System.Text.RegularExpressions;
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

            // Jeśli wszystko OK — tworzymy konto
            CreateUser(uid, pwd, acc_type, mail, last_login, firstName, secondName);
        }
        private void CreateUser(string uid, string pwd, string acc_type, string mail, string last_login, string firstName, string secondName)
        {
            using (MySqlConnection conn = new MySqlConnection(SessionManager.connStrSQL))
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
            }
        }
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb && (tb.Text == "Imie" || tb.Text == "Nazwisko" || tb.Text == "E-mail" || tb.Text == "Hasło" || tb.Text == "Nazwa użytkownika"))
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
    }
}
