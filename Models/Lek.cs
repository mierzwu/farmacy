using System;

namespace FarmacySystem.Models
{
    public class Lek
    {
        public int Id { get; set; }
        public string NazwaHandlowa { get; set; }
        public string SubstancjaCzynna { get; set; }
        public string Producent { get; set; }
        public decimal CenaBrutto { get; set; }
        public bool CzyNaRecepte { get; set; }
    }
}
