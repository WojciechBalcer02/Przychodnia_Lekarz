using MySql.Data.MySqlClient;
using PolMedUMG.View;
using System.Windows;

namespace PolMedUMG.ViewModel
{
    public class PricingViewModel
    {
        public List<Model.Prices> prices { get; set; }

        public PricingViewModel()
        {
            prices = [new Model.Prices { Service_name = "Ładowanie...", Price = "..." }];
        }

        public void LoadData()
        {
            using (MySqlConnection conn = new MySqlConnection(SessionManager.connStrSQL))
            {
                try
                {
                    conn.Open();
                    string sql = "SELECT `name`, `price` FROM `Services`";
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            prices.Clear();
                            while (reader.Read())
                            {
                                prices.Add(new Model.Prices
                                {
                                    Service_name = reader["name"].ToString() ?? "",
                                    Price = reader["price"].ToString() ?? "",
                                });
                            }
                        }
                    }
                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd podczas pobierania danych: " + ex.Message);
                }
            }
        }

    }
}
