/*using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Media3D;
using MySql.Data.MySqlClient;
using PolMedUMG.View;
using static System.Net.Mime.MediaTypeNames;

namespace PolMedUMG.View
{
    public partial class DoctorMessagesOpenConv : UserControl
    {
        public List<ConvMessages> Messages { get; set; }
        public DateTime date { get; }
        public string patientName { get; }
        public string patientImage { get; }
        public ConvMessages conversation { get; set; }

        public DoctorMessagesOpenConv(DateTime date, string patientName, string patientImage, ConvMessages conversation)
        {
            InitializeComponent();
            var repo = new MessageRepository();
            Messages = repo.GetMessagesFrom(SessionManager.CurrentUsername, patientName);
            
            // Mark messages as read from doctor's perspective
            repo.markAsReaded(patientName);
            
            this.date = date;
            this.patientName = patientName;
            this.patientImage = patientImage;
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

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (MainArea != null)
            {
                MainArea.Children.Clear();
                MainArea.Children.Add(new DoctorMessages());
            }
        }

        private void Send_Click(object s, RoutedEventArgs e)
        {
            string messageText = MessageInput.Text;
            string senderr= SessionManager.CurrentUsername;
            string receiver = patientName;
            DateTime data = DateTime.Now;
            byte sendertype = 1;
            byte receivertype = 0;
            string dataAsString = data.ToString();
            if (!string.IsNullOrWhiteSpace(messageText))
            {
                
                // For doctor sending message: statusDoctor = "Odczytane", statusPatient = "nowa wiadomość"
                var newMsg = new ConvMessages(senderr, receiver, DateTime.Now, messageText, "Odczytane", "dummy", "nowa wiadomość",sendertype,receivertype);
                Messages.Add(newMsg);
                MessagesList.ItemsSource = null;
                MessagesList.ItemsSource = Messages;

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
                            cmd.Parameters.AddWithValue("@content", messageText);
                            cmd.Parameters.AddWithValue("@status", "Odczytane");
                            cmd.Parameters.AddWithValue("@doctorImage", "dummy");
                            cmd.Parameters.AddWithValue("@statusPatient", "nowa wiadomość");
                            cmd.Parameters.AddWithValue("@sendertype", sendertype);
                            cmd.Parameters.AddWithValue("@receivertype", receivertype);

                            cmd.ExecuteNonQuery();
                        }
                        conn.Close();
                    }
                    
                    catch (Exception ex)
                    {
                        MessageBox.Show("Błąd podczas dodawania wiadomości: " + ex.Message);
                    }
                }

                MessageInput.Text = "";
            }
        }

        // Helper method to determine if message is from current doctor
        public static bool compareDoctor(object value)
        {
            string sender = value as string;
            string user = SessionManager.CurrentUsername;
            bool areEqual = sender != null && user != null && sender == user;
            bool equalsMethod = sender != null && user != null && string.Equals(sender, user, StringComparison.Ordinal);
            return equalsMethod;
        }
    }

    // Converter for doctor's message background
    public class DoctorBoolToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool IsDoctor = DoctorMessagesOpenConv.compareDoctor(value);
            return IsDoctor ? (Brush)new BrushConverter().ConvertFromString("#5C84E2") : Brushes.LightGray;
           
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Converter for doctor's message alignment
    public class DoctorBoolToAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool IsDoctor = DoctorMessagesOpenConv.compareDoctor(value);
            return IsDoctor ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Converter for doctor's message foreground
    public class DoctorBoolToForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool IsDoctor = DoctorMessagesOpenConv.compareDoctor(value);
            return IsDoctor ? Brushes.White : Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}*/



