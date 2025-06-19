using MySql.Data.MySqlClient;
using PolMedUMG.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PolMedUMG.View
{
    /// <summary>
    /// Interaction logic for AdminScreen.xaml
    /// </summary>
    /// 


    public class Doctors
    {
        public string DoctorUid { get; set; }
        public string DoctorMail { get; set; }
        public string DoctorFirstName { get; set; }
        public string DoctorSecondName { get; set; }

    }

    public class Patients
    {
        public string PatientUid { get; set; }
        public string PatientMail { get; set; }
        public string PatientFirstName { get; set; }
        public string PatientSecondName { get; set; }
    }

    public partial class AdminScreen : Window
    {
        private List<Doctors> allDoctors;
        private int currentPageDoctors = 1;
        private int pageSizeDoctors = 5;
        private int totalPagesDoctors => (int)Math.Ceiling((double)allDoctors.Count / pageSizeDoctors);

        private List<Patients> allPatients;
        private int currentPagePatients = 1;
        private int pageSizePatients = 5;
        private int totalPagesPatients => (int)Math.Ceiling((double)allPatients.Count / pageSizePatients);
        public AdminScreen()
        {
            InitializeComponent();
            DataContext = new AdminScreenViewModel();
            allDoctors = new List<Doctors>();
            allPatients = new List<Patients>();


            using (MySqlConnection conn = new MySqlConnection(SessionManager.connStrSQL))
            {
                try
                {
                    conn.Open();

                    string sql = @"
                        SELECT uid, mail,firstName,secondName 
                        FROM users WHERE acc_type='1';";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                allDoctors.Add(new Doctors
                                {
                                    DoctorUid = reader["uid"].ToString(),
                                    DoctorMail = reader["mail"].ToString(),
                                    DoctorFirstName = reader["firstName"].ToString(),
                                    DoctorSecondName = reader["secondName"].ToString()
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd bazy danych: " + ex.Message);
                }
            }
            using (MySqlConnection conn = new MySqlConnection(SessionManager.connStrSQL))
            {
                try
                {
                    conn.Open();

                    string sql2 = @"
                        SELECT uid, mail,firstName,secondName 
                        FROM users WHERE acc_type='0';";

                    using (MySqlCommand cmd = new MySqlCommand(sql2, conn))
                    {

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                allPatients.Add(new Patients
                                {
                                    PatientUid = reader["uid"].ToString(),
                                    PatientMail = reader["mail"].ToString(),
                                    PatientFirstName = reader["firstName"].ToString(),
                                    PatientSecondName = reader["secondName"].ToString()
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd bazy danych: " + ex.Message);
                }
            }
            LoadCurrentPage();
            LoadCurrentPagePatients();
        }
        private void LoadCurrentPage()
        {
            var pageResults = allDoctors
            .Skip((currentPageDoctors - 1) * pageSizeDoctors)
            .Take(pageSizeDoctors)
            .ToList();

            ResultsListBox.ItemsSource = null;
            ResultsListBox.Items.Clear();
            ResultsListBox.ItemsSource = pageResults;

            PageCounterText.Text = $"{currentPageDoctors}/{totalPagesDoctors}";
        }


        private void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPageDoctors > 1)
            {
                currentPageDoctors--;
                LoadCurrentPage();
            }
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPageDoctors < totalPagesDoctors)
            {
                currentPageDoctors++;
                LoadCurrentPage();
            }
        }

        private void LoadCurrentPagePatients()
        {
            var pageResults = allPatients
            .Skip((currentPagePatients - 1) * pageSizePatients)
            .Take(pageSizePatients)
            .ToList();

            ListBoxPatients.ItemsSource = null;
            ListBoxPatients.Items.Clear();
            ListBoxPatients.ItemsSource = pageResults;

            PageCounterTextPatients.Text = $"{currentPagePatients}/{totalPagesPatients}";
        }

        private void PrevPage_ClickPatient(object sender, RoutedEventArgs e)
        {
            if (currentPagePatients > 1)
            {
                currentPagePatients--;
                LoadCurrentPagePatients();
            }
        }

        private void NextPage_ClickPatient(object sender, RoutedEventArgs e)
        {
            if (currentPagePatients < totalPagesPatients)
            {
                currentPagePatients++;
                LoadCurrentPagePatients();
            }
        }


        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e) //Minimalizuje ekran 
        {
            WindowState = WindowState.Minimized;
        }

        private void btnMinimize_Close(object sender, RoutedEventArgs e) //Wyłącza aplikację 
        {
            Application.Current.Shutdown();
        }

        private void btnMinimize_FullScreen(object sender, RoutedEventArgs e) //Fullscreanuje aplikację
        {
            if (WindowState != WindowState.Maximized) WindowState = WindowState.Maximized;
            else WindowState = WindowState.Normal;
        }

        private void ReloadPage(object sender, RoutedEventArgs e)
        {
            AdminScreen newWindow = new AdminScreen();
            Application.Current.MainWindow = newWindow;
            newWindow.Show();
            this.Close();
        }
    }



}