using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using FarmacySystem.Repositories;

namespace FarmacySystem
{
    public partial class ReceptaDataSetForm : Form
    {
        private ReceptaDataAdapter dataAdapter;
        private DataSet dataSet;
        private DataTable dataTable;
        private BindingSource bindingSource;

        private BindingNavigator bindingNavigator1;
        private DataGridView dgvRecepty;
        private GroupBox groupBox1;
        private Label label1, label2, label3, label4, label5, label6, lblLiczbaRekordow;
        private ComboBox cmbKlient, cmbLek, cmbLekarz;
        private DateTimePicker dtpDataWystawienia, dtpDataWaznosci;
        private TextBox txtNumerRecepty;
        private CheckBox chkCzyZrealizowana;
        private Button btnDodaj, btnUsun, btnZapisz, btnAnuluj, btnOdswiez, btnZamknij;
        private System.ComponentModel.IContainer components = null;

        public ReceptaDataSetForm()
        {
            InitializeComponent();
            dataAdapter = new ReceptaDataAdapter();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.Text = "Zarządzanie Receptami - DataSet";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Load += ReceptaDataSetForm_Load;
            this.FormClosing += ReceptaDataSetForm_FormClosing;

            bindingNavigator1 = new BindingNavigator(this.components);
            bindingNavigator1.AddStandardItems();
            bindingNavigator1.Dock = DockStyle.Top;
            this.Controls.Add(bindingNavigator1);

            dgvRecepty = new DataGridView();
            dgvRecepty.Location = new Point(12, 35);
            dgvRecepty.Size = new Size(750, 600);
            dgvRecepty.ReadOnly = true;
            dgvRecepty.AllowUserToAddRows = false;
            dgvRecepty.AllowUserToDeleteRows = false;
            dgvRecepty.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRecepty.CellFormatting += dgvRecepty_CellFormatting;
            this.Controls.Add(dgvRecepty);

            groupBox1 = new GroupBox();
            groupBox1.Text = "Szczegóły recepty";
            groupBox1.Location = new Point(770, 35);
            groupBox1.Size = new Size(410, 520);
            this.Controls.Add(groupBox1);

            int y = 30;
            label1 = new Label { Text = "Numer recepty:", Location = new Point(15, y), Size = new Size(120, 20) };
            groupBox1.Controls.Add(label1);
            txtNumerRecepty = new TextBox { Location = new Point(140, y), Size = new Size(250, 20) };
            groupBox1.Controls.Add(txtNumerRecepty);
            y += 35;

            label2 = new Label { Text = "Klient:", Location = new Point(15, y), Size = new Size(120, 20) };
            groupBox1.Controls.Add(label2);
            cmbKlient = new ComboBox { Location = new Point(140, y), Size = new Size(250, 20), DropDownStyle = ComboBoxStyle.DropDownList };
            groupBox1.Controls.Add(cmbKlient);
            y += 35;

            label3 = new Label { Text = "Lek:", Location = new Point(15, y), Size = new Size(120, 20) };
            groupBox1.Controls.Add(label3);
            cmbLek = new ComboBox { Location = new Point(140, y), Size = new Size(250, 20), DropDownStyle = ComboBoxStyle.DropDownList };
            groupBox1.Controls.Add(cmbLek);
            y += 35;

            label4 = new Label { Text = "Lekarz:", Location = new Point(15, y), Size = new Size(120, 20) };
            groupBox1.Controls.Add(label4);
            cmbLekarz = new ComboBox { Location = new Point(140, y), Size = new Size(250, 20), DropDownStyle = ComboBoxStyle.DropDownList };
            groupBox1.Controls.Add(cmbLekarz);
            y += 35;

            label5 = new Label { Text = "Data wystawienia:", Location = new Point(15, y), Size = new Size(120, 20) };
            groupBox1.Controls.Add(label5);
            dtpDataWystawienia = new DateTimePicker { Location = new Point(140, y), Size = new Size(250, 20), Format = DateTimePickerFormat.Short };
            groupBox1.Controls.Add(dtpDataWystawienia);
            y += 35;

            label6 = new Label { Text = "Data ważności:", Location = new Point(15, y), Size = new Size(120, 20) };
            groupBox1.Controls.Add(label6);
            dtpDataWaznosci = new DateTimePicker { Location = new Point(140, y), Size = new Size(250, 20), Format = DateTimePickerFormat.Short };
            groupBox1.Controls.Add(dtpDataWaznosci);
            y += 35;

            chkCzyZrealizowana = new CheckBox { Text = "Zrealizowana (automatyczne)", Location = new Point(140, y), Size = new Size(250, 20), Enabled = false };
            groupBox1.Controls.Add(chkCzyZrealizowana);
            y += 40;

            btnDodaj = new Button { Text = "Dodaj nowy", Location = new Point(15, y), Size = new Size(120, 30) };
            btnDodaj.Click += btnDodaj_Click;
            groupBox1.Controls.Add(btnDodaj);

            btnUsun = new Button { Text = "Usuń", Location = new Point(145, y), Size = new Size(120, 30) };
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

            btnOdswiez = new Button { Text = "Odśwież", Location = new Point(15, y), Size = new Size(120, 30) };
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

        private void ReceptaDataSetForm_Load(object sender, EventArgs e)
        {
            try { ZaladujDane(); }
            catch (Exception ex) { MessageBox.Show("Błąd: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void ZaladujDane()
        {
            dataSet = dataAdapter.PobierzDataSet();
            dataTable = dataSet.Tables["Recepty"];
            bindingSource = new BindingSource { DataSource = dataTable };
            dgvRecepty.DataSource = bindingSource;
            bindingNavigator1.BindingSource = bindingSource;

            cmbKlient.DataBindings.Clear();
            cmbLek.DataBindings.Clear();
            cmbLekarz.DataBindings.Clear();
            txtNumerRecepty.DataBindings.Clear();
            dtpDataWystawienia.DataBindings.Clear();
            dtpDataWaznosci.DataBindings.Clear();
            chkCzyZrealizowana.DataBindings.Clear();

            cmbKlient.DataSource = dataSet.Tables["Klienci"];
            cmbKlient.DisplayMember = "nazwa_pelna";
            cmbKlient.ValueMember = "id";
            cmbKlient.DataBindings.Add("SelectedValue", bindingSource, "id_klienta", true, DataSourceUpdateMode.OnPropertyChanged);

            cmbLek.DataSource = dataSet.Tables["Leki"];
            cmbLek.DisplayMember = "nazwa_handlowa";
            cmbLek.ValueMember = "id";
            cmbLek.DataBindings.Add("SelectedValue", bindingSource, "id_leku", true, DataSourceUpdateMode.OnPropertyChanged);

            cmbLekarz.DataSource = dataSet.Tables["Lekarze"];
            cmbLekarz.DisplayMember = "imie_nazwisko";
            cmbLekarz.ValueMember = "imie_nazwisko";
            cmbLekarz.DataBindings.Add("SelectedValue", bindingSource, "lekarz_imie_nazwisko", true, DataSourceUpdateMode.OnPropertyChanged);

            txtNumerRecepty.DataBindings.Add("Text", bindingSource, "numer_recepty", true, DataSourceUpdateMode.OnPropertyChanged);
            dtpDataWystawienia.DataBindings.Add("Value", bindingSource, "data_wystawienia", true, DataSourceUpdateMode.OnPropertyChanged);
            dtpDataWaznosci.DataBindings.Add("Value", bindingSource, "data_waznosci", true, DataSourceUpdateMode.OnPropertyChanged);
            chkCzyZrealizowana.DataBindings.Add("Checked", bindingSource, "czy_zrealizowana", true, DataSourceUpdateMode.OnPropertyChanged);

            FormatujKolumny();
            lblLiczbaRekordow.Text = $"Liczba rekordów: {dataTable.Rows.Count}";
        }

        private void FormatujKolumny()
        {
            if (dgvRecepty.Columns.Count > 0)
            {
                dgvRecepty.Columns["id"].HeaderText = "ID";
                dgvRecepty.Columns["id"].Width = 50;
                dgvRecepty.Columns["numer_recepty"].HeaderText = "Numer recepty";
                dgvRecepty.Columns["numer_recepty"].Width = 120;
                dgvRecepty.Columns["id_klienta"].Visible = false;
                dgvRecepty.Columns["id_leku"].Visible = false;
                dgvRecepty.Columns["klient_imie_nazwisko"].HeaderText = "Klient";
                dgvRecepty.Columns["klient_imie_nazwisko"].Width = 150;
                dgvRecepty.Columns["nazwa_leku"].HeaderText = "Lek";
                dgvRecepty.Columns["nazwa_leku"].Width = 150;
                dgvRecepty.Columns["lekarz_imie_nazwisko"].HeaderText = "Lekarz";
                dgvRecepty.Columns["lekarz_imie_nazwisko"].Width = 150;
                dgvRecepty.Columns["data_wystawienia"].HeaderText = "Data wystawienia";
                dgvRecepty.Columns["data_wystawienia"].Width = 110;
                dgvRecepty.Columns["data_wystawienia"].DefaultCellStyle.Format = "dd.MM.yyyy";
                dgvRecepty.Columns["data_waznosci"].HeaderText = "Data ważności";
                dgvRecepty.Columns["data_waznosci"].Width = 110;
                dgvRecepty.Columns["data_waznosci"].DefaultCellStyle.Format = "dd.MM.yyyy";
                dgvRecepty.Columns["czy_zrealizowana"].HeaderText = "Zrealizowana";
                dgvRecepty.Columns["czy_zrealizowana"].Width = 90;
            }
        }

        private void btnDodaj_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataSet.Tables["Klienci"].Rows.Count == 0) { MessageBox.Show("Brak klientów w bazie.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                if (dataSet.Tables["Leki"].Rows.Count == 0) { MessageBox.Show("Brak leków NA RECEPTĘ w bazie.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                if (dataSet.Tables["Lekarze"].Rows.Count == 0) { MessageBox.Show("Brak lekarzy w bazie. Dodaj pracownika na stanowisku 'lekarz'.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                DataRow newRow = dataTable.NewRow();
                newRow["numer_recepty"] = "R-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                newRow["id_klienta"] = dataSet.Tables["Klienci"].Rows[0]["id"];
                newRow["id_leku"] = dataSet.Tables["Leki"].Rows[0]["id"];
                newRow["lekarz_imie_nazwisko"] = dataSet.Tables["Lekarze"].Rows[0]["imie_nazwisko"];
                newRow["data_wystawienia"] = DateTime.Now;
                newRow["data_waznosci"] = DateTime.Now.AddDays(30);
                newRow["czy_zrealizowana"] = false;
                newRow["klient_imie_nazwisko"] = dataSet.Tables["Klienci"].Rows[0]["nazwa_pelna"];
                newRow["nazwa_leku"] = dataSet.Tables["Leki"].Rows[0]["nazwa_handlowa"];
                dataTable.Rows.Add(newRow);
                bindingSource.MoveLast();
                lblLiczbaRekordow.Text = $"Liczba rekordów: {dataTable.Rows.Count}";

                // Wymuś focus na pierwszym polu
                cmbKlient.Focus();
            }
            catch (Exception ex) { MessageBox.Show("Błąd: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnUsun_Click(object sender, EventArgs e)
        {
            try
            {
                if (bindingSource.Current == null) return;
                DataRowView currentRow = (DataRowView)bindingSource.Current;
                if (MessageBox.Show($"Usunąć receptę ID '{currentRow["id"]}'?", "Potwierdzenie", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
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
                DataTable changedTable = dataSet.Tables["Recepty"].GetChanges();
                if (changedTable != null)
                {
                    foreach (DataRow row in changedTable.Rows)
                    {
                        if (row.RowState != DataRowState.Deleted)
                        {
                            // Walidacja dat recepty
                            DateTime dataWystawienia = Convert.ToDateTime(row["data_wystawienia"]);
                            DateTime dataWaznosci = Convert.ToDateTime(row["data_waznosci"]);

                            if (dataWystawienia > DateTime.Now)
                            {
                                MessageBox.Show("Data wystawienia nie może być w przyszłości.", "Błąd walidacji", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            if (dataWaznosci < DateTime.Now.Date)
                            {
                                MessageBox.Show("Data ważności nie może być w przeszłości.", "Błąd walidacji", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            if (dataWaznosci <= dataWystawienia)
                            {
                                MessageBox.Show("Data ważności musi być późniejsza niż data wystawienia.", "Błąd walidacji", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

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

        private void ReceptaDataSetForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (dataSet != null && dataSet.HasChanges() && MessageBox.Show("Masz niezapisane zmiany. Zamknąć?", "Ostrzeżenie", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                e.Cancel = true;
        }

        private void dgvRecepty_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvRecepty.Rows[e.RowIndex].DataBoundItem == null) return;

            DataRowView rowView = dgvRecepty.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (rowView == null) return;

            bool czyZrealizowana = Convert.ToBoolean(rowView["czy_zrealizowana"]);
            DateTime dataWaznosci = Convert.ToDateTime(rowView["data_waznosci"]);
            bool przeterminowana = dataWaznosci < DateTime.Now;

            if (czyZrealizowana)
            {
                // Zrealizowane - kolor zielony
                dgvRecepty.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                dgvRecepty.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
            }
            else if (przeterminowana)
            {
                // Przeterminowane - kolor czerwony
                dgvRecepty.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightCoral;
                dgvRecepty.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
            }
            else
            {
                // Aktywne - normalny kolor
                dgvRecepty.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                dgvRecepty.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
            }
        }
    }
}
