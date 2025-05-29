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
        private string _patientUsername;

        public string DoctorUsername
        {
            get => _doctorusername;
            set { _doctorusername = value; OnPropertyChanged(); }
        }

        public string PatientUsername
        {
            get => _patientUsername;
            set { _patientUsername = value; OnPropertyChanged(); }
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
                       conn.Open();
                       string sql= "SELECT `firstName`,`secondName`,`mail` FROM users WHERE uid  =@uid ";
                        MySqlCommand cmd = new MySqlCommand();
                        cmd.Connection = conn;
                        cmd.CommandText = sql;
                        cmd.Parameters.AddWithValue("@uid", _doctorusername);
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);      
                                DataTable dataTable = new DataTable();
                                adapter.Fill(dataTable);
                                Doctor dok = new Doctor(dataTable.Rows[0].Field<string>(0), dataTable.Rows[0].Field<string>(1), dataTable.Rows[0].Field<string>(2));
                                conn.Close();
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
                    MessageBox.Show("Niepoprawny id lekarza");
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

                // Zapytanie do bazy o wybranego u�ytkownika
                MySqlCommand query = new MySqlCommand();
                query.Connection = conn;
                query.CommandText = @"SELECT COUNT(*) FROM users WHERE uid = @uid AND acc_type='0';";
                query.Parameters.AddWithValue("@uid", _patientUsername);
                int userCount = (int)(long)query.ExecuteScalar();
                conn.Close();
                if (userCount > 0)
                {
                    try
                    {
                        conn.Open();
                        string sql = "SELECT `firstName`,`secondName`,`mail` FROM users WHERE uid  =@uid ";
                        MySqlCommand cmd = new MySqlCommand();
                        cmd.Connection = conn;
                        cmd.CommandText = sql;
                        cmd.Parameters.AddWithValue("@uid", _patientUsername);
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        Patient pat = new Patient(dataTable.Rows[0].Field<string>(0), dataTable.Rows[0].Field<string>(1), dataTable.Rows[0].Field<string>(2));
                        conn.Close();
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
                    MessageBox.Show("Niepoprawny pesel pacjenta");
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