using System;

namespace FarmacySystem.Models
{
    public class StanMagazynowy
    {
        public int Id { get; set; }
        public int IdLokalizacji { get; set; }
        public int IdLeku { get; set; }
        public int Ilosc { get; set; }
        public DateTime? DataWaznosci { get; set; }
        public string Status { get; set; }

        public string NazwaLokalizacji { get; set; }
        public string NazwaLeku { get; set; }
    }
}
