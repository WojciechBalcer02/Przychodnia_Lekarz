using MySql.Data.MySqlClient;
using PolMedUMG.Model;
using PolMedUMG.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PolMedUMG.View
{
    public class Price
    {
        public string ServicePrice { get; set; }
        public string ServiceName { get; set; }

    }
    public partial class dPricing : UserControl
    {
        private List<Price> allPrices;
        private int currentPage = 1;
        private int pageSize = 9;
        private int totalPages => (int)Math.Ceiling((double)allPrices.Count / pageSize);


        public dPricing()
        {
            InitializeComponent();

            allPrices = new List<Price>();

            using (MySqlConnection conn = new MySqlConnection(SessionManager.connStrSQL))
            {
                try
                {
                    conn.Open();

                    string sql = @"
                        SELECT name, price 
                        FROM Services ;";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                allPrices.Add(new Price
                                {
                                    ServicePrice = reader["price"].ToString(),
                                    ServiceName = reader["name"].ToString()
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
        }
                private void LoadCurrentPage()
                {
                var pageResults = allPrices
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToList();

                ResultsListBox.ItemsSource = null;
                ResultsListBox.Items.Clear();
                ResultsListBox.ItemsSource = pageResults;

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




    }     





}
 
    
