using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace FarmacySystem.Repositories
{
    public class LokalizacjaDataAdapter
    {
        private readonly string connectionString;

        public LokalizacjaDataAdapter()
        {
            connectionString = ConfigurationManager.ConnectionStrings["AptekaConnectionString"].ConnectionString;
        }

        public DataSet PobierzDataSet()
        {
            DataSet dataSet = new DataSet("LokalizacjaDataSet");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT 
                            id, 
                            nazwa, 
                            miasto, 
                            adres, 
                            telefon
                        FROM lokalizacje 
                        ORDER BY nazwa";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    adapter.Fill(dataSet, "Lokalizacje");

                    // Skonfiguruj kolumnę id jako AutoIncrement
                    DataColumn idColumn = dataSet.Tables["Lokalizacje"].Columns["id"];
                    idColumn.AutoIncrement = true;

                    // Ustaw seed na wartość większą od maksymalnego ID w bazie
                    int maxId = 0;
                    if (dataSet.Tables["Lokalizacje"].Rows.Count > 0)
                    {
                        maxId = dataSet.Tables["Lokalizacje"].AsEnumerable()
                            .Max(row => row.Field<int>("id"));
                    }
                    idColumn.AutoIncrementSeed = maxId + 1;
                    idColumn.AutoIncrementStep = 1;
                    idColumn.ReadOnly = false; // Pozwól na modyfikację przez adapter

                    if (dataSet.Tables["Lokalizacje"].Rows.Count > 0)
                    {
                        dataSet.Tables["Lokalizacje"].PrimaryKey = new DataColumn[] 
                        { 
                            dataSet.Tables["Lokalizacje"].Columns["id"] 
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Błąd podczas pobierania DataSet lokalizacji: " + ex.Message, ex);
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
                            nazwa, 
                            miasto, 
                            adres, 
                            telefon
                        FROM lokalizacje";

                    SqlDataAdapter adapter = new SqlDataAdapter(selectQuery, connection);
                    SqlCommandBuilder builder = new SqlCommandBuilder(adapter);

                    // Konfiguracja InsertCommand aby pobierać wygenerowane ID
                    adapter.InsertCommand = builder.GetInsertCommand();
                    adapter.InsertCommand.CommandText += "; SELECT id, nazwa, miasto, adres, telefon FROM lokalizacje WHERE id = SCOPE_IDENTITY()";
                    adapter.InsertCommand.UpdatedRowSource = UpdateRowSource.FirstReturnedRecord;

                    affectedRows = adapter.Update(dataSet.Tables["Lokalizacje"]);

                    // Zaktualizuj AutoIncrementSeed do aktualnego maksymalnego ID
                    if (dataSet.Tables["Lokalizacje"].Rows.Count > 0)
                    {
                        int maxId = dataSet.Tables["Lokalizacje"].AsEnumerable()
                            .Max(row => row.Field<int>("id"));
                        dataSet.Tables["Lokalizacje"].Columns["id"].AutoIncrementSeed = maxId + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Błąd podczas zapisywania zmian lokalizacji: " + ex.Message, ex);
            }

            return affectedRows;
        }

        public bool Waliduj(DataRow row, out string bladWalidacji)
        {
            bladWalidacji = string.Empty;

            try
            {
                if (row["nazwa"] == DBNull.Value || string.IsNullOrWhiteSpace(row["nazwa"].ToString()))
                {
                    bladWalidacji = "Pole 'Nazwa' jest wymagane.";
                    return false;
                }

                if (row["miasto"] == DBNull.Value || string.IsNullOrWhiteSpace(row["miasto"].ToString()))
                {
                    bladWalidacji = "Pole 'Miasto' jest wymagane.";
                    return false;
                }

                if (row["adres"] == DBNull.Value || string.IsNullOrWhiteSpace(row["adres"].ToString()))
                {
                    bladWalidacji = "Pole 'Adres' jest wymagane.";
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
    }
}
