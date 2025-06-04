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

        public Messages()//Konwersacje pacjenta z lekarzami, sortowane odwrotnie chronologicznie
        {
            
            InitializeComponent();

            var varii = new MessageRepository();
            AllConversations = varii.ListOfUniqueDoctors(SessionManager.CurrentUsername).OrderByDescending(c => c.Date).ToList();
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

        //Pobranie czasu ostatniego zalogowania użytkownika z których pisze pacjent
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
                    var Conv = new MessagesOpenConv(
                        GetLastLogin(selectedConversation.Receiver),
                        selectedConversation.Receiver,
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
                else 
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

    //Konwerter zmieniające ikone odczytania wiadomości
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

    //Konwerter zmieniające nazwy osób z którymi piszemi ukazujące się w liści konwersacji
    public class CounterpartNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ConvMessages message)
            {
                bool isDoctor = SessionManager.accType == 1;

                // For doctor view: show patient name
                if (isDoctor)
                    return message.SenderAccType == 0 ? message.Sender : message.Receiver;

                // For patient view: show doctor name
                return message.SenderAccType == 1 ? message.Sender : message.Receiver;
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }



}

