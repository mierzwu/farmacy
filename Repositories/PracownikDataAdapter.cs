using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace FarmacySystem.Repositories
{
    public class PracownikDataAdapter
    {
        private readonly string connectionString;

        public PracownikDataAdapter()
        {
            connectionString = ConfigurationManager.ConnectionStrings["AptekaConnectionString"].ConnectionString;
        }

        public DataSet PobierzDataSet()
        {
            DataSet dataSet = new DataSet("PracownikDataSet");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Główna tabela: Pracownicy z JOIN
                    string queryPracownicy = @"
                        SELECT 
                            p.id, 
                            p.imie, 
                            p.nazwisko, 
                            p.pesel, 
                            p.email, 
                            p.stanowisko, 
                            p.id_lokalizacji, 
                            p.data_zatrudnienia,
                            l.nazwa AS nazwa_lokalizacji
                        FROM pracownicy p
                        INNER JOIN lokalizacje l ON p.id_lokalizacji = l.id
                        ORDER BY p.nazwisko, p.imie";

                    SqlDataAdapter adapter = new SqlDataAdapter(queryPracownicy, connection);
                    adapter.Fill(dataSet, "Pracownicy");

                    // Skonfiguruj kolumnę id jako AutoIncrement
                    DataColumn idColumn = dataSet.Tables["Pracownicy"].Columns["id"];
                    idColumn.AutoIncrement = true;

                    // Ustaw seed na wartość większą od maksymalnego ID w bazie
                    int maxId = 0;
                    if (dataSet.Tables["Pracownicy"].Rows.Count > 0)
                    {
                        maxId = dataSet.Tables["Pracownicy"].AsEnumerable()
                            .Max(row => row.Field<int>("id"));
                    }
                    idColumn.AutoIncrementSeed = maxId + 1;
                    idColumn.AutoIncrementStep = 1;
                    idColumn.ReadOnly = false; // Pozwól na modyfikację przez adapter

                    if (dataSet.Tables["Pracownicy"].Rows.Count > 0)
                    {
                        dataSet.Tables["Pracownicy"].PrimaryKey = new DataColumn[] 
                        { 
                            dataSet.Tables["Pracownicy"].Columns["id"] 
                        };
                    }

                    // Tabela pomocnicza: Lokalizacje (dla ComboBox)
                    string queryLokalizacje = "SELECT id, nazwa FROM lokalizacje ORDER BY nazwa";
                    SqlDataAdapter lokalizacjeAdapter = new SqlDataAdapter(queryLokalizacje, connection);
                    lokalizacjeAdapter.Fill(dataSet, "Lokalizacje");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Błąd podczas pobierania DataSet pracowników: " + ex.Message, ex);
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
                            email, 
                            stanowisko, 
                            id_lokalizacji, 
                            data_zatrudnienia
                        FROM pracownicy";

                    SqlDataAdapter adapter = new SqlDataAdapter(selectQuery, connection);
                    SqlCommandBuilder builder = new SqlCommandBuilder(adapter);

                    // Konfiguracja InsertCommand aby pobierać wygenerowane ID
                    adapter.InsertCommand = builder.GetInsertCommand();
                    adapter.InsertCommand.CommandText += "; SELECT id, imie, nazwisko, pesel, email, stanowisko, id_lokalizacji, data_zatrudnienia FROM pracownicy WHERE id = SCOPE_IDENTITY()";
                    adapter.InsertCommand.UpdatedRowSource = UpdateRowSource.FirstReturnedRecord;

                    // Usunięcie kolumny z JOIN
                    DataTable dtToUpdate = dataSet.Tables["Pracownicy"].Copy();
                    if (dtToUpdate.Columns.Contains("nazwa_lokalizacji"))
                        dtToUpdate.Columns.Remove("nazwa_lokalizacji");

                    affectedRows = adapter.Update(dtToUpdate);

                    // Zaktualizuj AutoIncrementSeed do aktualnego maksymalnego ID
                    if (dataSet.Tables["Pracownicy"].Rows.Count > 0)
                    {
                        int maxId = dataSet.Tables["Pracownicy"].AsEnumerable()
                            .Max(row => row.Field<int>("id"));
                        dataSet.Tables["Pracownicy"].Columns["id"].AutoIncrementSeed = maxId + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Błąd podczas zapisywania zmian pracowników: " + ex.Message, ex);
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

                if (row["email"] != DBNull.Value && !string.IsNullOrWhiteSpace(row["email"].ToString()))
                {
                    string email = row["email"].ToString();
                    if (!email.Contains("@") || !email.Contains("."))
                    {
                        bladWalidacji = "Email musi zawierać '@' i domenę.";
                        return false;
                    }
                }

                if (row["stanowisko"] == DBNull.Value || string.IsNullOrWhiteSpace(row["stanowisko"].ToString()))
                {
                    bladWalidacji = "Pole 'Stanowisko' jest wymagane.";
                    return false;
                }

                if (row["id_lokalizacji"] == DBNull.Value)
                {
                    bladWalidacji = "Należy wybrać lokalizację.";
                    return false;
                }

                if (row["data_zatrudnienia"] == DBNull.Value)
                {
                    bladWalidacji = "Data zatrudnienia jest wymagana.";
                    return false;
                }

                DateTime dataZatrudnienia = Convert.ToDateTime(row["data_zatrudnienia"]);
                if (dataZatrudnienia > DateTime.Now)
                {
                    bladWalidacji = "Data zatrudnienia nie może być w przyszłości.";
                    return false;
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
