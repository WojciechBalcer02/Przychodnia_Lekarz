using System.Diagnostics;
using System.Windows;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace PolMedUMG.View
{
    public class ConvMessages
    {
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public DateTime Date { get; set; }
        public string Content { get; set; }
        public string DoctorImage { get; set; }
        public string StatusPatient { get; set;  }
        public string StatusDoctor { get; set; }

        public ConvMessages(string sender, string receiver, DateTime date, string content, string statusDoctor, string doctorImage, string statusPatient)
        {
            Sender = sender;
            Receiver = receiver;
            Date = date;
            Content = content;
            StatusDoctor = statusDoctor;
            DoctorImage = doctorImage;
            StatusPatient = statusPatient;
        } 
        
    }
    public class MessageRepository
    {
        public List<ConvMessages> GetMessagesFromDB()
        {
            List<ConvMessages> conversations = new List<ConvMessages>();

            using (MySqlConnection conn = new MySqlConnection(SessionManager.connStrSQL))
            {
                try
                {
                    conn.Open();
                    string sql = "SELECT uid, sender, receiver, date, content, status, doctorImage, statusPatient FROM Conversations;";
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ConvMessages msg = new ConvMessages(
                                reader["sender"].ToString(),
                                reader["receiver"].ToString(),
                                Convert.ToDateTime(reader["date"]),
                                reader["content"].ToString(),
                                reader["status"].ToString(),
                                reader["doctorImage"].ToString(),
                                reader["statusPatient"].ToString()
                            );
                            conversations.Add(msg);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Obsługa błędów
                    Console.WriteLine("Błąd podczas pobierania danych: " + ex.Message);
                }
            }

            return conversations;
        }
        public List<ConvMessages> GetMessages() // ta metoda ma wiadomości testowe niezwiązane z bazą danych
        {

            var ConvMessages = new List<ConvMessages>
            {
                new ConvMessages("dr. Witt", "patient", new DateTime(2004, 12, 12), "Witam serdecznie kochaniutki", "Odczytane" , "dummy", "Odczytane"),
                new ConvMessages("patient", "dr. Witt", DateTime.Now, "Cześć, jak się masz?", "Odczytane" , "dummy", "Odczytane"),
                new ConvMessages("dr. Witt", "Patient", DateTime.Now, "Nie twoj interes kasztanie", "Odczytane" , "dummy", "Odczytane"),
                new ConvMessages("dr. Marek Towarek", "patient", DateTime.Now, "tescik1", "Odczytane" , "dummy", "Odczytane"),
                new ConvMessages("patient", "dr. Tomasz Kowalski", DateTime.Now, "tescik2", "Odczytane" , "dummy", "Odczytane"),
                new ConvMessages("patient", "dr. Andrzej Pędrak", DateTime.Now, "tescik3333", "Odczytane" , "dummy", "Odczytane"),
                new ConvMessages("dr. Witt", "patient", DateTime.Now, "w sumie dziala", "Odczytane" , "dummy", "Odczytane"),
                new ConvMessages("patient", "dr. Witt", DateTime.Now, "a no", "Odczytane" , "dummy", "Odczytane"),
                new ConvMessages("patient", "dr. Eryk Fryderyk", DateTime.Now, "a no", "Odczytane" , "dummy", "Odczytane"),
                new ConvMessages("dr. Moczybroda", "patient", DateTime.Now, "siema byku jak tam kodowanie", "nowa wiadomość" , "dummy", "Odczytane")
            };
            return ConvMessages;
        }
        public List<ConvMessages> GetMessagesFrom(string doctorName, string patientName) // ta metoda zwraca wszystkie wiadomosci miedzy dwoma uzytkownikami
        {
            var ConvMessages = GetMessagesFromDB();
            var filteredMessages = new List<ConvMessages>();

            foreach (var msg in ConvMessages)
            {
                if ((msg.Sender == patientName && msg.Receiver == doctorName) ||
                    (msg.Sender == doctorName && msg.Receiver == patientName))
                {
                    filteredMessages.Add(msg);
                }
            }

            return filteredMessages;
        }
        public List<ConvMessages> ListOfUniqueDoctors(string user) //ta metoda mówi o unikalnych lekarzach z którymi miał kontakt dany pacjent, zapisuje ich w "Sender"
        {
            var allMessages = GetMessagesFromDB();

            var uniqueDoctors = allMessages
                .Where(m => m.Sender.Equals(user, StringComparison.OrdinalIgnoreCase) || m.Receiver.Equals(user, StringComparison.OrdinalIgnoreCase))
                .Select(m => new
                {
                    Doctor = m.Sender.Equals(user, StringComparison.OrdinalIgnoreCase) ? m.Receiver : m.Sender,
                    Message = m
                })
                .Where(x => x.Doctor.ToLower().StartsWith("dr."))
                .GroupBy(x => x.Doctor, StringComparer.OrdinalIgnoreCase)
                .Select(g =>
                {
                    var lastMsg = g.Last().Message;
                    return new ConvMessages(
                        sender: g.Key,               // ustawiamy Sender na nazwę doktora (g.Key)
                        receiver: lastMsg.Receiver,
                        date: lastMsg.Date,
                        content: lastMsg.Content,
                        statusDoctor: lastMsg.StatusDoctor,
                        statusPatient: lastMsg.StatusPatient,
                        doctorImage: lastMsg.DoctorImage
                    );
                })
                .ToList();

            return uniqueDoctors;
        }
        public void markAsReaded(string doctorName, string patientName) // markuje jako przeczytane wiadomości
        {
            string connectionString = SessionManager.connStrSQL;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"UPDATE Conversations SET statusPatient = @newStatus WHERE Sender = @doctorName AND Receiver = @patientName AND statusPatient = @currentStatus";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@newStatus", "Odczytane");
                    cmd.Parameters.AddWithValue("@currentStatus", "nowa wiadomość");
                    cmd.Parameters.AddWithValue("@doctorName", doctorName);
                    cmd.Parameters.AddWithValue("@patientName", patientName);

                    int affected = cmd.ExecuteNonQuery();
                    Debug.WriteLine($"[DEBUG] Zaktualizowano {affected} wiadomości.");
                }
            }
        }
    }
}