/*
< UserControl x: Class = "PolMedUMG.View.DoctorMessagesOpenConv"
             xmlns = "http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns: x = "http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns: mc = "http://schemas.openxmlformats.org/markup-compatibility/2006"
             xmlns: d = "http://schemas.microsoft.com/expression/blend/2008"
             xmlns: local = "clr-namespace:PolMedUMG.View"
             mc: Ignorable = "d"
             d: DesignHeight = "450" d: DesignWidth = "800" >


    < UserControl.Resources >
        < local:BoolToBackgroundConverter x:Key = "BoolToBackgroundConverter" />
        < local:BoolToAlignmentConverter x:Key = "BoolToAlignmentConverter" />
        < local:BoolToForegroundConverter x:Key = "BoolToForegroundConverter" />
    </ UserControl.Resources >

    < Grid x: Name = "MainArea" >
        < Grid.RowDefinitions >
            < RowDefinition Height = "Auto" />
            < RowDefinition Height = "*" />
            < RowDefinition Height = "Auto" />
        </ Grid.RowDefinitions >


        < !--Header with patient info and back button -->
        <Border Grid.Row="0" Background="#3498DB" Padding="15">
            <Grid>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="Auto"/>
                    <ColumnDefinition Width="Auto"/>
                    <ColumnDefinition Width="*"/>
                    <ColumnDefinition Width="Auto"/>
                </Grid.ColumnDefinitions>
                
                <!-- Back Button -->
                <Button Grid.Column="0" Content="◀ Powrót" Click="Back_Click" 
                        Background="Transparent" Foreground="White" BorderBrush="White"
                        Padding="10,5" Margin="0,0,15,0"/>
                
                <!-- Patient Avatar -->
                <Ellipse Grid.Column="1" Width="50" Height="50" Fill="White" Margin="0,0,15,0"/>
                <TextBlock Grid.Column="1" Text="P" FontSize="24" FontWeight="Bold" 
                         Foreground="#3498DB" HorizontalAlignment="Center" VerticalAlignment="Center"/>
                
                <!-- Patient Info -->
                <StackPanel Grid.Column="2" VerticalAlignment="Center">
                    <TextBlock Text="{Binding patientName}" FontSize="18" FontWeight="Bold" 
                             Foreground="White"/>
                    <TextBlock Text="{Binding FormattedLoginDate}" FontSize="12" 
                             Foreground="#ECF0F1"/>
                </StackPanel>
            </Grid>
        </Border>
        
        <!-- Messages List -->
        <ScrollViewer Grid.Row="1" VerticalScrollBarVisibility="Auto" Padding="10">
            <ItemsControl x:Name = "MessagesList" ItemsSource = "{Binding Messages}" >
                < ItemsControl.ItemTemplate >
                    < DataTemplate >
                        < Border Background = "{Binding Sender, Converter={StaticResource BoolToBackgroundConverter}}"
                                CornerRadius = "10" Padding = "10" Margin = "5,2"
                                HorizontalAlignment = "{Binding Sender, Converter={StaticResource BoolToAlignmentConverter}}"
                                MaxWidth = "300" >
                            < Border.Effect >
                                < DropShadowEffect Color = "Gray" BlurRadius = "3" ShadowDepth = "1" Opacity = "0.3" />
                            </ Border.Effect >
                            < StackPanel >
                                < TextBlock Text = "{Binding Content}"
                                         Foreground = "{Binding Sender, Converter={StaticResource BoolToForegroundConverter}}"
                                         TextWrapping = "Wrap" FontSize = "14" />
                                < TextBlock Text = "{Binding Date, StringFormat='{}{0:dd.MM.yyyy HH:mm}'}"
                                         Foreground = "{Binding Sender, Converter={StaticResource BoolToForegroundConverter}}"
                                         FontSize = "10" HorizontalAlignment = "Right" Margin = "0,5,0,0" Opacity = "0.7" />
                            </ StackPanel >
                        </ Border >
                    </ DataTemplate >
                </ ItemsControl.ItemTemplate >
            </ ItemsControl >
        </ ScrollViewer >


        < !--Message Input-- >
        < Border Grid.Row = "2" Background = "#F8F9FA" BorderBrush = "#E0E0E0" BorderThickness = "0,1,0,0" Padding = "15" >
            < Grid >
                < Grid.ColumnDefinitions >
                    < ColumnDefinition Width = "*" />
                    < ColumnDefinition Width = "Auto" />
                </ Grid.ColumnDefinitions >


                < TextBox x: Name = "MessageInput" Grid.Column = "0"
                         Background = "White" BorderBrush = "#BDC3C7" BorderThickness = "1"
                         Padding = "10" FontSize = "14" VerticalAlignment = "Center"
                         AcceptsReturn = "True" TextWrapping = "Wrap" MaxHeight = "80"
                         />


                < Button x: Name = "SendButton" Grid.Column = "1" Content = "Wyślij" Click = "Send_Click"
                        Background = "#3498DB" Foreground = "White" BorderBrush = "Transparent"
                        Padding = "15,10" Margin = "10,0,0,0" FontWeight = "Bold" />
            </ Grid >
        </ Border >
    </ Grid >
</ UserControl >*/



using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using MySql.Data.MySqlClient;


namespace PolMedUMG.View
{
    public partial class DoctorMessagesOpenConv : UserControl
    {
        public List<ConvMessages> Messages { get; set; }
        public DateTime date { get; }
        public string patientName { get; }
        public string doctorImage { get; }
        public ConvMessages conversation { get; set; }
        public DoctorMessagesOpenConv(DateTime date, string patientName, string doctorImage, ConvMessages conversation)
        {

            InitializeComponent();

            var repo = new MessageRepository();
            //Pobranie wiadomości dotyczących naszego lekarza
            Messages = repo.GetMessagesFrom(SessionManager.CurrentUsername, patientName);

            repo.markAsReaded(patientName);

            this.date = date;
            this.patientName = patientName;
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
                MainArea.Children.Add(new DoctorMessages());
            }
        }

        //Obsługa przycisku wysłanie wiadomości do pacjenta
        private void Send_Click(object sender, RoutedEventArgs e)
        {
            string messageText = MessageInput.Text;
            string senderr = SessionManager.CurrentUsername;
            string receiver = patientName;
            DateTime data = DateTime.Now;
            string dataAsString = data.ToString();
            byte receivertype = 1;
            byte sendertype = 0;

            if (!string.IsNullOrWhiteSpace(messageText))
            {
                var newMsg = new ConvMessages(senderr, receiver, DateTime.Now, messageText, "nowa wiadomość", "dummy", "Odczytane", receivertype, sendertype);

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

        private void MessageInput_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Send_Click(sender, e);
            }
        }


    }
   
}
