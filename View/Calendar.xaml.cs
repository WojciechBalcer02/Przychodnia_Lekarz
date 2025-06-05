using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
namespace PolMedUMG.View
{
    /// <summary>
   /// Logika interakcji dla klasy Calendar.xaml
   /// </summary>
    public partial class Calendar : UserControl
    {
        private int Year;
        public ObservableCollection<DateTime> Months { get; set; }
        public ObservableCollection<Model.Visit> PlannedVisits { get; set; }

        public int year
        {
            get
            {
                return Year;
            }

            set
            {
                Year = value;
                rok.Text = Year.ToString();
                _ = Calendar_setMonths(Year);
            }
        }
        public Calendar()
        {
            PlannedVisits = new ObservableCollection<Model.Visit>();
            InitializeComponent();
            year = DateTime.Now.Year;
            getVisits();


        }
        public void getVisits()
        {
            using (MySqlConnection conn = new MySqlConnection(SessionManager.connStrSQL))
            {
                try
                {
                    conn.Open();
                    string sql = "SELECT `causeOfVisit`, `additionalInfo`, `phoneNumber`, `dateOfVisit`, `serviceName` FROM `Visits` WHERE `patient_id` = @patientId";
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@patientId", SessionManager.CurrentUsername);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                PlannedVisits.Add(new Model.Visit
                                {
                                    causeOfVisit = reader["causeOfVisit"].ToString() ?? "",
                                    additionalInfo = Convert.ToString(reader["additionalInfo"]) ?? "",
                                    PhoneNumber = Convert.ToString(reader["phoneNumber"]) ?? "",
                                    DateOfVisit = Convert.ToDateTime(reader["dateOfVisit"]),
                                    serviceName = Convert.ToString(reader["serviceName"]) ?? ""
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Obsługa błędów
                    MessageBox.Show("Błąd podczas pobierania danych: " + ex.Message);
                }
            }
        }
        // asynchroniczne wczytanie kalendarzy do grida
        private async Task Calendar_setMonths(int year)
        {
            Months = new ObservableCollection<DateTime>();
            monthGrid.ItemsSource = Months;

            for (int m = 1; m <= 12; m++)
            {
                int month = m;
                _ = Task.Run(() =>
               {
                   Application.Current.Dispatcher.Invoke(() =>
                   {
                       Months.Add(new DateTime(year, month, 1));
                   });
               });
            }
        }
        private async void Calendar_Loaded(object sender, RoutedEventArgs e)
        {
            // pozbycie się dni spoza danego miesiąca
            var calendar = (System.Windows.Controls.Calendar)sender;
            calendar.Visibility = Visibility.Visible;

            DateTime? begin = calendar.DisplayDateStart;
            if (begin.HasValue)
            {
                calendar.DisplayDateEnd = begin.Value.AddDays(DateTime.DaysInMonth(begin.Value.Year, begin.Value.Month) - 1);
            }
            // zaznaczenie dni wizyt w kalendarzu
            if (PlannedVisits != null)
            {
                var visits = await Task.Run(() =>
                    PlannedVisits.Select(v => v.DateOfVisit).Distinct().ToList()
                );
                foreach (var d in visits)
                {
                    if (calendar.DisplayDate.Month == d.Month)
                    {
                        foreach (var btn in FindVisualChildren<CalendarDayButton>(calendar))
                        {
                            if (btn.IsInactive == false && btn.DataContext is DateTime c && d.Date == c)
                            {
                                btn.Background = new RadialGradientBrush(
                                    Colors.Yellow,
                                    Color.FromArgb(0, 230, 230, 0))
                                {
                                    GradientOrigin = new Point(0.5, 0.5),
                                    Center = new Point(0.5, 0.5),
                                    RadiusX = 0.5,
                                    RadiusY = 0.5
                                };
                            }
                        }
                    }
                }
            }
        }
        static IEnumerable<T> FindVisualChildren<T>(DependencyObject o) where T : DependencyObject
        {
            if (o == null) yield break;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++)
            {
                var c = VisualTreeHelper.GetChild(o, i);
                if (c is T t) yield return t;
                foreach (var i2 in FindVisualChildren<T>(c)) yield return i2;
            }
        }
        //funkcje od przełączania roku
        public void CalendarPrevious(object sender, RoutedEventArgs e)
        {
            year--;
        }
        public void CalendarNext(object sender, RoutedEventArgs e) {
            year++;
        }

        private void Calendar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cal = (System.Windows.Controls.Calendar)sender;
            foreach (var d in PlannedVisits)
            {
                var calendar = (System.Windows.Controls.Calendar)sender;
                if (d.DateOfVisit.Date == cal.SelectedDate)
                {

                    var detailsWindow = new CalendarVisitDetails(d);

                    detailsWindow.ShowDialog();
                }
            }
            cal.SelectedDates.Clear();
        }
    }
}