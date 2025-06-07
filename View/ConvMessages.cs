using System.Diagnostics;
using System.Windows;
using Google.Protobuf.WellKnownTypes;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Effects;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using static System.Net.Mime.MediaTypeNames;

namespace PolMedUMG.View
{   //Klasa wiadomości między lekarzem a pacjentem
    public class ConvMessages
    {
        public string Sender { get; set; } //Nadawca
        public string Receiver { get; set; }//Odbiorca
        public DateTime Date { get; set; }//Data wysłania
        public string Content { get; set; }//Zawartośc wiadomości
        public string DoctorImage { get; set; }//Obraz lekarza, ogólnie obraz narazie nic nie bedzie robi
        public string Status { get; set; }//Status odczytania wiadomości

        public string StatusSender { get; set; }

        //Konstruktor tworzący obiek wiadomości
        public ConvMessages(string sender,string receiver, DateTime date, string content, string status, string doctorImage)
        {
            Sender = sender;
            Receiver = receiver;
            Date = date;
            Content = content;
            Status = status;
            DoctorImage = doctorImage;
            StatusSender = "Odczytane";
            
        } 
        
    }

    //Klasa repozytorium wiadomości
    public class MessageRepository
    {
        //Metoda pobierające wszystkie wiadomości z bazy których zalogowany użytkownik jest nadawcą lub odbiorcą
        public List<ConvMessages> GetMessagesFromDB()
        {
            List<ConvMessages> conversations = new List<ConvMessages>();

            using (MySqlConnection conn = new MySqlConnection(SessionManager.connStrSQL))
            {
                try
                {
                    conn.Open();
                    string sql = @"SELECT sender, receiver, date, content, 
                                status, doctorImage
                                FROM Conversations WHERE sender = @sender OR receiver = @receiver;";
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("@sender", SessionManager.CurrentUsername);
                    cmd.Parameters.AddWithValue("@receiver", SessionManager.CurrentUsername);
                    cmd.Connection = conn;
                    MySqlDataReader reader = cmd.ExecuteReader();
                    {
                        while (reader.Read())
                        {
                            ConvMessages msg = new ConvMessages(
                                reader["sender"].ToString(),
                                reader["receiver"].ToString(),
                                Convert.ToDateTime(reader["date"]),
                                reader["content"].ToString(),
                                reader["status"].ToString(),
                                reader["doctorImage"].ToString()
                            );
                            conversations.Add(msg);
                        }
                    }
                    conn.Close();
                }
                
                catch (Exception ex)
                {
                    // Obsługa błędów przy pobieraniu danych z bazy
                    Console.WriteLine("Błąd podczas pobierania danych: " + ex.Message);
                }
            }

            return conversations;
        }

        //Metoda zwraca wszystkie wiadomości miedzy dwoma użytkownikami
        public List<ConvMessages> GetMessagesFrom(string sender, string receiver)
        {
            var ConvMessages = GetMessagesFromDB();
            var filteredMessages = new List<ConvMessages>();

            foreach (var msg in ConvMessages)
            {
                if ((msg.Sender == sender && msg.Receiver == receiver) ||
                    (msg.Sender == receiver && msg.Receiver == sender))
                {
                    filteredMessages.Add(msg);
                }
            }

            return filteredMessages;
        }

        public List<ConvMessages> ListOfUniqueRecepiants(string user)
        {
            var allMessages = GetMessagesFromDB()
                .Where(m => m.Sender.Equals(SessionManager.CurrentUsername, StringComparison.OrdinalIgnoreCase) ||
                            m.Receiver.Equals(SessionManager.CurrentUsername, StringComparison.OrdinalIgnoreCase))
                .ToList();

            var uniqueConversations = allMessages
                .Select(m => new
                {
                    Recipient = m.Sender.Equals(SessionManager.CurrentUsername, StringComparison.OrdinalIgnoreCase) ? m.Receiver : m.Sender,
                    Message = m
                })
                .GroupBy(x => x.Recipient, StringComparer.OrdinalIgnoreCase)
                .Select(g =>
                {
                    // Ostatnia wiadomość w konwersacji (do podglądu treści i daty)
                    var lastMsgOverall = g.OrderByDescending(x => x.Message.Date).First().Message;

                    // Ostatnia wiadomość OTRZYMANA przez użytkownika (do statusu)
                    var lastReceivedMsg = g
                        .Where(x => x.Message.Receiver.Equals(SessionManager.CurrentUsername, StringComparison.OrdinalIgnoreCase))
                        .OrderByDescending(x => x.Message.Date)
                        .FirstOrDefault()?.Message;

                    // Jeżeli nie ma żadnej otrzymanej wiadomości, to Odczytane
                    string statusToShow = lastReceivedMsg != null ? lastReceivedMsg.Status : "Odczytane";

                    return new ConvMessages(
                        sender: lastMsgOverall.Sender,
                        receiver: lastMsgOverall.Receiver,
                        date: lastMsgOverall.Date,
                        content: lastMsgOverall.Content,
                        status: statusToShow,
                        doctorImage: lastMsgOverall.DoctorImage
                    );
                })
                .OrderByDescending(m => m.Date)
                .ToList();

            return uniqueConversations;
        }



        //Funkcja oznaczająca wiadomość jako nie przeczytaną dla podanego użytkownika i przeczytaną dla zalogowanego użytkownika
        public void markAsReaded(string receiver)
        {
          
            string query = $@"UPDATE Conversations 
                    SET status = 'Odczytane' 
                    WHERE (receiver = @send);";
            try
            {
                using (MySqlConnection conn = new MySqlConnection(SessionManager.connStrSQL))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.CommandText = query;
                        cmd.Parameters.AddWithValue("@send", receiver);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Błąd podczas aktualizowania statusu odczytania wiadomości: " + ex.Message); }
        }
    }
}
