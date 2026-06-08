using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using FarmacySystem.Repositories;

namespace FarmacySystem
{
    public partial class KlientDataSetForm : Form
    {
        private KlientDataAdapter dataAdapter;
        private DataSet dataSet;
        private DataTable dataTable;
        private BindingSource bindingSource;

        private BindingNavigator bindingNavigator1;
        private DataGridView dgvKlienci;
        private GroupBox groupBox1;
        private Label label1, label2, label3, label4, label5, lblLiczbaRekordow;
        private TextBox txtImie, txtNazwisko, txtPesel, txtTelefon;
        private CheckBox chkCzyUbezpieczony;
        private Button btnDodaj, btnUsun, btnZapisz, btnAnuluj, btnOdswiez, btnZamknij;
        private System.ComponentModel.IContainer components = null;

        public KlientDataSetForm()
        {
            InitializeComponent();
            dataAdapter = new KlientDataAdapter();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.Text = "Zarządzanie Klientami - DataSet";
            this.Size = new Size(1100, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Load += KlientDataSetForm_Load;
            this.FormClosing += KlientDataSetForm_FormClosing;

            bindingNavigator1 = new BindingNavigator(this.components);
            bindingNavigator1.AddStandardItems();
            bindingNavigator1.Dock = DockStyle.Top;
            this.Controls.Add(bindingNavigator1);

            dgvKlienci = new DataGridView();
            dgvKlienci.Location = new Point(12, 35);
            dgvKlienci.Size = new Size(650, 550);
            dgvKlienci.ReadOnly = true;
            dgvKlienci.AllowUserToAddRows = false;
            dgvKlienci.AllowUserToDeleteRows = false;
            dgvKlienci.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.Controls.Add(dgvKlienci);

            groupBox1 = new GroupBox();
            groupBox1.Text = "Szczegóły klienta";
            groupBox1.Location = new Point(670, 35);
            groupBox1.Size = new Size(400, 400);
            this.Controls.Add(groupBox1);

            int y = 30;
            label1 = new Label { Text = "Imię:", Location = new Point(15, y), Size = new Size(100, 20) };
            groupBox1.Controls.Add(label1);
            txtImie = new TextBox { Location = new Point(120, y), Size = new Size(260, 20) };
            groupBox1.Controls.Add(txtImie);
            y += 35;

            label2 = new Label { Text = "Nazwisko:", Location = new Point(15, y), Size = new Size(100, 20) };
            groupBox1.Controls.Add(label2);
            txtNazwisko = new TextBox { Location = new Point(120, y), Size = new Size(260, 20) };
            groupBox1.Controls.Add(txtNazwisko);
            y += 35;

            label3 = new Label { Text = "PESEL:", Location = new Point(15, y), Size = new Size(100, 20) };
            groupBox1.Controls.Add(label3);
            txtPesel = new TextBox { Location = new Point(120, y), Size = new Size(260, 20), MaxLength = 11 };
            groupBox1.Controls.Add(txtPesel);
            y += 35;

            label4 = new Label { Text = "Telefon:", Location = new Point(15, y), Size = new Size(100, 20) };
            groupBox1.Controls.Add(label4);
            txtTelefon = new TextBox { Location = new Point(120, y), Size = new Size(260, 20) };
            groupBox1.Controls.Add(txtTelefon);
            y += 35;

            chkCzyUbezpieczony = new CheckBox { Text = "Ubezpieczony", Location = new Point(120, y), Size = new Size(260, 20) };
            groupBox1.Controls.Add(chkCzyUbezpieczony);
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
            lblLiczbaRekordow.Location = new Point(670, 445);
            lblLiczbaRekordow.Size = new Size(200, 20);
            lblLiczbaRekordow.Font = new Font(lblLiczbaRekordow.Font, FontStyle.Bold);
            this.Controls.Add(lblLiczbaRekordow);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void KlientDataSetForm_Load(object sender, EventArgs e)
        {
            try { ZaladujDane(); }
            catch (Exception ex) { MessageBox.Show("Błąd: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void ZaladujDane()
        {
            dataSet = dataAdapter.PobierzDataSet();
            dataTable = dataSet.Tables["Klienci"];
            bindingSource = new BindingSource { DataSource = dataTable };
            dgvKlienci.DataSource = bindingSource;
            bindingNavigator1.BindingSource = bindingSource;

            txtImie.DataBindings.Clear();
            txtNazwisko.DataBindings.Clear();
            txtPesel.DataBindings.Clear();
            txtTelefon.DataBindings.Clear();
            chkCzyUbezpieczony.DataBindings.Clear();

            txtImie.DataBindings.Add("Text", bindingSource, "imie", true, DataSourceUpdateMode.OnPropertyChanged);
            txtNazwisko.DataBindings.Add("Text", bindingSource, "nazwisko", true, DataSourceUpdateMode.OnPropertyChanged);
            txtPesel.DataBindings.Add("Text", bindingSource, "pesel", true, DataSourceUpdateMode.OnPropertyChanged);
            txtTelefon.DataBindings.Add("Text", bindingSource, "telefon", true, DataSourceUpdateMode.OnPropertyChanged);
            chkCzyUbezpieczony.DataBindings.Add("Checked", bindingSource, "czy_ubezpieczony", true, DataSourceUpdateMode.OnPropertyChanged);

            FormatujKolumny();
            lblLiczbaRekordow.Text = $"Liczba rekordów: {dataTable.Rows.Count}";
        }

        private void FormatujKolumny()
        {
            if (dgvKlienci.Columns.Count > 0)
            {
                dgvKlienci.Columns["id"].HeaderText = "ID";
                dgvKlienci.Columns["id"].Width = 50;
                dgvKlienci.Columns["imie"].HeaderText = "Imię";
                dgvKlienci.Columns["imie"].Width = 120;
                dgvKlienci.Columns["nazwisko"].HeaderText = "Nazwisko";
                dgvKlienci.Columns["nazwisko"].Width = 150;
                dgvKlienci.Columns["pesel"].HeaderText = "PESEL";
                dgvKlienci.Columns["pesel"].Width = 120;
                dgvKlienci.Columns["telefon"].HeaderText = "Telefon";
                dgvKlienci.Columns["telefon"].Width = 120;
                dgvKlienci.Columns["czy_ubezpieczony"].HeaderText = "Ubezpieczony";
                dgvKlienci.Columns["czy_ubezpieczony"].Width = 100;
            }
        }

        private void btnDodaj_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow newRow = dataTable.NewRow();
                newRow["imie"] = "";
                newRow["nazwisko"] = "";
                newRow["pesel"] = "";
                newRow["telefon"] = DBNull.Value;
                newRow["czy_ubezpieczony"] = false;
                dataTable.Rows.Add(newRow);
                bindingSource.MoveLast();
                lblLiczbaRekordow.Text = $"Liczba rekordów: {dataTable.Rows.Count}";

                // Wymuś focus na pierwszym polu
                txtImie.Focus();
                txtImie.SelectAll();
            }
            catch (Exception ex) { MessageBox.Show("Błąd: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnUsun_Click(object sender, EventArgs e)
        {
            try
            {
                if (bindingSource.Current == null) return;
                DataRowView currentRow = (DataRowView)bindingSource.Current;
                if (MessageBox.Show($"Usunąć '{currentRow["imie"]} {currentRow["nazwisko"]}'?", "Potwierdzenie", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
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
                DataTable changedTable = dataSet.Tables["Klienci"].GetChanges();
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

        private void KlientDataSetForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (dataSet != null && dataSet.HasChanges() && MessageBox.Show("Masz niezapisane zmiany. Zamknąć?", "Ostrzeżenie", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                e.Cancel = true;
        }
    }
}
