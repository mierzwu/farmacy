using System;

namespace FarmacySystem.Models
{
    public class Klient
    {
        public int Id { get; set; }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public string Pesel { get; set; }
        public string Telefon { get; set; }
        public bool CzyUbezpieczony { get; set; }
    }
}
