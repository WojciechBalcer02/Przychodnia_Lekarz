using FontAwesome.WPF;
using MySql.Data.MySqlClient;
using PolMedUMG.Model;
using PolMedUMG.View;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

namespace PolMedUMG.ViewModel
{
    public class Doctor
    {
        public string uid { get; set; }
        public string firstName { get; set; }
        public string secondName { get; set; }

        public Doctor(string uid, string firstName, string secondName)
        {
            this.uid = uid;
            this.firstName = firstName;
            this.secondName = secondName;
        }
    }

    public class SpecialistsViewModel
    {
        public ObservableCollection<Specialist> Specialists { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public FontAwesomeIcon Icon { get; set; }

        public SpecialistsViewModel()
        {
            Specialists = new ObservableCollection<Specialist>{};

            using (MySqlConnection conn = new MySqlConnection(SessionManager.connStrSQL))
            {
                try
                {
                    conn.Open();
                    string sql = "SELECT `uid`, `firstName`, `secondName` FROM `users` WHERE acc_type=1 LIMIT 12;";
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Doctor doctor = new Doctor(
                                reader["uid"].ToString(),
                                reader["firstName"].ToString(),
                                reader["secondName"].ToString()
                            );

                            Specialists.Add(new Specialist { Uid = doctor.uid, Icon = "UserMd", Title = "Doktor", Name = $"dr. {doctor.firstName} {doctor.secondName}" });
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Obsługa błędów
                    MessageBox.Show("Podczas pobierania danych wystapił błąd, spróbuj ponownie później lub skontaktuj się z administratorem", "Wystapił błąd");
                    Debug.WriteLine("Błąd podczas pobierania danych: " + ex.Message);
                }
            }
        }
    }
}
