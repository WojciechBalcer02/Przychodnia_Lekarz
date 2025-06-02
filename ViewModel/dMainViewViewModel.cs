using MySql.Data.MySqlClient;
using PolMedUMG.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Data;

namespace PolMedUMG.ViewModel
{
    public class dMainViewViewModel : INotifyPropertyChanged
    {
        private string _doctorusername;
        private string _patientPesel;

        public string DoctorUsername
        {
            get => _doctorusername;
            set { _doctorusername = value; OnPropertyChanged(); }
        }

        public string PatientPesel
        {
            get => _patientPesel;
            set { _patientPesel = value; OnPropertyChanged(); }
        }

        public ICommand DoctorLookup { get; }

        public ICommand PatientLookup { get; }

        public dMainViewViewModel()
        {
            DoctorLookup = new RelayCommand(Check_Doctor);
            PatientLookup = new RelayCommand(Check_Patient);
        }

        private void Check_Doctor()
        {
            try
            {
                MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(SessionManager.connStrSQL);
                conn.Open();

                // Zapytanie do bazy o wybranego u�ytkownika
                MySqlCommand query = new MySqlCommand();
                query.Connection = conn;
                query.CommandText = @"SELECT COUNT(*) FROM users WHERE uid = @uid AND acc_type='1';";
                query.Parameters.AddWithValue("@uid", _doctorusername);
                int userCount = (int)(long)query.ExecuteScalar();
                conn.Close();
                if (userCount > 0)
                {    
                    try
                    {
                        //Zapytanie do bazy pobierające dane lekarza
                       conn.Open();
                        string sql = "SELECT users.firstName,users.secondName,users.mail,doctors.roomNumber,doctors.phoneNumber FROM doctors INNER JOIN users ON doctors.uid = users.uid WHERE users.uid = @uid";
                        MySqlCommand cmd = new MySqlCommand();
                        cmd.Connection = conn;
                        cmd.CommandText = sql;
                        cmd.Parameters.AddWithValue("@uid", _doctorusername);
                        //Dane z kwerendy dodane są do tabeli danych
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);      
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        dDoctor dok = new dDoctor(dataTable.Rows[0].Field<string>(0), dataTable.Rows[0].Field<string>(1), dataTable.Rows[0].Field<string>(2), dataTable.Rows[0].Field<string>(3), dataTable.Rows[0].Field<string>(4));
                        conn.Close();

                        //Stworzenie nowego okienka inforamycjnego z danymi lekarz
                        dMainViewDoctor LookupdoctorWindow = new dMainViewDoctor(dok);
                        LookupdoctorWindow.Show();                                  
                    }
                    catch (MySql.Data.MySqlClient.MySqlException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Niepoprawne id lekarza");
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }



        }

        private void Check_Patient()
        {
            try
            {
                MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(SessionManager.connStrSQL);
                conn.Open();

                // Zapytanie do bazy o użytkownika z danym peselem
                MySqlCommand query = new MySqlCommand();
                query.Connection = conn;
                query.CommandText = @"SELECT COUNT(*) FROM patients WHERE PESEL = @pesel;";
                query.Parameters.AddWithValue("@pesel", _patientPesel);
                int userCount = (int)(long)query.ExecuteScalar();
                conn.Close();
                if (userCount > 0)
                {
                    try
                    {
                        //Zapytanie do bazy pobierające dane pacjenta
                        conn.Open();
                        string sql = @"SELECT users.firstName,users.secondName,users.mail,patients.phoneNumber,patients.address FROM patients INNER JOIN users ON patients.uid = users.uid WHERE patients.PESEL = @pesel";
                        MySqlCommand cmd = new MySqlCommand();
                        cmd.Connection = conn;
                        cmd.CommandText = sql;
                        cmd.Parameters.AddWithValue("@pesel", _patientPesel);
                        //Dane z kwerendy dodane są do tabeli danych
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        Patient pat = new Patient(dataTable.Rows[0].Field<string>(0), dataTable.Rows[0].Field<string>(1), dataTable.Rows[0].Field<string>(2), dataTable.Rows[0].Field<string>(3), dataTable.Rows[0].Field<string>(4));
                        conn.Close();

                        //Stworzenie nowego okienka inforamycjnego z danymi pacjenta
                        dMainViewPatient LookuppatientWindow = new dMainViewPatient(pat);
                        LookuppatientWindow.Show();
                    }
                    catch (MySql.Data.MySqlClient.MySqlException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Nie istnieje pacjent o podanym PESELu");
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }



        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}