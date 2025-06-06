using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using MySql.Data.MySqlClient;


namespace PolMedUMG.View
{
    public partial class MessagesOpenConv : UserControl
    {
        public List<ConvMessages> Messages { get; set; }
        public DateTime date { get;  }
        public string doctorName { get; }
        public string doctorImage { get; }
        public ConvMessages conversation { get; set; }
        public MessagesOpenConv(DateTime date, string doctorName, string doctorImage, ConvMessages conversation)
        {

            InitializeComponent();

            var repo = new MessageRepository();
            //Pobranie wiadomości dotyczących naszego pacjenta
            Messages = repo.GetMessagesFrom(doctorName,SessionManager.CurrentUsername);

            repo.markAsReaded(doctorName);

            this.date = date;
            this.doctorName = doctorName;
            this.doctorImage = doctorImage;
            this.conversation = conversation;

            this.DataContext = this;
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
        //Obsługa przycisku powrotu do głównego ekranu wiadomości  
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (MainArea != null)
            {
                MainArea.Children.Clear();
                MainArea.Children.Add(new Messages());
            }
        }
        //Obsługa przycisku wysłanie wiadomości do lekarza
        private void Send_Click(object sender, RoutedEventArgs e)
        {
            string messageText = MessageInput.Text;
            string senderr = SessionManager.CurrentUsername;
            string receiver = doctorName;
            DateTime data = DateTime.Now;
            string dataAsString = data.ToString();
            byte receivertype = 1;
            byte sendertype = 0;

            if (!string.IsNullOrWhiteSpace(messageText))
            {
                var newMsg = new ConvMessages(senderr, receiver, DateTime.Now, messageText,"nowa wiadomość", "dummy", "Odczytane",receivertype,sendertype);

                Messages.Add(newMsg);

                MessagesList.ItemsSource = null;
                MessagesList.ItemsSource = Messages;


                using (MySqlConnection conn = new MySqlConnection(SessionManager.connStrSQL))
                {
                    try
                    {
                        conn.Open();
                        //Wrzucenie wiadomości do bazy danych
                        string sql = @"INSERT INTO Conversations (sender, receiver, date, content, status, doctorImage, statusPatient, sender_acctype,receiver_acctype) 
                        VALUES (@sender, @receiver, @date, @content, @status, @doctorImage, @statusPatient,@sendertype, @receivertype);";

                        using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@sender", senderr);
                            cmd.Parameters.AddWithValue("@receiver", receiver);
                            cmd.Parameters.AddWithValue("@date", dataAsString);
                            cmd.Parameters.AddWithValue("@content", messageText);
                            cmd.Parameters.AddWithValue("@status", "nowa wiadomość");
                            cmd.Parameters.AddWithValue("@doctorImage", "dummy");
                            cmd.Parameters.AddWithValue("@statusPatient", "Odczytane");
                            cmd.Parameters.AddWithValue("@sendertype", sendertype);
                            cmd.Parameters.AddWithValue("@receivertype", receivertype);

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
            else
            {
                
            }
        }
        //Funkcja sprawdzająca użytkownika wiadomości i obecnego użytkownika, służąca do zmiany koloru
        public static bool compare(object value)
        {
            string sender = value as string;
            string user = SessionManager.CurrentUsername;


            bool areEqual = sender != null && user != null && sender == user;
            bool equalsMethod = sender != null && user != null && string.Equals(sender, user, StringComparison.Ordinal);

            return equalsMethod;
        }

        private void MessageInput_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Send_Click(sender,e);
            }
        }
    }

    //Konwerter do zmianay koloru wiadomości w zależności od tego kto ją wysłał
    public class BoolToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool IsSender = MessagesOpenConv.compare(value);

            return IsSender ? Brushes.LightGray : (Brush)new BrushConverter().ConvertFromString("#5C84E2");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    //Konwerter do zmiany strony wyświetlania wiadomości w zależności od tego kto ją wysłał
    public class BoolToAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool IsSender = MessagesOpenConv.compare(value);

            return IsSender ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    //Konwertery do zmiany koloru czcionka wiadomości w zależności od tego kto ją wysłał
    public class BoolToForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool IsSender = MessagesOpenConv.compare(value);

            return IsSender ? Brushes.Black : Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}


