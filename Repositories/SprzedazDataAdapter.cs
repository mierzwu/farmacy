using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace FarmacySystem.Repositories
{
    public class SprzedazDataAdapter
    {
        private readonly string connectionString;

        public SprzedazDataAdapter()
        {
            connectionString = ConfigurationManager.ConnectionStrings["AptekaConnectionString"].ConnectionString;
        }

        public DataSet PobierzDaneDoSprzedazy()
        {
            DataSet dataSet = new DataSet("SprzedazDataSet");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Tabela lokalizacji
                    string queryLokalizacje = "SELECT id, nazwa FROM lokalizacje ORDER BY nazwa";
                    SqlDataAdapter lokalizacjeAdapter = new SqlDataAdapter(queryLokalizacje, connection);
                    lokalizacjeAdapter.Fill(dataSet, "Lokalizacje");

                    // Tabela klientów
                    string queryKlienci = @"
                        SELECT 
                            id, 
                            imie, 
                            nazwisko, 
                            pesel,
                            imie + ' ' + nazwisko AS nazwa_pelna
                        FROM klienci 
                        ORDER BY nazwisko, imie";
                    SqlDataAdapter klienciAdapter = new SqlDataAdapter(queryKlienci, connection);
                    klienciAdapter.Fill(dataSet, "Klienci");

                    // Tabela leków
                    string queryLeki = @"
                        SELECT 
                            id, 
                            nazwa_handlowa, 
                            substancja_czynna,
                            cena_brutto,
                            czy_na_recepte
                        FROM leki 
                        ORDER BY nazwa_handlowa";
                    SqlDataAdapter lekiAdapter = new SqlDataAdapter(queryLeki, connection);
                    lekiAdapter.Fill(dataSet, "Leki");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Błąd podczas pobierania danych do sprzedaży: " + ex.Message, ex);
            }

            return dataSet;
        }

        public int PobierzStanMagazynowy(int idLokalizacji, int idLeku)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT ISNULL(SUM(ilosc), 0) 
                        FROM stany_magazynowe 
                        WHERE id_lokalizacji = @idLokalizacji 
                          AND id_leku = @idLeku 
                          AND status = 'dostępny'";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@idLokalizacji", idLokalizacji);
                        command.Parameters.AddWithValue("@idLeku", idLeku);

                        object result = command.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Błąd podczas sprawdzania stanu magazynowego: " + ex.Message, ex);
            }
        }

        public bool KlientPosiadaRecepteNaLek(int idKlienta, int idLeku)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT COUNT(*) 
                        FROM recepty 
                        WHERE id_klienta = @idKlienta 
                          AND id_leku = @idLeku 
                          AND czy_zrealizowana = 0
                          AND data_waznosci >= GETDATE()";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@idKlienta", idKlienta);
                        command.Parameters.AddWithValue("@idLeku", idLeku);

                        int count = Convert.ToInt32(command.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Błąd podczas sprawdzania recepty: " + ex.Message, ex);
            }
        }

        public bool ZrealizujSprzedaz(int idKlienta, int idLeku, int idLokalizacji, int ilosc, decimal kwota, out int idReceptyDoZrealizowania)
        {
            idReceptyDoZrealizowania = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // 1. Znajdź receptę (jeśli potrzebna)
                            string queryRecepta = @"
                                SELECT TOP 1 id 
                                FROM recepty 
                                WHERE id_klienta = @idKlienta 
                                  AND id_leku = @idLeku 
                                  AND czy_zrealizowana = 0
                                  AND data_waznosci >= GETDATE()
                                ORDER BY data_waznosci ASC";

                            using (SqlCommand commandRecepta = new SqlCommand(queryRecepta, connection, transaction))
                            {
                                commandRecepta.Parameters.AddWithValue("@idKlienta", idKlienta);
                                commandRecepta.Parameters.AddWithValue("@idLeku", idLeku);

                                object result = commandRecepta.ExecuteScalar();
                                if (result != null)
                                {
                                    idReceptyDoZrealizowania = Convert.ToInt32(result);
                                }
                            }

                            // 2. Oznacz receptę jako zrealizowaną
                            if (idReceptyDoZrealizowania > 0)
                            {
                                string updateRecepta = @"
                                    UPDATE recepty 
                                    SET czy_zrealizowana = 1 
                                    WHERE id = @idRecepty";

                                using (SqlCommand commandUpdate = new SqlCommand(updateRecepta, connection, transaction))
                                {
                                    commandUpdate.Parameters.AddWithValue("@idRecepty", idReceptyDoZrealizowania);
                                    commandUpdate.ExecuteNonQuery();
                                }
                            }

                            // 3. Zmniejsz stan magazynowy
                            string updateStan = @"
                                UPDATE stany_magazynowe 
                                SET ilosc = ilosc - @ilosc 
                                WHERE id_lokalizacji = @idLokalizacji 
                                  AND id_leku = @idLeku 
                                  AND status = 'dostępny'
                                  AND ilosc >= @ilosc";

                            using (SqlCommand commandStan = new SqlCommand(updateStan, connection, transaction))
                            {
                                commandStan.Parameters.AddWithValue("@ilosc", ilosc);
                                commandStan.Parameters.AddWithValue("@idLokalizacji", idLokalizacji);
                                commandStan.Parameters.AddWithValue("@idLeku", idLeku);

                                int rowsAffected = commandStan.ExecuteNonQuery();
                                if (rowsAffected == 0)
                                {
                                    transaction.Rollback();
                                    throw new Exception("Nie można zmniejszyć stanu magazynowego. Sprawdź dostępność.");
                                }
                            }

                            transaction.Commit();
                            return true;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Błąd podczas realizacji sprzedaży: " + ex.Message, ex);
            }
        }
    }
}
