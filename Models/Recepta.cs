using System;

namespace FarmacySystem.Models
{
    public class Recepta
    {
        public int Id { get; set; }
        public string NumerRecepty { get; set; }
        public int IdKlienta { get; set; }
        public int IdLeku { get; set; }
        public DateTime DataWystawienia { get; set; }
        public DateTime DataWaznosci { get; set; }
        public string LekarzImieNazwisko { get; set; }
        public bool CzyZrealizowana { get; set; }

        public string KlientImieNazwisko { get; set; }
        public string NazwaLeku { get; set; }
    }
}
