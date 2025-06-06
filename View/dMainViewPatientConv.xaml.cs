using MySql.Data.MySqlClient;
using PolMedUMG.Model;
using PolMedUMG.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
namespace PolMedUMG.View
{
    public partial class dMainViewPatientConv : Window
    {
        private Patient currentPatient;
        public List<ConvMessages> Messages { get; set; }
        public DateTime date { get; }
        public string patientName { get; }
        public ConvMessages conversation { get; set; }


        public dMainViewPatientConv(Patient patient)
        {
            InitializeComponent();
            this.currentPatient = patient;
            this.patientName=currentPatient.FirstName+" "+currentPatient.LastName;
            if (currentPatient == null) return;
            var repo = new MessageRepository();
            //Pobranie wiadomości dotyczących naszego lekarza
            this.Messages = repo.GetMessagesFrom(SessionManager.CurrentUsername, currentPatient.Uid);
            repo.markAsReaded(SessionManager.CurrentUsername);
            this.date = DateTime.Now;
            this.date = currentPatient.last_login;
            this.DataContext = this;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
            
        public string FormattedLoginDate
        {
            get
            {
                TimeSpan diff = DateTime.Now - date;
                if (diff.TotalMinutes < 60)
                {
                    int minutes = (int)diff.TotalMinutes;
                    return $"Ostatnia aktywność: {date:dd.MM.yyyy HH:mm} ({minutes} minut temu)";
                }
                else if (diff.TotalHours == 1)
                {
                    return $"Ostatnia aktywność: {date:dd.MM.yyyy HH:mm} (jedną godzinę temu)";
                }
                else if (diff.TotalHours < 24)
                {
                    int hours = (int)diff.TotalHours;
                    return $"Ostatnia aktywność: {date:dd.MM.yyyy HH:mm} ({hours} godzin temu)";
                }
                else if (diff.TotalDays < 2)
                {
                    return $"Ostatnia aktywność: {date:dd.MM.yyyy HH:mm} (jeden dzień temu)";
                }
                else
                {
                    int days = (int)diff.TotalDays;
                    return $"Ostatnia aktywność: {date:dd.MM.yyyy HH:mm} ({days} dni temu)";
                }
            }
        }

        //Obsługa przycisku wysłanie wiadomości do pacjenta
        private void Send_Click1(object sender, RoutedEventArgs e)
        {
            string messageText = MessageInput.Text;
            string senderr = SessionManager.CurrentUsername;
            string receiver = currentPatient.Uid;
            DateTime data = DateTime.Now;
            string dataAsString = data.ToString();

            if (!string.IsNullOrWhiteSpace(messageText))
            {
                var newMsg = new ConvMessages(senderr, receiver, DateTime.Now, messageText, "Odczytane", "dummy", "nowa wiadomość", 1, 0);

                Messages.Add(newMsg);

                MessagesList.ItemsSource = null;
                MessagesList.ItemsSource = Messages;

                //Wrzucenie wiadomości do bazy danych
                using (MySqlConnection conn = new MySqlConnection(SessionManager.connStrSQL))
                {
                    try
                    {
                        conn.Open();

                        string sql = @"INSERT INTO Conversations (sender, receiver, date, content, status, doctorImage, statusPatient, sender_acctype,receiver_acctype) 
                        VALUES (@sender, @receiver, @date, @content, @status, @doctorImage, @statusPatient,@sendertype, @receivertype);";


                        using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@sender", senderr);
                            cmd.Parameters.AddWithValue("@receiver", receiver);
                            cmd.Parameters.AddWithValue("@date", dataAsString);
                            cmd.Parameters.AddWithValue("@content", messageText+"4343");
                            cmd.Parameters.AddWithValue("@status", "Odczytane");
                            cmd.Parameters.AddWithValue("@doctorImage", "dummy");
                            cmd.Parameters.AddWithValue("@statusPatient", "nowa wiadomość");
                            cmd.Parameters.AddWithValue("@sendertype", 1);
                            cmd.Parameters.AddWithValue("@receivertype", 0);

                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Błąd podczas dodawania wiadomości: " + ex.Message);
                    }
                }
                MessageInput.Text = "";
            }
        }

        private void MessageInput_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Send_Click1(sender, e);
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}