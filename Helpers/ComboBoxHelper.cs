using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace FarmacySystem.Helpers
{
    public class ComboBoxItem
    {
        public int Value { get; set; }
        public string Display { get; set; }

        public override string ToString()
        {
            return Display;
        }
    }

    public static class ComboBoxHelper
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["AptekaConnectionString"].ConnectionString;

        public static List<ComboBoxItem> PobierzLokalizacje()
        {
            List<ComboBoxItem> lista = new List<ComboBoxItem>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT id, nazwa FROM lokalizacje ORDER BY nazwa";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(new ComboBoxItem 
                    { 
                        Value = reader.GetInt32(0), 
                        Display = reader.GetString(1) 
                    });
                }
            }

            return lista;
        }

        public static List<ComboBoxItem> PobierzKlientow()
        {
            List<ComboBoxItem> lista = new List<ComboBoxItem>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT id, ISNULL(imie + ' ' + nazwisko, 'ID: ' + CAST(id AS VARCHAR)) AS nazwa FROM klienci ORDER BY nazwisko, imie";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(new ComboBoxItem 
                    { 
                        Value = reader.GetInt32(0), 
                        Display = reader.GetString(1) 
                    });
                }
            }

            return lista;
        }

        public static List<ComboBoxItem> PobierzLeki()
        {
            List<ComboBoxItem> lista = new List<ComboBoxItem>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT id, nazwa_handlowa FROM leki ORDER BY nazwa_handlowa";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(new ComboBoxItem 
                    { 
                        Value = reader.GetInt32(0), 
                        Display = reader.GetString(1) 
                    });
                }
            }

            return lista;
        }
    }
}
