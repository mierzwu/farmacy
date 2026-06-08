using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using FarmacySystem.Repositories;

namespace FarmacySystem
{
    public partial class LokalizacjaDataSetForm : Form
    {
        private LokalizacjaDataAdapter dataAdapter;
        private DataSet dataSet;
        private DataTable dataTable;
        private BindingSource bindingSource;

        public LokalizacjaDataSetForm()
        {
            InitializeComponent();
            dataAdapter = new LokalizacjaDataAdapter();
        }

        private void LokalizacjaDataSetForm_Load(object sender, EventArgs e)
        {
            try { ZaladujDane(); }
            catch (Exception ex) { MessageBox.Show("Błąd podczas ładowania danych: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void ZaladujDane()
        {
            dataSet = dataAdapter.PobierzDataSet();
            dataTable = dataSet.Tables["Lokalizacje"];
            bindingSource = new BindingSource { DataSource = dataTable };
            dgvLokalizacje.DataSource = bindingSource;
            bindingNavigator1.BindingSource = bindingSource;

            txtNazwa.DataBindings.Clear();
            txtMiasto.DataBindings.Clear();
            txtAdres.DataBindings.Clear();
            txtTelefon.DataBindings.Clear();

            txtNazwa.DataBindings.Add("Text", bindingSource, "nazwa", true, DataSourceUpdateMode.OnPropertyChanged);
            txtMiasto.DataBindings.Add("Text", bindingSource, "miasto", true, DataSourceUpdateMode.OnPropertyChanged);
            txtAdres.DataBindings.Add("Text", bindingSource, "adres", true, DataSourceUpdateMode.OnPropertyChanged);
            txtTelefon.DataBindings.Add("Text", bindingSource, "telefon", true, DataSourceUpdateMode.OnPropertyChanged);

            FormatujKolumny();
            lblLiczbaRekordow.Text = $"Liczba rekordów: {dataTable.Rows.Count}";
        }

        private void FormatujKolumny()
        {
            if (dgvLokalizacje.Columns.Count > 0)
            {
                dgvLokalizacje.Columns["id"].HeaderText = "ID";
                dgvLokalizacje.Columns["id"].Width = 50;
                dgvLokalizacje.Columns["nazwa"].HeaderText = "Nazwa";
                dgvLokalizacje.Columns["nazwa"].Width = 150;
                dgvLokalizacje.Columns["miasto"].HeaderText = "Miasto";
                dgvLokalizacje.Columns["miasto"].Width = 120;
                dgvLokalizacje.Columns["adres"].HeaderText = "Adres";
                dgvLokalizacje.Columns["adres"].Width = 200;
                dgvLokalizacje.Columns["telefon"].HeaderText = "Telefon";
                dgvLokalizacje.Columns["telefon"].Width = 100;
            }
        }

        private void btnDodaj_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow newRow = dataTable.NewRow();
                newRow["nazwa"] = "";
                newRow["miasto"] = "";
                newRow["adres"] = "";
                newRow["telefon"] = DBNull.Value;
                dataTable.Rows.Add(newRow);
                bindingSource.MoveLast();
                lblLiczbaRekordow.Text = $"Liczba rekordów: {dataTable.Rows.Count}";

                // Wymuś focus na pierwszym polu aby umożliwić edycję
                txtNazwa.Focus();
                txtNazwa.SelectAll();
            }
            catch (Exception ex) { MessageBox.Show("Błąd: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnUsun_Click(object sender, EventArgs e)
        {
            try
            {
                if (bindingSource.Current == null) { MessageBox.Show("Nie wybrano rekordu.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
                DataRowView currentRow = (DataRowView)bindingSource.Current;
                if (MessageBox.Show($"Usunąć '{currentRow["nazwa"]}'?", "Potwierdzenie", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    currentRow.Row.Delete();
                    lblLiczbaRekordow.Text = $"Liczba rekordów: {dataTable.Rows.Count}";
                }
            }
            catch (Exception ex) { MessageBox.Show("Błąd: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnZapisz_Click(object sender, EventArgs e)
        {
            try
            {
                // KLUCZOWE: Zakończ edycję bieżącego rekordu przed walidacją
                bindingSource.EndEdit();

                if (!dataSet.HasChanges()) { MessageBox.Show("Brak zmian.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
                DataTable changedTable = dataSet.Tables["Lokalizacje"].GetChanges();
                if (changedTable != null)
                {
                    foreach (DataRow row in changedTable.Rows)
                    {
                        if (row.RowState != DataRowState.Deleted)
                        {
                            string bladWalidacji;
                            if (!dataAdapter.Waliduj(row, out bladWalidacji)) { MessageBox.Show($"Błąd walidacji: {bladWalidacji}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                        }
                    }
                }
                int affected = dataAdapter.ZapiszZmiany(dataSet);
                dataSet.AcceptChanges(); // Zaakceptuj zmiany zamiast przeładowywać
                MessageBox.Show($"Zapisano: {affected} rekordów", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                lblLiczbaRekordow.Text = $"Liczba rekordów: {dataTable.Rows.Count}";
            }
            catch (Exception ex) { MessageBox.Show("Błąd: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnAnuluj_Click(object sender, EventArgs e)
        {
            try
            {
                if (!dataSet.HasChanges()) return;
                if (MessageBox.Show("Anulować zmiany?", "Potwierdzenie", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    dataSet.RejectChanges();
            }
            catch (Exception ex) { MessageBox.Show("Błąd: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnOdswiez_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataSet.HasChanges() && MessageBox.Show("Utracisz niezapisane zmiany. Kontynuować?", "Ostrzeżenie", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No) return;
                ZaladujDane();
            }
            catch (Exception ex) { MessageBox.Show("Błąd: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnZamknij_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LokalizacjaDataSetForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (dataSet != null && dataSet.HasChanges() && MessageBox.Show("Masz niezapisane zmiany. Zamknąć?", "Ostrzeżenie", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                e.Cancel = true;
        }
    }
}
