using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using FontAwesome.WPF;
using ModelVisit = PolMedUMG.Model.Visit;

namespace PolMedUMG.View
{
    public partial class dCalendar : UserControl
    {
        private enum CalendarViewMode
        {
            SingleMonth,
            ThreeMonths,
            FullYear
        }

        private CalendarViewMode currentViewMode = CalendarViewMode.SingleMonth;
        private int Year;
        public ObservableCollection<DateTime> Months { get; set; }
        public ObservableCollection<ModelVisit> PlannedVisits { get; set; }

        public int year
        {
            get => Year;
            set
            {
                Year = value;
                rok.Text = Year.ToString();
                UpdateCalendarView();
            }
        }

        public dCalendar()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            PlannedVisits = new ObservableCollection<ModelVisit>();
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
                                PlannedVisits.Add(new ModelVisit
                                {
                                    CauseOfVisit = reader["causeOfVisit"].ToString() ?? "",
                                    AdditionalInfo = Convert.ToString(reader["additionalInfo"]) ?? "",
                                    PhoneNumber = Convert.ToString(reader["phoneNumber"]) ?? "",
                                    DateOfVisit = Convert.ToDateTime(reader["dateOfVisit"]),
                                    ServiceName = Convert.ToString(reader["serviceName"]) ?? ""
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd podczas pobierania danych: " + ex.Message);
                }
            }
        }

        private void ToggleCalendarView(object sender, RoutedEventArgs e)
        {
            currentViewMode = currentViewMode switch
            {
                CalendarViewMode.SingleMonth => CalendarViewMode.ThreeMonths,
                CalendarViewMode.ThreeMonths => CalendarViewMode.FullYear,
                CalendarViewMode.FullYear => CalendarViewMode.SingleMonth,
                _ => CalendarViewMode.SingleMonth
            };

            UpdateCalendarView();
        }

        private void UpdateCalendarView()
        {
            Months = new ObservableCollection<DateTime>();         
            var panelTemplate = new ItemsPanelTemplate();
            var factory = new FrameworkElementFactory(typeof(UniformGrid));

            switch (currentViewMode)
            {
                case CalendarViewMode.SingleMonth:
                    Months.Add(new DateTime(year, DateTime.Now.Month, 1));
                    factory.SetValue(UniformGrid.RowsProperty, 1);
                    factory.SetValue(UniformGrid.ColumnsProperty, 1);
                    viewIcon.Icon = FontAwesomeIcon.Expand;
                    break;

                case CalendarViewMode.ThreeMonths:
                    int currentMonth = DateTime.Now.Month;
                    for (int i = 0; i < 3; i++)
                    {
                        int month = currentMonth + i;
                        int year = this.year;
                        if (month > 12)
                        {
                            month -= 12;
                            year += 1;
                        }
                        Months.Add(new DateTime(year, month, 1));
                    }
                    factory.SetValue(UniformGrid.RowsProperty, 1);
                    factory.SetValue(UniformGrid.ColumnsProperty, 3);
                    viewIcon.Icon = FontAwesomeIcon.Expand;
                    break;

                case CalendarViewMode.FullYear:
                    for (int m = 1; m <= 12; m++)
                    {
                        Months.Add(new DateTime(year, m, 1));
                    }
                    factory.SetValue(UniformGrid.RowsProperty, 3);
                    factory.SetValue(UniformGrid.ColumnsProperty, 4);
                    viewIcon.Icon = FontAwesomeIcon.Compress;
                    break;
            }

            panelTemplate.VisualTree = factory;
            monthGrid.ItemsPanel = panelTemplate;
            monthGrid.ItemsSource = Months;
        }

        private async void Calendar_Loaded(object sender, RoutedEventArgs e)
        {
            var calendar = (System.Windows.Controls.Calendar)sender;
            calendar.Visibility = Visibility.Visible;

            DateTime? begin = calendar.DisplayDateStart;
            if (begin.HasValue)
            {
                calendar.DisplayDateEnd = begin.Value.AddDays(DateTime.DaysInMonth(begin.Value.Year, begin.Value.Month) - 1);
            }

           

            var scaleTransform = calendar.RenderTransform as ScaleTransform;
            if (scaleTransform == null || scaleTransform.IsFrozen)
            {
                scaleTransform = new ScaleTransform();
                calendar.RenderTransform = scaleTransform;
            }

            switch (currentViewMode)
            {
                case CalendarViewMode.SingleMonth:
                    calendar.RenderTransformOrigin = new Point(0.5, -0.2);
                    scaleTransform.ScaleX = 2.2;
                    scaleTransform.ScaleY = 2.2;
                    break;
                case CalendarViewMode.ThreeMonths:
                    calendar.RenderTransformOrigin = new Point(0.5, -0.5);
                    scaleTransform.ScaleX = 1.5;
                    scaleTransform.ScaleY = 1.5;
                    break;
                case CalendarViewMode.FullYear:
                    scaleTransform.ScaleX = 1.0;
                    scaleTransform.ScaleY = 1.0;
                    break;
            }

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

        public void CalendarPrevious(object sender, RoutedEventArgs e)
        {
            year--;
        }

        public void CalendarNext(object sender, RoutedEventArgs e)
        {
            year++;
        }

        private void Calendar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cal = (System.Windows.Controls.Calendar)sender;
            foreach (var d in PlannedVisits)
            {
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