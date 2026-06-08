using System;
using System.Windows.Forms;

namespace FarmacySystem
{
    public partial class MainMenuForm : Form
    {
        public MainMenuForm()
        {
            InitializeComponent();
        }

        private void btnLokalizacje_Click(object sender, EventArgs e)
        {
            LokalizacjaDataSetForm form = new LokalizacjaDataSetForm();
            form.ShowDialog();
        }

        private void btnPracownicy_Click(object sender, EventArgs e)
        {
            PracownikDataSetForm form = new PracownikDataSetForm();
            form.ShowDialog();
        }

        private void btnKlienci_Click(object sender, EventArgs e)
        {
            KlientDataSetForm form = new KlientDataSetForm();
            form.ShowDialog();
        }

        private void btnLeki_Click(object sender, EventArgs e)
        {
            LekDataSetForm form = new LekDataSetForm();
            form.ShowDialog();
        }

        private void btnRecepty_Click(object sender, EventArgs e)
        {
            ReceptaDataSetForm form = new ReceptaDataSetForm();
            form.ShowDialog();
        }

        private void btnMagazyn_Click(object sender, EventArgs e)
        {
            StanMagazynowyDataSetForm form = new StanMagazynowyDataSetForm();
            form.ShowDialog();
        }

        private void btnSprzedaz_Click(object sender, EventArgs e)
        {
            SprzedazForm form = new SprzedazForm();
            form.ShowDialog();
        }

        private void btnWyjscie_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
