using System;

namespace FarmacySystem.Models
{
    public class Pracownik
    {
        public int Id { get; set; }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public string Pesel { get; set; }
        public string Email { get; set; }
        public string Stanowisko { get; set; }
        public int IdLokalizacji { get; set; }
        public DateTime DataZatrudnienia { get; set; }

        public string NazwaLokalizacji { get; set; }
    }
}
