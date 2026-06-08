using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace FarmacySystem.Repositories
{
    public class StanMagazynowyDataAdapter
    {
        private readonly string connectionString;

        public StanMagazynowyDataAdapter()
        {
            connectionString = ConfigurationManager.ConnectionStrings["AptekaConnectionString"].ConnectionString;
        }

        public DataSet PobierzDataSet()
        {
            DataSet dataSet = new DataSet("StanMagazynowyDataSet");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Główna tabela: Stany magazynowe z JOIN
                    string queryStany = @"
                        SELECT 
                            s.id, 
                            s.id_lokalizacji, 
                            s.id_leku, 
                            s.ilosc, 
                            s.data_waznosci, 
                            s.status,
                            lok.nazwa AS nazwa_lokalizacji,
                            l.nazwa_handlowa AS nazwa_leku
                        FROM stany_magazynowe s
                        INNER JOIN lokalizacje lok ON s.id_lokalizacji = lok.id
                        INNER JOIN leki l ON s.id_leku = l.id
                        ORDER BY lok.nazwa, l.nazwa_handlowa";

                    SqlDataAdapter adapter = new SqlDataAdapter(queryStany, connection);
                    adapter.Fill(dataSet, "StanyMagazynowe");

                    // Skonfiguruj kolumnę id jako AutoIncrement
                    DataColumn idColumn = dataSet.Tables["StanyMagazynowe"].Columns["id"];
                    idColumn.AutoIncrement = true;

                    // Ustaw seed na wartość większą od maksymalnego ID w bazie
                    int maxId = 0;
                    if (dataSet.Tables["StanyMagazynowe"].Rows.Count > 0)
                    {
                        maxId = dataSet.Tables["StanyMagazynowe"].AsEnumerable()
                            .Max(row => row.Field<int>("id"));
                    }
                    idColumn.AutoIncrementSeed = maxId + 1;
                    idColumn.AutoIncrementStep = 1;
                    idColumn.ReadOnly = false; // Pozwól na modyfikację przez adapter

                    if (dataSet.Tables["StanyMagazynowe"].Rows.Count > 0)
                    {
                        dataSet.Tables["StanyMagazynowe"].PrimaryKey = new DataColumn[] 
                        { 
                            dataSet.Tables["StanyMagazynowe"].Columns["id"] 
                        };
                    }

                    // Tabela pomocnicza: Lokalizacje (dla ComboBox)
                    string queryLokalizacje = "SELECT id, nazwa FROM lokalizacje ORDER BY nazwa";
                    SqlDataAdapter lokalizacjeAdapter = new SqlDataAdapter(queryLokalizacje, connection);
                    lokalizacjeAdapter.Fill(dataSet, "Lokalizacje");

                    // Tabela pomocnicza: Leki (dla ComboBox)
                    string queryLeki = "SELECT id, nazwa_handlowa FROM leki ORDER BY nazwa_handlowa";
                    SqlDataAdapter lekiAdapter = new SqlDataAdapter(queryLeki, connection);
                    lekiAdapter.Fill(dataSet, "Leki");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Błąd podczas pobierania DataSet stanów magazynowych: " + ex.Message, ex);
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
                            id_lokalizacji, 
                            id_leku, 
                            ilosc, 
                            data_waznosci, 
                            status
                        FROM stany_magazynowe";

                    SqlDataAdapter adapter = new SqlDataAdapter(selectQuery, connection);
                    SqlCommandBuilder builder = new SqlCommandBuilder(adapter);

                    // Konfiguracja InsertCommand aby pobierać wygenerowane ID
                    adapter.InsertCommand = builder.GetInsertCommand();
                    adapter.InsertCommand.CommandText += "; SELECT id, id_lokalizacji, id_leku, ilosc, data_waznosci, status FROM stany_magazynowe WHERE id = SCOPE_IDENTITY()";
                    adapter.InsertCommand.UpdatedRowSource = UpdateRowSource.FirstReturnedRecord;

                    // Usunięcie kolumn z JOIN
                    DataTable dtToUpdate = dataSet.Tables["StanyMagazynowe"].Copy();
                    if (dtToUpdate.Columns.Contains("nazwa_lokalizacji"))
                        dtToUpdate.Columns.Remove("nazwa_lokalizacji");
                    if (dtToUpdate.Columns.Contains("nazwa_leku"))
                        dtToUpdate.Columns.Remove("nazwa_leku");

                    affectedRows = adapter.Update(dtToUpdate);

                    // Zaktualizuj AutoIncrementSeed do aktualnego maksymalnego ID
                    if (dataSet.Tables["StanyMagazynowe"].Rows.Count > 0)
                    {
                        int maxId = dataSet.Tables["StanyMagazynowe"].AsEnumerable()
                            .Max(row => row.Field<int>("id"));
                        dataSet.Tables["StanyMagazynowe"].Columns["id"].AutoIncrementSeed = maxId + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Błąd podczas zapisywania zmian stanów magazynowych: " + ex.Message, ex);
            }

            return affectedRows;
        }

        public bool Waliduj(DataRow row, out string bladWalidacji)
        {
            bladWalidacji = string.Empty;

            try
            {
                if (row["id_lokalizacji"] == DBNull.Value)
                {
                    bladWalidacji = "Należy wybrać lokalizację.";
                    return false;
                }

                if (row["id_leku"] == DBNull.Value)
                {
                    bladWalidacji = "Należy wybrać lek.";
                    return false;
                }

                if (row["ilosc"] == DBNull.Value)
                {
                    bladWalidacji = "Pole 'Ilość' jest wymagane.";
                    return false;
                }

                int ilosc = Convert.ToInt32(row["ilosc"]);
                if (ilosc < 0)
                {
                    bladWalidacji = "Ilość nie może być ujemna.";
                    return false;
                }

                if (row["status"] != DBNull.Value && !string.IsNullOrWhiteSpace(row["status"].ToString()))
                {
                    string status = row["status"].ToString().ToLower();
                    if (status != "dostępny" && status != "zarezerwowany" && status != "niedostępny")
                    {
                        bladWalidacji = "Status musi być: 'dostępny', 'zarezerwowany' lub 'niedostępny'.";
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
