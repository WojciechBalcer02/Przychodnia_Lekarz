using System.Windows;
using System.Windows.Controls;
using PolMedUMG.ViewModel;
using System.Net;
using System.Net.Mail;
using MySql.Data.MySqlClient;
using System.ComponentModel;

namespace PolMedUMG.View
{
    public partial class PasswordRecovery : UserControl
    {
        private string _email;

        public PasswordRecovery()
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
        private void goNext_Click(object sender, RoutedEventArgs e)
        {
            string enteredEmail = emailInput.Text;
            bool emailExist = EmailInDB(enteredEmail);

            if (enteredEmail != null && enteredEmail != "" && emailExist == true) // trzeba dodać regexową walidacje
            {
                SendNewPassword(enteredEmail);

                var Conv = new PasswordRecoverySuccess(enteredEmail);

                var parentWindow = Window.GetWindow(this) as LoginScreen;

                if (parentWindow != null)
                {
                    parentWindow.LoadContent(Conv);
                }
            }
            else{

                errorTextBlock.Text = "Podano zły adres e-mail";
            }
        }

        public static void SendNewPassword(string toEmail)
        {
            string generatedPassword = PasswordGenerator();

            var fromAddress = new MailAddress("PolMedUmg@gmail.com", "PolMedUMG");
            
            var toAddress = new MailAddress(toEmail);
            
            const string subject = "Reset hasła - PolMedUMG";

            string body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; color: #333;'>
            <h2 style='color: #2E86C1;'>Reset hasła – PolMedUMG</h2>
            <p>Wygenerowaliśmy dla Ciebie tymczasowe hasło, które umożliwi Ci zalogowanie się do aplikacji.</p>
            <p>Po pomyślnym logowaniu hasło to stanie się Twoim nowym hasłem. Pamiętaj, aby jak najszybciej zmienić je w ustawieniach konta.</p>
            <p><strong>Nowe hasło (ważne przez 15 minut):</strong></p>
            <div style='background-color: #f4f4f4; padding: 10px; border-radius: 5px; display: inline-block; font-size: 18px;'>{generatedPassword.Trim()}</div>
            <p style='margin-top: 20px;'>Jeśli nie prosiłeś o reset hasła, zignoruj tę wiadomość.</p>
            <br/>
            <p>Pozdrawiamy,<br/><strong>PolMed-UMG</strong></p>
            </body>
            </html>";

            var smtp = new SmtpClient
            {
                Host = "smtp-relay.brevo.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("8e4f8b001@smtp-brevo.com", "WUPfRyN3xHVmq6Zj")
            };

            using (var message = new MailMessage(fromAddress, toAddress){Subject = subject, Body = body, IsBodyHtml = true})
            {
                try
                {
                    smtp.Send(message);
                    SendInfoToDB(toEmail, generatedPassword, DateTime.Now);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        public static string PasswordGenerator()
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*";
            Random rnd = new Random();
            return new string(Enumerable.Repeat(valid, 15)
                .Select(s => s[rnd.Next(s.Length)]).ToArray());
        }
        public static void SendInfoToDB(string email, string newPass, DateTime dateOfGeneration)
        {
            using (MySqlConnection conn = new MySqlConnection(SessionManager.connStrSQL))
            {
                try
                {
                    conn.Open();

                    string getUsernameSql = "SELECT uid FROM users WHERE @mail = mail LIMIT 1;";
                    string username = null;

                    using (MySqlCommand getUserCmd = new MySqlCommand(getUsernameSql, conn))
                    {
                        getUserCmd.Parameters.AddWithValue("@mail", email);

                        var result = getUserCmd.ExecuteScalar();
                        if (result != null)
                        {
                            username = result.ToString();
                        }
                        else
                        {
                            return;
                        }
                    }

                    string insertSql = @"INSERT INTO PassRecovery (username, newPass, dateOfGeneration) 
                        VALUES (@username, @newPass, @dateOfGeneration);";

                    using (MySqlCommand cmd = new MySqlCommand(insertSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@newPass", newPass);
                        cmd.Parameters.AddWithValue("@dateOfGeneration", dateOfGeneration);

                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd podczas dodawania wiadomości: " + ex.Message);
                }
            }
        }
        private bool EmailInDB(string email)
        {
            using (MySqlConnection conn = new MySqlConnection(SessionManager.connStrSQL))
            {
                conn.Open();

                string sql = "SELECT COUNT(*) FROM users WHERE mail = @Email";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    var count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }

        }
    }
}
