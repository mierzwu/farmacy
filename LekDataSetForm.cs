using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using FarmacySystem.Repositories;

namespace FarmacySystem
{
    public partial class LekDataSetForm : Form
    {
        private LekDataAdapter dataAdapter;
        private DataSet dataSet;
        private DataTable dataTable;
        private BindingSource bindingSource;

        private BindingNavigator bindingNavigator1;
        private DataGridView dgvLeki;
        private GroupBox groupBox1;
        private Label label1, label2, label3, label4, label5, lblLiczbaRekordow;
        private TextBox txtNazwaHandlowa, txtSubstancjaCzynna, txtCenaBrutto, txtProducent;
        private CheckBox chkCzyNaRecepte;
        private Button btnDodaj, btnUsun, btnZapisz, btnAnuluj, btnOdswiez, btnZamknij;
        private System.ComponentModel.IContainer components = null;

        public LekDataSetForm()
        {
            InitializeComponent();
            dataAdapter = new LekDataAdapter();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.Text = "Zarządzanie Lekami - DataSet";
            this.Size = new Size(1100, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Load += LekDataSetForm_Load;
            this.FormClosing += LekDataSetForm_FormClosing;

            bindingNavigator1 = new BindingNavigator(this.components);
            bindingNavigator1.AddStandardItems();
            bindingNavigator1.Dock = DockStyle.Top;
            this.Controls.Add(bindingNavigator1);

            dgvLeki = new DataGridView();
            dgvLeki.Location = new Point(12, 35);
            dgvLeki.Size = new Size(650, 550);
            dgvLeki.ReadOnly = true;
            dgvLeki.AllowUserToAddRows = false;
            dgvLeki.AllowUserToDeleteRows = false;
            dgvLeki.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.Controls.Add(dgvLeki);

            groupBox1 = new GroupBox();
            groupBox1.Text = "Szczegóły leku";
            groupBox1.Location = new Point(670, 35);
            groupBox1.Size = new Size(400, 450);
            this.Controls.Add(groupBox1);

            int y = 30;
            label1 = new Label { Text = "Nazwa handlowa:", Location = new Point(15, y), Size = new Size(120, 20) };
            groupBox1.Controls.Add(label1);
            txtNazwaHandlowa = new TextBox { Location = new Point(140, y), Size = new Size(240, 20) };
            groupBox1.Controls.Add(txtNazwaHandlowa);
            y += 35;

            label2 = new Label { Text = "Substancja czynna:", Location = new Point(15, y), Size = new Size(120, 20) };
            groupBox1.Controls.Add(label2);
            txtSubstancjaCzynna = new TextBox { Location = new Point(140, y), Size = new Size(240, 20) };
            groupBox1.Controls.Add(txtSubstancjaCzynna);
            y += 35;

            label3 = new Label { Text = "Cena brutto (PLN):", Location = new Point(15, y), Size = new Size(120, 20) };
            groupBox1.Controls.Add(label3);
            txtCenaBrutto = new TextBox { Location = new Point(140, y), Size = new Size(240, 20) };
            groupBox1.Controls.Add(txtCenaBrutto);
            y += 35;

            label4 = new Label { Text = "Producent:", Location = new Point(15, y), Size = new Size(120, 20) };
            groupBox1.Controls.Add(label4);
            txtProducent = new TextBox { Location = new Point(140, y), Size = new Size(240, 20) };
            groupBox1.Controls.Add(txtProducent);
            y += 35;

            label5 = new Label { Text = "Na receptę:", Location = new Point(15, y), Size = new Size(120, 20) };
            groupBox1.Controls.Add(label5);
            chkCzyNaRecepte = new CheckBox { Location = new Point(140, y), Size = new Size(240, 20), Text = "Wymaga recepty" };
            groupBox1.Controls.Add(chkCzyNaRecepte);
            y += 45;

            btnDodaj = new Button { Text = "Dodaj nowy", Location = new Point(15, y), Size = new Size(115, 30) };
            btnDodaj.Click += btnDodaj_Click;
            groupBox1.Controls.Add(btnDodaj);

            btnUsun = new Button { Text = "Usuń", Location = new Point(140, y), Size = new Size(115, 30) };
            btnUsun.Click += btnUsun_Click;
            groupBox1.Controls.Add(btnUsun);
            y += 40;

            btnZapisz = new Button { Text = "Zapisz zmiany", Location = new Point(15, y), Size = new Size(115, 30) };
            btnZapisz.Click += btnZapisz_Click;
            groupBox1.Controls.Add(btnZapisz);

            btnAnuluj = new Button { Text = "Anuluj zmiany", Location = new Point(140, y), Size = new Size(115, 30) };
            btnAnuluj.Click += btnAnuluj_Click;
            groupBox1.Controls.Add(btnAnuluj);
            y += 40;

            btnOdswiez = new Button { Text = "Odśwież", Location = new Point(15, y), Size = new Size(115, 30) };
            btnOdswiez.Click += btnOdswiez_Click;
            groupBox1.Controls.Add(btnOdswiez);

            btnZamknij = new Button { Text = "Zamknij", Location = new Point(140, y), Size = new Size(115, 30) };
            btnZamknij.Click += btnZamknij_Click;
            groupBox1.Controls.Add(btnZamknij);

            lblLiczbaRekordow = new Label();
            lblLiczbaRekordow.Text = "Liczba rekordów: 0";
            lblLiczbaRekordow.Location = new Point(670, 495);
            lblLiczbaRekordow.Size = new Size(200, 20);
            lblLiczbaRekordow.Font = new Font(lblLiczbaRekordow.Font, FontStyle.Bold);
            this.Controls.Add(lblLiczbaRekordow);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void LekDataSetForm_Load(object sender, EventArgs e)
        {
            try { ZaladujDane(); }
            catch (Exception ex) { MessageBox.Show("Błąd: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void ZaladujDane()
        {
            dataSet = dataAdapter.PobierzDataSet();
            dataTable = dataSet.Tables["Leki"];
            bindingSource = new BindingSource { DataSource = dataTable };
            dgvLeki.DataSource = bindingSource;
            bindingNavigator1.BindingSource = bindingSource;

            txtNazwaHandlowa.DataBindings.Clear();
            txtSubstancjaCzynna.DataBindings.Clear();
            txtCenaBrutto.DataBindings.Clear();
            txtProducent.DataBindings.Clear();
            chkCzyNaRecepte.DataBindings.Clear();

            txtNazwaHandlowa.DataBindings.Add("Text", bindingSource, "nazwa_handlowa", true, DataSourceUpdateMode.OnPropertyChanged);
            txtSubstancjaCzynna.DataBindings.Add("Text", bindingSource, "substancja_czynna", true, DataSourceUpdateMode.OnPropertyChanged);
            txtCenaBrutto.DataBindings.Add("Text", bindingSource, "cena_brutto", true, DataSourceUpdateMode.OnPropertyChanged);
            txtProducent.DataBindings.Add("Text", bindingSource, "producent", true, DataSourceUpdateMode.OnPropertyChanged);
            chkCzyNaRecepte.DataBindings.Add("Checked", bindingSource, "czy_na_recepte", true, DataSourceUpdateMode.OnPropertyChanged);

            FormatujKolumny();
            lblLiczbaRekordow.Text = $"Liczba rekordów: {dataTable.Rows.Count}";
        }

        private void FormatujKolumny()
        {
            if (dgvLeki.Columns.Count > 0)
            {
                dgvLeki.Columns["id"].HeaderText = "ID";
                dgvLeki.Columns["id"].Width = 50;
                dgvLeki.Columns["nazwa_handlowa"].HeaderText = "Nazwa handlowa";
                dgvLeki.Columns["nazwa_handlowa"].Width = 180;
                dgvLeki.Columns["substancja_czynna"].HeaderText = "Substancja czynna";
                dgvLeki.Columns["substancja_czynna"].Width = 180;
                dgvLeki.Columns["producent"].HeaderText = "Producent";
                dgvLeki.Columns["producent"].Width = 150;
                dgvLeki.Columns["cena_brutto"].HeaderText = "Cena brutto (PLN)";
                dgvLeki.Columns["cena_brutto"].Width = 100;
                dgvLeki.Columns["cena_brutto"].DefaultCellStyle.Format = "N2";
                dgvLeki.Columns["czy_na_recepte"].HeaderText = "Na receptę";
                dgvLeki.Columns["czy_na_recepte"].Width = 80;
            }
        }

        private void btnDodaj_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow newRow = dataTable.NewRow();
                newRow["nazwa_handlowa"] = "";
                newRow["substancja_czynna"] = "";
                newRow["producent"] = "";
                newRow["cena_brutto"] = 0;
                newRow["czy_na_recepte"] = false;
                dataTable.Rows.Add(newRow);
                bindingSource.MoveLast();
                lblLiczbaRekordow.Text = $"Liczba rekordów: {dataTable.Rows.Count}";

                // Wymuś focus na pierwszym polu
                txtNazwaHandlowa.Focus();
                txtNazwaHandlowa.SelectAll();
            }
            catch (Exception ex) { MessageBox.Show("Błąd: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnUsun_Click(object sender, EventArgs e)
        {
            try
            {
                if (bindingSource.Current == null) return;
                DataRowView currentRow = (DataRowView)bindingSource.Current;
                if (MessageBox.Show($"Usunąć '{currentRow["nazwa_handlowa"]}'?", "Potwierdzenie", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
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
                DataTable changedTable = dataSet.Tables["Leki"].GetChanges();
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

        private void LekDataSetForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (dataSet != null && dataSet.HasChanges() && MessageBox.Show("Masz niezapisane zmiany. Zamknąć?", "Ostrzeżenie", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                e.Cancel = true;
        }
    }
}
