using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using FarmacySystem.Repositories;

namespace FarmacySystem
{
    public partial class PracownikDataSetForm : Form
    {
        private PracownikDataAdapter dataAdapter;
        private DataSet dataSet;
        private DataTable dataTable;
        private BindingSource bindingSource;

        private BindingNavigator bindingNavigator1;
        private DataGridView dgvPracownicy;
        private GroupBox groupBox1;
        private Label label1, label2, label3, label4, label5, label6, label7, lblLiczbaRekordow;
        private TextBox txtImie, txtNazwisko, txtPesel, txtEmail, txtStanowisko;
        private ComboBox cmbLokalizacja;
        private DateTimePicker dtpDataZatrudnienia;
        private Button btnDodaj, btnUsun, btnZapisz, btnAnuluj, btnOdswiez, btnZamknij;
        private System.ComponentModel.IContainer components = null;

        public PracownikDataSetForm()
        {
            InitializeComponent();
            dataAdapter = new PracownikDataAdapter();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.Text = "Zarz¹dzanie Pracownikami - DataSet";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Load += PracownikDataSetForm_Load;
            this.FormClosing += PracownikDataSetForm_FormClosing;

            bindingNavigator1 = new BindingNavigator(this.components);
            bindingNavigator1.AddStandardItems();
            bindingNavigator1.Dock = DockStyle.Top;
            this.Controls.Add(bindingNavigator1);

            dgvPracownicy = new DataGridView();
            dgvPracownicy.Location = new Point(12, 35);
            dgvPracownicy.Size = new Size(750, 600);
            dgvPracownicy.ReadOnly = true;
            dgvPracownicy.AllowUserToAddRows = false;
            dgvPracownicy.AllowUserToDeleteRows = false;
            dgvPracownicy.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.Controls.Add(dgvPracownicy);

            groupBox1 = new GroupBox();
            groupBox1.Text = "Szczegó³y pracownika";
            groupBox1.Location = new Point(770, 35);
            groupBox1.Size = new Size(410, 520);
            this.Controls.Add(groupBox1);

            int y = 30;
            label1 = new Label { Text = "Imiê:", Location = new Point(15, y), Size = new Size(100, 20) };
            groupBox1.Controls.Add(label1);
            txtImie = new TextBox { Location = new Point(120, y), Size = new Size(270, 20) };
            groupBox1.Controls.Add(txtImie);
            y += 35;

            label2 = new Label { Text = "Nazwisko:", Location = new Point(15, y), Size = new Size(100, 20) };
            groupBox1.Controls.Add(label2);
            txtNazwisko = new TextBox { Location = new Point(120, y), Size = new Size(270, 20) };
            groupBox1.Controls.Add(txtNazwisko);
            y += 35;

            label3 = new Label { Text = "PESEL:", Location = new Point(15, y), Size = new Size(100, 20) };
            groupBox1.Controls.Add(label3);
            txtPesel = new TextBox { Location = new Point(120, y), Size = new Size(270, 20), MaxLength = 11 };
            groupBox1.Controls.Add(txtPesel);
            y += 35;

            label4 = new Label { Text = "Email:", Location = new Point(15, y), Size = new Size(100, 20) };
            groupBox1.Controls.Add(label4);
            txtEmail = new TextBox { Location = new Point(120, y), Size = new Size(270, 20) };
            groupBox1.Controls.Add(txtEmail);
            y += 35;

            label5 = new Label { Text = "Stanowisko:", Location = new Point(15, y), Size = new Size(100, 20) };
            groupBox1.Controls.Add(label5);
            txtStanowisko = new TextBox { Location = new Point(120, y), Size = new Size(270, 20) };
            groupBox1.Controls.Add(txtStanowisko);
            y += 35;

            label6 = new Label { Text = "Lokalizacja:", Location = new Point(15, y), Size = new Size(100, 20) };
            groupBox1.Controls.Add(label6);
            cmbLokalizacja = new ComboBox { Location = new Point(120, y), Size = new Size(270, 20), DropDownStyle = ComboBoxStyle.DropDownList };
            groupBox1.Controls.Add(cmbLokalizacja);
            y += 35;

            label7 = new Label { Text = "Data zatrudnienia:", Location = new Point(15, y), Size = new Size(100, 20) };
            groupBox1.Controls.Add(label7);
            dtpDataZatrudnienia = new DateTimePicker { Location = new Point(120, y), Size = new Size(270, 20), Format = DateTimePickerFormat.Short };
            groupBox1.Controls.Add(dtpDataZatrudnienia);
            y += 55;

            btnDodaj = new Button { Text = "Dodaj nowy", Location = new Point(15, y), Size = new Size(120, 30) };
            btnDodaj.Click += btnDodaj_Click;
            groupBox1.Controls.Add(btnDodaj);

            btnUsun = new Button { Text = "Usuñ", Location = new Point(145, y), Size = new Size(120, 30) };
            btnUsun.Click += btnUsun_Click;
            groupBox1.Controls.Add(btnUsun);
            y += 40;

            btnZapisz = new Button { Text = "Zapisz zmiany", Location = new Point(15, y), Size = new Size(120, 30) };
            btnZapisz.Click += btnZapisz_Click;
            groupBox1.Controls.Add(btnZapisz);

            btnAnuluj = new Button { Text = "Anuluj zmiany", Location = new Point(145, y), Size = new Size(120, 30) };
            btnAnuluj.Click += btnAnuluj_Click;
            groupBox1.Controls.Add(btnAnuluj);
            y += 40;

            btnOdswiez = new Button { Text = "Odœwie¿", Location = new Point(15, y), Size = new Size(120, 30) };
            btnOdswiez.Click += btnOdswiez_Click;
            groupBox1.Controls.Add(btnOdswiez);

            btnZamknij = new Button { Text = "Zamknij", Location = new Point(145, y), Size = new Size(120, 30) };
            btnZamknij.Click += btnZamknij_Click;
            groupBox1.Controls.Add(btnZamknij);

            lblLiczbaRekordow = new Label();
            lblLiczbaRekordow.Text = "Liczba rekordów: 0";
            lblLiczbaRekordow.Location = new Point(770, 565);
            lblLiczbaRekordow.Size = new Size(200, 20);
            lblLiczbaRekordow.Font = new Font(lblLiczbaRekordow.Font, FontStyle.Bold);
            this.Controls.Add(lblLiczbaRekordow);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void PracownikDataSetForm_Load(object sender, EventArgs e)
        {
            try { ZaladujDane(); }
            catch (Exception ex) { MessageBox.Show("B³¹d: " + ex.Message, "B³¹d", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void ZaladujDane()
        {
            dataSet = dataAdapter.PobierzDataSet();
            dataTable = dataSet.Tables["Pracownicy"];
            bindingSource = new BindingSource { DataSource = dataTable };
            dgvPracownicy.DataSource = bindingSource;
            bindingNavigator1.BindingSource = bindingSource;

            txtImie.DataBindings.Clear();
            txtNazwisko.DataBindings.Clear();
            txtPesel.DataBindings.Clear();
            txtEmail.DataBindings.Clear();
            txtStanowisko.DataBindings.Clear();
            dtpDataZatrudnienia.DataBindings.Clear();
            cmbLokalizacja.DataBindings.Clear();

            txtImie.DataBindings.Add("Text", bindingSource, "imie", true, DataSourceUpdateMode.OnPropertyChanged);
            txtNazwisko.DataBindings.Add("Text", bindingSource, "nazwisko", true, DataSourceUpdateMode.OnPropertyChanged);
            txtPesel.DataBindings.Add("Text", bindingSource, "pesel", true, DataSourceUpdateMode.OnPropertyChanged);
            txtEmail.DataBindings.Add("Text", bindingSource, "email", true, DataSourceUpdateMode.OnPropertyChanged);
            txtStanowisko.DataBindings.Add("Text", bindingSource, "stanowisko", true, DataSourceUpdateMode.OnPropertyChanged);
            dtpDataZatrudnienia.DataBindings.Add("Value", bindingSource, "data_zatrudnienia", true, DataSourceUpdateMode.OnPropertyChanged);

            cmbLokalizacja.DataSource = dataSet.Tables["Lokalizacje"];
            cmbLokalizacja.DisplayMember = "nazwa";
            cmbLokalizacja.ValueMember = "id";
            cmbLokalizacja.DataBindings.Add("SelectedValue", bindingSource, "id_lokalizacji", true, DataSourceUpdateMode.OnPropertyChanged);

            FormatujKolumny();
            lblLiczbaRekordow.Text = $"Liczba rekordów: {dataTable.Rows.Count}";
        }

        private void FormatujKolumny()
        {
            if (dgvPracownicy.Columns.Count > 0)
            {
                dgvPracownicy.Columns["id"].HeaderText = "ID";
                dgvPracownicy.Columns["id"].Width = 50;
                dgvPracownicy.Columns["imie"].HeaderText = "Imiê";
                dgvPracownicy.Columns["imie"].Width = 100;
                dgvPracownicy.Columns["nazwisko"].HeaderText = "Nazwisko";
                dgvPracownicy.Columns["nazwisko"].Width = 120;
                dgvPracownicy.Columns["pesel"].HeaderText = "PESEL";
                dgvPracownicy.Columns["pesel"].Width = 100;
                dgvPracownicy.Columns["email"].HeaderText = "Email";
                dgvPracownicy.Columns["email"].Width = 150;
                dgvPracownicy.Columns["stanowisko"].HeaderText = "Stanowisko";
                dgvPracownicy.Columns["stanowisko"].Width = 120;
                dgvPracownicy.Columns["id_lokalizacji"].Visible = false;
                dgvPracownicy.Columns["data_zatrudnienia"].HeaderText = "Data zatrudnienia";
                dgvPracownicy.Columns["data_zatrudnienia"].Width = 120;
                dgvPracownicy.Columns["data_zatrudnienia"].DefaultCellStyle.Format = "dd.MM.yyyy";
                dgvPracownicy.Columns["nazwa_lokalizacji"].HeaderText = "Lokalizacja";
                dgvPracownicy.Columns["nazwa_lokalizacji"].Width = 150;
            }
        }

        private void btnDodaj_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataSet.Tables["Lokalizacje"].Rows.Count == 0) { MessageBox.Show("Brak lokalizacji w bazie.", "B³¹d", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                DataRow newRow = dataTable.NewRow();
                newRow["imie"] = "";
                newRow["nazwisko"] = "";
                newRow["pesel"] = "";
                newRow["email"] = "";
                newRow["stanowisko"] = "";
                newRow["id_lokalizacji"] = dataSet.Tables["Lokalizacje"].Rows[0]["id"];
                newRow["data_zatrudnienia"] = DateTime.Now;
                newRow["nazwa_lokalizacji"] = dataSet.Tables["Lokalizacje"].Rows[0]["nazwa"];
                dataTable.Rows.Add(newRow);
                bindingSource.MoveLast();
                lblLiczbaRekordow.Text = $"Liczba rekordów: {dataTable.Rows.Count}";

                // Wymuœ focus na pierwszym polu
                txtImie.Focus();
                txtImie.SelectAll();
            }
            catch (Exception ex) { MessageBox.Show("B³¹d: " + ex.Message, "B³¹d", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnUsun_Click(object sender, EventArgs e)
        {
            try
            {
                if (bindingSource.Current == null) return;
                DataRowView currentRow = (DataRowView)bindingSource.Current;
                if (MessageBox.Show($"Usun¹æ '{currentRow["imie"]} {currentRow["nazwisko"]}'?", "Potwierdzenie", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    currentRow.Row.Delete();
                    lblLiczbaRekordow.Text = $"Liczba rekordów: {dataTable.Rows.Count}";
                }
            }
            catch (Exception ex) { MessageBox.Show("B³¹d: " + ex.Message, "B³¹d", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnZapisz_Click(object sender, EventArgs e)
        {
            try
            {
                // KLUCZOWE: Zakoñcz edycjê bie¿¹cego rekordu przed walidacj¹
                bindingSource.EndEdit();

                if (!dataSet.HasChanges()) { MessageBox.Show("Brak zmian.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
                DataTable changedTable = dataSet.Tables["Pracownicy"].GetChanges();
                if (changedTable != null)
                {
                    foreach (DataRow row in changedTable.Rows)
                    {
                        if (row.RowState != DataRowState.Deleted)
                        {
                            // Walidacja daty zatrudnienia
                            if (row["data_zatrudnienia"] != DBNull.Value)
                            {
                                DateTime dataZatrudnienia = Convert.ToDateTime(row["data_zatrudnienia"]);
                                if (dataZatrudnienia > DateTime.Now)
                                {
                                    MessageBox.Show("Data zatrudnienia nie mo¿e byæ w przysz³oœci.", "B³¹d walidacji", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                            }

                            string bladWalidacji;
                            if (!dataAdapter.Waliduj(row, out bladWalidacji)) { MessageBox.Show($"B³¹d walidacji: {bladWalidacji}", "B³¹d", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                        }
                    }
                }
                int affected = dataAdapter.ZapiszZmiany(dataSet);
                dataSet.AcceptChanges(); // Zaakceptuj zmiany zamiast prze³adowywaæ
                MessageBox.Show($"Zapisano: {affected} rekordów", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                lblLiczbaRekordow.Text = $"Liczba rekordów: {dataTable.Rows.Count}";
            }
            catch (Exception ex) { MessageBox.Show("B³¹d: " + ex.Message, "B³¹d", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnAnuluj_Click(object sender, EventArgs e)
        {
            try
            {
                if (!dataSet.HasChanges()) return;
                if (MessageBox.Show("Anulowaæ zmiany?", "Potwierdzenie", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    dataSet.RejectChanges();
            }
            catch (Exception ex) { MessageBox.Show("B³¹d: " + ex.Message, "B³¹d", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnOdswiez_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataSet.HasChanges() && MessageBox.Show("Utracisz niezapisane zmiany. Kontynuowaæ?", "Ostrze¿enie", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No) return;
                ZaladujDane();
            }
            catch (Exception ex) { MessageBox.Show("B³¹d: " + ex.Message, "B³¹d", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnZamknij_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PracownikDataSetForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (dataSet != null && dataSet.HasChanges() && MessageBox.Show("Masz niezapisane zmiany. Zamkn¹æ?", "Ostrze¿enie", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                e.Cancel = true;
        }
    }
}
