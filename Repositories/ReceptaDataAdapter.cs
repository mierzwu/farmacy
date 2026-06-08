using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace FarmacySystem.Repositories
{
    public class ReceptaDataAdapter
    {
        private readonly string connectionString;

        public ReceptaDataAdapter()
        {
            connectionString = ConfigurationManager.ConnectionStrings["AptekaConnectionString"].ConnectionString;
        }

        public DataSet PobierzDataSet()
        {
            DataSet dataSet = new DataSet("ReceptaDataSet");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Główna tabela: Recepty z JOIN
                    string queryRecepty = @"
                        SELECT 
                            r.id, 
                            r.numer_recepty, 
                            r.id_klienta, 
                            r.id_leku, 
                            r.data_wystawienia, 
                            r.data_waznosci, 
                            r.lekarz_imie_nazwisko, 
                            r.czy_zrealizowana,
                            k.imie + ' ' + k.nazwisko AS klient_imie_nazwisko,
                            l.nazwa_handlowa AS nazwa_leku
                        FROM recepty r
                        INNER JOIN klienci k ON r.id_klienta = k.id
                        INNER JOIN leki l ON r.id_leku = l.id
                        ORDER BY r.data_wystawienia DESC";

                    SqlDataAdapter adapter = new SqlDataAdapter(queryRecepty, connection);
                    adapter.Fill(dataSet, "Recepty");

                    // Skonfiguruj kolumnę id jako AutoIncrement
                    DataColumn idColumn = dataSet.Tables["Recepty"].Columns["id"];
                    idColumn.AutoIncrement = true;

                    // Ustaw seed na wartość większą od maksymalnego ID w bazie
                    int maxId = 0;
                    if (dataSet.Tables["Recepty"].Rows.Count > 0)
                    {
                        maxId = dataSet.Tables["Recepty"].AsEnumerable()
                            .Max(row => row.Field<int>("id"));
                    }
                    idColumn.AutoIncrementSeed = maxId + 1;
                    idColumn.AutoIncrementStep = 1;
                    idColumn.ReadOnly = false; // Pozwól na modyfikację przez adapter

                    if (dataSet.Tables["Recepty"].Rows.Count > 0)
                    {
                        dataSet.Tables["Recepty"].PrimaryKey = new DataColumn[] 
                        { 
                            dataSet.Tables["Recepty"].Columns["id"] 
                        };
                    }

                    // Tabela pomocnicza: Klienci (dla ComboBox)
                    string queryKlienci = "SELECT id, imie + ' ' + nazwisko AS nazwa_pelna FROM klienci ORDER BY nazwisko, imie";
                    SqlDataAdapter klienciAdapter = new SqlDataAdapter(queryKlienci, connection);
                    klienciAdapter.Fill(dataSet, "Klienci");

                    // Tabela pomocnicza: Leki NA RECEPTĘ (dla ComboBox)
                    string queryLeki = "SELECT id, nazwa_handlowa FROM leki WHERE czy_na_recepte = 1 ORDER BY nazwa_handlowa";
                    SqlDataAdapter lekiAdapter = new SqlDataAdapter(queryLeki, connection);
                    lekiAdapter.Fill(dataSet, "Leki");

                    // Tabela pomocnicza: Lekarze (pracownicy na stanowisku lekarz)
                    string queryLekarze = @"
                        SELECT 
                            p.id, 
                            p.imie + ' ' + p.nazwisko AS imie_nazwisko
                        FROM pracownicy p
                        WHERE p.stanowisko = 'lekarz'
                        ORDER BY p.nazwisko, p.imie";
                    SqlDataAdapter lekarzeAdapter = new SqlDataAdapter(queryLekarze, connection);
                    lekarzeAdapter.Fill(dataSet, "Lekarze");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Błąd podczas pobierania DataSet recept: " + ex.Message, ex);
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
                            numer_recepty, 
                            id_klienta, 
                            id_leku, 
                            data_wystawienia, 
                            data_waznosci, 
                            lekarz_imie_nazwisko, 
                            czy_zrealizowana
                        FROM recepty";

                    SqlDataAdapter adapter = new SqlDataAdapter(selectQuery, connection);
                    SqlCommandBuilder builder = new SqlCommandBuilder(adapter);

                    // Konfiguracja InsertCommand aby pobierać wygenerowane ID
                    adapter.InsertCommand = builder.GetInsertCommand();
                    adapter.InsertCommand.CommandText += "; SELECT id, numer_recepty, id_klienta, id_leku, data_wystawienia, data_waznosci, lekarz_imie_nazwisko, czy_zrealizowana FROM recepty WHERE id = SCOPE_IDENTITY()";
                    adapter.InsertCommand.UpdatedRowSource = UpdateRowSource.FirstReturnedRecord;

                    // Usunięcie kolumn z JOIN
                    DataTable dtToUpdate = dataSet.Tables["Recepty"].Copy();
                    if (dtToUpdate.Columns.Contains("klient_imie_nazwisko"))
                        dtToUpdate.Columns.Remove("klient_imie_nazwisko");
                    if (dtToUpdate.Columns.Contains("nazwa_leku"))
                        dtToUpdate.Columns.Remove("nazwa_leku");

                    affectedRows = adapter.Update(dtToUpdate);

                    // Zaktualizuj AutoIncrementSeed do aktualnego maksymalnego ID
                    if (dataSet.Tables["Recepty"].Rows.Count > 0)
                    {
                        int maxId = dataSet.Tables["Recepty"].AsEnumerable()
                            .Max(row => row.Field<int>("id"));
                        dataSet.Tables["Recepty"].Columns["id"].AutoIncrementSeed = maxId + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Błąd podczas zapisywania zmian recept: " + ex.Message, ex);
            }

            return affectedRows;
        }

        public bool Waliduj(DataRow row, out string bladWalidacji)
        {
            bladWalidacji = string.Empty;

            try
            {
                if (row["numer_recepty"] == DBNull.Value || string.IsNullOrWhiteSpace(row["numer_recepty"].ToString()))
                {
                    bladWalidacji = "Pole 'Numer recepty' jest wymagane.";
                    return false;
                }

                if (row["id_klienta"] == DBNull.Value)
                {
                    bladWalidacji = "Należy wybrać klienta.";
                    return false;
                }

                if (row["id_leku"] == DBNull.Value)
                {
                    bladWalidacji = "Należy wybrać lek.";
                    return false;
                }

                if (row["data_wystawienia"] == DBNull.Value)
                {
                    bladWalidacji = "Data wystawienia jest wymagana.";
                    return false;
                }

                if (row["data_waznosci"] == DBNull.Value)
                {
                    bladWalidacji = "Data ważności jest wymagana.";
                    return false;
                }

                DateTime dataWystawienia = Convert.ToDateTime(row["data_wystawienia"]);
                DateTime dataWaznosci = Convert.ToDateTime(row["data_waznosci"]);

                if (dataWaznosci <= dataWystawienia)
                {
                    bladWalidacji = "Data ważności musi być późniejsza niż data wystawienia.";
                    return false;
                }

                if (row["lekarz_imie_nazwisko"] == DBNull.Value || string.IsNullOrWhiteSpace(row["lekarz_imie_nazwisko"].ToString()))
                {
                    bladWalidacji = "Pole 'Lekarz' jest wymagane.";
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
