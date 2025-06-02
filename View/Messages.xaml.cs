using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using FontAwesome.WPF;
using System.Windows.Data;
using MySql.Data.MySqlClient;

namespace PolMedUMG.View
{
    public partial class Messages : UserControl
    {
        public ObservableCollection<ConvMessages> Conversations { get; set; }
        public string date { get; }
        public string doctorImage { get; }

        private List<ConvMessages> AllConversations;

        private int currentPage = 1;

        private int pageSize = 6;
        private int totalPages => (int)Math.Ceiling((double)AllConversations.Count / pageSize);

        public Messages()
        {
            InitializeComponent();

            var varii = new MessageRepository();
            AllConversations = varii.ListOfUniqueDoctors(SessionManager.CurrentUsername).OrderByDescending(c => c.Date).ToList();
            Conversations = new ObservableCollection<ConvMessages>();

            DataContext = this;

            LoadCurrentPage();

        }
        private void LoadCurrentPage()
        {
            var pageVisits = AllConversations
        .Skip((currentPage - 1) * pageSize)
        .Take(pageSize)
        .ToList();

            Conversations.Clear();
            foreach (var item in pageVisits)
                Conversations.Add(item);

            PageCounterText.Text = $"{currentPage}/{totalPages}";
        }

        private void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                LoadCurrentPage();
            }
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                LoadCurrentPage();
            }
        }
        public DateTime GetLastLogin(string receiver)
        {
            using (MySqlConnection conn = new MySqlConnection(SessionManager.connStrSQL))
            {
                try
                {
                    conn.Open();

                    // Krok 1: Pobierz username (login) na podstawie receivera
                    string getUsernameSql = @"SELECT uid FROM users WHERE CONCAT('dr. ', firstName, ' ', secondName) = @receiver LIMIT 1";

                    string username = null;

                    using (MySqlCommand cmd = new MySqlCommand(getUsernameSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@receiver", receiver);
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                            username = result.ToString();
                    }

                    // Jeśli nie znaleziono loginu – wyjście
                    if (string.IsNullOrEmpty(username))
                        return DateTime.Now;

                    // Krok 2: Pobierz datę ostatniego logowania dla znalezionego username
                    string getLoginSql = "SELECT last_login FROM users WHERE uid = @username LIMIT 1";

                    using (MySqlCommand cmd = new MySqlCommand(getLoginSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        object result = cmd.ExecuteScalar();

                        if (result != null && DateTime.TryParse(result.ToString(), out DateTime lastLogin))
                            return lastLogin;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Błąd przy pobieraniu daty logowania: " + ex.Message);
                }
            }

            return DateTime.Now;
        }
        private void ConversationList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ConversationList.SelectedItem is ConvMessages selectedConversation)
            {
                var Conv = new MessagesOpenConv(
                    GetLastLogin(selectedConversation.Sender),
                    selectedConversation.Sender,
                    selectedConversation.DoctorImage,
                    selectedConversation
                );
                var parentWindow = Window.GetWindow(this) as PatientScreen;

                if (parentWindow != null)
                {
                    parentWindow.LoadContent(Conv);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Nie udało się znaleźć nadrzędnego okna.");
                }
            }
        }
        public void LoadContent(UserControl control)
        {

            if (MainArea != null)
            {
                MainArea.Children.Clear();
                MainArea.Children.Add(control);
            }
        }
    }
    public class StatusToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value as string;
            if (status == null)
                return FontAwesomeIcon.CheckCircle; // domyślna ikona

            switch (status.ToLower())
            {
                case "nowa wiadomość":
                    return FontAwesomeIcon.Envelope; // ikona dla nowej wiadomości
                case "Odczytane":
                    return FontAwesomeIcon.CheckCircle; // ikona dla odczytane
                default:
                    return FontAwesomeIcon.CheckCircle;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

