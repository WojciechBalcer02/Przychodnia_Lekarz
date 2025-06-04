/*using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using FontAwesome.WPF;
using System.Windows.Data;
using MySql.Data.MySqlClient;

namespace PolMedUMG.View
{
    public partial class DoctorMessages : UserControl
    {
        public ObservableCollection<ConvMessages> Conversations { get; set; }
        public string date { get; }
        public string patientImage { get; }

        private List<ConvMessages> AllConversations;

        private int currentPage = 1;

        private int pageSize = 6;
        private int totalPages => (int)Math.Ceiling((double)AllConversations.Count / pageSize);

        public DoctorMessages()
        {
            InitializeComponent();
            var repo = new MessageRepository();
            // Use ListOfUniquePatients instead of ListOfUniqueDoctors for doctor view
            AllConversations = repo.ListOfUniquePatients(SessionManager.CurrentUsername).OrderByDescending(c => c.Date).ToList();
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

        public DateTime GetLastLogin(string patientName)
        {
            using (MySqlConnection conn = new MySqlConnection(SessionManager.connStrSQL))
            {
                try
                {
                    conn.Open();
                    

                    // Get last login date for the found username
                    string getLoginSql = "SELECT last_login FROM users WHERE uid = @username LIMIT 1";
                    using (MySqlCommand cmd = new MySqlCommand(getLoginSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", patientName);
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
                var Conv = new DoctorMessagesOpenConv(
                    GetLastLogin(selectedConversation.Receiver),
                    selectedConversation.Receiver,
                    "patient_default", // default patient image
                    selectedConversation
                );

                var parentWindow = Window.GetWindow(this) as DoctorScreen; // Assuming there's a DoctorScreen window
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

    // Status to icon converter for doctor view (checks StatusDoctor instead of StatusPatient)
    public class DoctorStatusToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value as string;
            if (status == null)
                return FontAwesomeIcon.CheckCircle; // default icon

            switch (status.ToLower())
            {
                case "nowa wiadomość":
                    return FontAwesomeIcon.Envelope; // icon for new message
                case "odczytane":
                    return FontAwesomeIcon.CheckCircle; // icon for read
                default:
                    return FontAwesomeIcon.CheckCircle;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}*/


using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using FontAwesome.WPF;
using System.Windows.Data;
using MySql.Data.MySqlClient;
using PolMedUMG.ViewModel;

namespace PolMedUMG.View
{
    public partial class DoctorMessages : UserControl
    {
        public ObservableCollection<ConvMessages> Conversations { get; set; }
        public string date { get; }
        public string doctorImage { get; }

        private List<ConvMessages> AllConversations;

        private int currentPage = 1;

        private int pageSize = 6;

        private int totalPages => (int)Math.Ceiling((double)AllConversations.Count / pageSize);

        
        public DoctorMessages()//Konwersacje lekarza z pacjentami, sortowane odwrotnie chronologicznie
        {
            InitializeComponent();

            var varii = new MessageRepository();
            AllConversations = varii.ListOfUniquePatients(SessionManager.CurrentUsername).OrderByDescending(c => c.Date).ToList();
            Conversations = new ObservableCollection<ConvMessages>();

            DataContext = this;

            LoadCurrentPage();

        }

        //Załadowanie obecnej strony z konwersacjami
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

        //Poprzednia strona konwersacji
        private void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                LoadCurrentPage();
            }
        }

        //Kolejna strona konwersacji
        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                LoadCurrentPage();
            }
        }

        //Pobranie czasu ostatniego zalogowania użytkownika z których pisze lekarz
        public DateTime GetLastLogin(string receiver)
        {
            using (MySqlConnection conn = new MySqlConnection(SessionManager.connStrSQL))
            {
                try
                {
                    conn.Open();
                    string getLoginSql = "SELECT last_login FROM users WHERE uid = @username LIMIT 1";
                    using (MySqlCommand cmd = new MySqlCommand(getLoginSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", receiver);
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

        //Otwarcie nowego okna konwersacji
        private void ConversationList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ConversationList.SelectedItem is ConvMessages selectedConversation)
            {
                if (selectedConversation.ReceiverAccType == Convert.ToByte("1"))
                {
                    var Conv = new DoctorMessagesOpenConv(
                        GetLastLogin(selectedConversation.Sender),
                        selectedConversation.Sender,
                        selectedConversation.DoctorImage,
                        selectedConversation
                    );


                    var parentWindow = Window.GetWindow(this) as DoctorScreen;

                    if (parentWindow != null)
                    {
                        parentWindow.LoadContent(Conv);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Nie udało się znaleźć nadrzędnego okna.");
                    }
                }
                else
                {
                    var Conv = new DoctorMessagesOpenConv(
                        GetLastLogin(selectedConversation.Receiver),
                        selectedConversation.Receiver,
                        selectedConversation.DoctorImage,
                        selectedConversation
                    );

                    var parentWindow = Window.GetWindow(this) as DoctorScreen;

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
}

