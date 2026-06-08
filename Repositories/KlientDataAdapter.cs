using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace FarmacySystem.Repositories
{
    public class KlientDataAdapter
    {
        private readonly string connectionString;

        public KlientDataAdapter()
        {
            connectionString = ConfigurationManager.ConnectionStrings["AptekaConnectionString"].ConnectionString;
        }

        public DataSet PobierzDataSet()
        {
            DataSet dataSet = new DataSet("KlientDataSet");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT 
                            id, 
                            imie, 
                            nazwisko, 
                            pesel, 
                            telefon, 
                            czy_ubezpieczony
                        FROM klienci 
                        ORDER BY nazwisko, imie";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    adapter.Fill(dataSet, "Klienci");

                    // Skonfiguruj kolumnę id jako AutoIncrement
                    DataColumn idColumn = dataSet.Tables["Klienci"].Columns["id"];
                    idColumn.AutoIncrement = true;

                    // Ustaw seed na wartość większą od maksymalnego ID w bazie
                    int maxId = 0;
                    if (dataSet.Tables["Klienci"].Rows.Count > 0)
                    {
                        maxId = dataSet.Tables["Klienci"].AsEnumerable()
                            .Max(row => row.Field<int>("id"));
                    }
                    idColumn.AutoIncrementSeed = maxId + 1;
                    idColumn.AutoIncrementStep = 1;
                    idColumn.ReadOnly = false; // Pozwól na modyfikację przez adapter

                    if (dataSet.Tables["Klienci"].Rows.Count > 0)
                    {
                        dataSet.Tables["Klienci"].PrimaryKey = new DataColumn[] 
                        { 
                            dataSet.Tables["Klienci"].Columns["id"] 
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Błąd podczas pobierania DataSet klientów: " + ex.Message, ex);
            }

            return dataSet;
        }

        public int ZapiszZmiany(DataSet dataSet)
        {
            int affectedRows = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string selectQuery = @"
                        SELECT 
                            id, 
                            imie, 
                            nazwisko, 
                            pesel, 
                            telefon, 
                            czy_ubezpieczony
                        FROM klienci";

                    SqlDataAdapter adapter = new SqlDataAdapter(selectQuery, connection);
                    SqlCommandBuilder builder = new SqlCommandBuilder(adapter);

                    // Konfiguracja InsertCommand aby pobierać wygenerowane ID
                    adapter.InsertCommand = builder.GetInsertCommand();
                    adapter.InsertCommand.CommandText += "; SELECT id, imie, nazwisko, pesel, telefon, czy_ubezpieczony FROM klienci WHERE id = SCOPE_IDENTITY()";
                    adapter.InsertCommand.UpdatedRowSource = UpdateRowSource.FirstReturnedRecord;

                    affectedRows = adapter.Update(dataSet.Tables["Klienci"]);

                    // Zaktualizuj AutoIncrementSeed do aktualnego maksymalnego ID
                    if (dataSet.Tables["Klienci"].Rows.Count > 0)
                    {
                        int maxId = dataSet.Tables["Klienci"].AsEnumerable()
                            .Max(row => row.Field<int>("id"));
                        dataSet.Tables["Klienci"].Columns["id"].AutoIncrementSeed = maxId + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Błąd podczas zapisywania zmian klientów: " + ex.Message, ex);
            }

            return affectedRows;
        }

        public bool Waliduj(DataRow row, out string bladWalidacji)
        {
            bladWalidacji = string.Empty;

            try
            {
                if (row["imie"] == DBNull.Value || string.IsNullOrWhiteSpace(row["imie"].ToString()))
                {
                    bladWalidacji = "Pole 'Imię' jest wymagane.";
                    return false;
                }

                if (row["nazwisko"] == DBNull.Value || string.IsNullOrWhiteSpace(row["nazwisko"].ToString()))
                {
                    bladWalidacji = "Pole 'Nazwisko' jest wymagane.";
                    return false;
                }

                if (row["pesel"] == DBNull.Value || string.IsNullOrWhiteSpace(row["pesel"].ToString()))
                {
                    bladWalidacji = "Pole 'PESEL' jest wymagane.";
                    return false;
                }

                if (row["pesel"].ToString().Length != 11)
                {
                    bladWalidacji = "PESEL musi mieć dokładnie 11 cyfr.";
                    return false;
                }

                // Walidacja czy PESEL zawiera tylko cyfry
                if (!row["pesel"].ToString().All(char.IsDigit))
                {
                    bladWalidacji = "PESEL może zawierać tylko cyfry.";
                    return false;
                }

                // Walidacja sumy kontrolnej PESEL
                if (!WalidujPeselSumaKontrolna(row["pesel"].ToString()))
                {
                    bladWalidacji = "PESEL jest nieprawidłowy (błędna suma kontrolna).";
                    return false;
                }

                // Walidacja telefonu (jeśli podany)
                if (row["telefon"] != DBNull.Value && !string.IsNullOrWhiteSpace(row["telefon"].ToString()))
                {
                    string telefon = row["telefon"].ToString().Replace(" ", "").Replace("-", "").Replace("+", "");

                    if (!telefon.All(char.IsDigit))
                    {
                        bladWalidacji = "Numer telefonu może zawierać tylko cyfry, spacje, myślniki i znak '+'.";
                        return false;
                    }

                    if (telefon.Length < 9 || telefon.Length > 15)
                    {
                        bladWalidacji = "Numer telefonu musi mieć od 9 do 15 cyfr.";
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                bladWalidacji = "Błąd walidacji: " + ex.Message;
                return false;
            }
        }

        private bool WalidujPeselSumaKontrolna(string pesel)
        {
            if (pesel.Length != 11) return false;

            int[] wagi = { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };
            int suma = 0;

            for (int i = 0; i < 10; i++)
            {
                suma += (pesel[i] - '0') * wagi[i];
            }

            int sumaKontrolna = (10 - (suma % 10)) % 10;
            int cyfraKontrolna = pesel[10] - '0';

            return sumaKontrolna == cyfraKontrolna;
        }
    }
}
