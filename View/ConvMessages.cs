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
        public byte SenderAccType { get; set; }//Typ konta nadawcy
        public string Receiver { get; set; }//Odbiorca
        public byte ReceiverAccType { get; set; }//Typ konta odbciorcy
        public DateTime Date { get; set; }//Data wysłania
        public string Content { get; set; }//Zawartośc wiadomości
        public string DoctorImage { get; set; }//Obraz lekarza/ ogólnie obraz narazie i pewnie na zawsze nic nie bedzie robic
        public string StatusPatient { get; set;  }//Status odczytania wiadomości przez pacjenta
        public string StatusDoctor { get; set; }//Status odczytania wiadomości przez lekarza

        //Konstruktor tworzący obiek wiadomości
        public ConvMessages(string sender,string receiver, DateTime date, string content, string statusDoctor, string doctorImage, string statusPatient, byte sendertype, byte receivertype)
        {
            Sender = sender;
            Receiver = receiver;
            Date = date;
            Content = content;
            StatusDoctor = statusDoctor;
            DoctorImage = doctorImage;
            StatusPatient = statusPatient;
            SenderAccType = sendertype;
            ReceiverAccType = receivertype;
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
                                status, doctorImage, statusPatient,
                                sender_acctype,
                                receiver_acctype
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
                                reader["doctorImage"].ToString(),
                                reader["statusPatient"].ToString(),
                                Convert.ToByte(reader["sender_acctype"]),
                                Convert.ToByte(reader["receiver_acctype"])
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
        public List<ConvMessages> GetMessagesFrom(string doctorName, string patientName)
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

        //Metoda zwraca listę wszystkich unikalnych lekarzy z danej listy wiadomości
        public List<ConvMessages> ListOfUniqueDoctors(string user)
        {
            return GetMessagesFromDB()
                .Where(m => (m.ReceiverAccType == 1 && m.Sender == user) || (m.SenderAccType == 1 && m.Receiver==user))
                .GroupBy(m => m.SenderAccType == Convert.ToByte("1") ? m.Sender : m.Receiver)
                .Select(g => g.Last())
                .ToList();
        }
        //Metoda zwraca listę wszystkich unikalnych pacjentów z danej listy wiadomości
        public List<ConvMessages> ListOfUniquePatients(string user)
        {
            return GetMessagesFromDB()
                .Where(m => (m.ReceiverAccType == 0 && m.Sender == user) || (m.SenderAccType == 0 && m.Receiver == user))
                .GroupBy(m => m.SenderAccType == Convert.ToByte("0") ? m.Sender : m.Receiver)
                .Select(g => g.Last())
                .ToList();
        }

        //Funkcja oznaczająca wiadomość jako nie przeczytaną dla podanego użytkownika i przeczytaną dla zalogowanego użytkownika
        public void markAsReaded(string receiver)
        {
            string statusField = SessionManager.accType == 1
                ? "status" : "statusPatient";

            string query = $@"UPDATE Conversations 
                    SET {statusField} = 'Odczytane' 
                    WHERE ((sender = @counterpart AND receiver = @user)
                        OR (sender = @user AND receiver = @counterpart))
                    AND {statusField} = 'nowa wiadomość'";
            try
            {
                using (MySqlConnection conn = new MySqlConnection(SessionManager.connStrSQL))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.CommandText = query;
                        cmd.Parameters.AddWithValue("@counterpart", receiver);
                        cmd.Parameters.AddWithValue("@user", SessionManager.CurrentUsername);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Błąd podczas aktualizowania statusu odczytania wiadomości: " + ex.Message); }
        }
    }
}
