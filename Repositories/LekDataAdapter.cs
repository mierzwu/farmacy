using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace FarmacySystem.Repositories
{
    public class LekDataAdapter
    {
        private readonly string connectionString;

        public LekDataAdapter()
        {
            connectionString = ConfigurationManager.ConnectionStrings["AptekaConnectionString"].ConnectionString;
        }

        public DataSet PobierzDataSet()
        {
            DataSet dataSet = new DataSet("LekDataSet");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT 
                            id, 
                            nazwa_handlowa, 
                            substancja_czynna, 
                            producent, 
                            cena_brutto, 
                            czy_na_recepte
                        FROM leki 
                        ORDER BY nazwa_handlowa";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    adapter.Fill(dataSet, "Leki");

                    // Skonfiguruj kolumnę id jako AutoIncrement
                    DataColumn idColumn = dataSet.Tables["Leki"].Columns["id"];
                    idColumn.AutoIncrement = true;

                    // Ustaw seed na wartość większą od maksymalnego ID w bazie
                    int maxId = 0;
                    if (dataSet.Tables["Leki"].Rows.Count > 0)
                    {
                        maxId = dataSet.Tables["Leki"].AsEnumerable()
                            .Max(row => row.Field<int>("id"));
                    }
                    idColumn.AutoIncrementSeed = maxId + 1;
                    idColumn.AutoIncrementStep = 1;
                    idColumn.ReadOnly = false; // Pozwól na modyfikację przez adapter

                    if (dataSet.Tables["Leki"].Rows.Count > 0)
                    {
                        dataSet.Tables["Leki"].PrimaryKey = new DataColumn[] 
                        { 
                            dataSet.Tables["Leki"].Columns["id"] 
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Błąd podczas pobierania DataSet leków: " + ex.Message, ex);
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
                            nazwa_handlowa, 
                            substancja_czynna, 
                            producent, 
                            cena_brutto, 
                            czy_na_recepte
                        FROM leki";

                    SqlDataAdapter adapter = new SqlDataAdapter(selectQuery, connection);
                    SqlCommandBuilder builder = new SqlCommandBuilder(adapter);

                    // Konfiguracja InsertCommand aby pobierać wygenerowane ID
                    adapter.InsertCommand = builder.GetInsertCommand();
                    adapter.InsertCommand.CommandText += "; SELECT id, nazwa_handlowa, substancja_czynna, producent, cena_brutto, czy_na_recepte FROM leki WHERE id = SCOPE_IDENTITY()";
                    adapter.InsertCommand.UpdatedRowSource = UpdateRowSource.FirstReturnedRecord;

                    affectedRows = adapter.Update(dataSet.Tables["Leki"]);

                    // Zaktualizuj AutoIncrementSeed do aktualnego maksymalnego ID
                    if (dataSet.Tables["Leki"].Rows.Count > 0)
                    {
                        int maxId = dataSet.Tables["Leki"].AsEnumerable()
                            .Max(row => row.Field<int>("id"));
                        dataSet.Tables["Leki"].Columns["id"].AutoIncrementSeed = maxId + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Błąd podczas zapisywania zmian leków: " + ex.Message, ex);
            }

            return affectedRows;
        }

        public bool Waliduj(DataRow row, out string bladWalidacji)
        {
            bladWalidacji = string.Empty;

            try
            {
                if (row["nazwa_handlowa"] == DBNull.Value || string.IsNullOrWhiteSpace(row["nazwa_handlowa"].ToString()))
                {
                    bladWalidacji = "Pole 'Nazwa handlowa' jest wymagane.";
                    return false;
                }

                if (row["substancja_czynna"] == DBNull.Value || string.IsNullOrWhiteSpace(row["substancja_czynna"].ToString()))
                {
                    bladWalidacji = "Pole 'Substancja czynna' jest wymagane.";
                    return false;
                }

                if (row["cena_brutto"] == DBNull.Value)
                {
                    bladWalidacji = "Pole 'Cena brutto' jest wymagane.";
                    return false;
                }

                decimal cena = Convert.ToDecimal(row["cena_brutto"]);
                if (cena <= 0)
                {
                    bladWalidacji = "Cena brutto musi być większa od 0.";
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
    }
}
