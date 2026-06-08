using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using FarmacySystem.Repositories;

namespace FarmacySystem
{
    public partial class StanMagazynowyDataSetForm : Form
    {
        private StanMagazynowyDataAdapter dataAdapter;
        private DataSet dataSet;
        private DataTable dataTable;
        private BindingSource bindingSource;

        private BindingNavigator bindingNavigator1;
        private DataGridView dgvStany;
        private GroupBox groupBox1;
        private Label label1, label2, label3, label4, label5, lblLiczbaRekordow;
        private ComboBox cmbLokalizacja, cmbLek, cmbStatus;
        private TextBox txtIlosc;
        private DateTimePicker dtpDataWaznosci;
        private Button btnDodaj, btnUsun, btnZapisz, btnAnuluj, btnOdswiez, btnZamknij;
        private System.ComponentModel.IContainer components = null;

        public StanMagazynowyDataSetForm()
        {
            InitializeComponent();
            dataAdapter = new StanMagazynowyDataAdapter();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.Text = "Zarządzanie Stanami Magazynowymi - DataSet";
            this.Size = new Size(1250, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Load += StanMagazynowyDataSetForm_Load;
            this.FormClosing += StanMagazynowyDataSetForm_FormClosing;

            bindingNavigator1 = new BindingNavigator(this.components);
            bindingNavigator1.AddStandardItems();
            bindingNavigator1.Dock = DockStyle.Top;
            this.Controls.Add(bindingNavigator1);

            dgvStany = new DataGridView();
            dgvStany.Location = new Point(12, 35);
            dgvStany.Size = new Size(800, 600);
            dgvStany.ReadOnly = true;
            dgvStany.AllowUserToAddRows = false;
            dgvStany.AllowUserToDeleteRows = false;
            dgvStany.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvStany.CellFormatting += dgvStany_CellFormatting;
            this.Controls.Add(dgvStany);

            groupBox1 = new GroupBox();
            groupBox1.Text = "Szczegóły stanu magazynowego";
            groupBox1.Location = new Point(820, 35);
            groupBox1.Size = new Size(410, 520);
            this.Controls.Add(groupBox1);

            int y = 30;
            label1 = new Label { Text = "Lokalizacja:", Location = new Point(15, y), Size = new Size(140, 20) };
            groupBox1.Controls.Add(label1);
            cmbLokalizacja = new ComboBox { Location = new Point(160, y), Size = new Size(230, 20), DropDownStyle = ComboBoxStyle.DropDownList };
            groupBox1.Controls.Add(cmbLokalizacja);
            y += 35;

            label2 = new Label { Text = "Lek:", Location = new Point(15, y), Size = new Size(140, 20) };
            groupBox1.Controls.Add(label2);
            cmbLek = new ComboBox { Location = new Point(160, y), Size = new Size(230, 20), DropDownStyle = ComboBoxStyle.DropDownList };
            groupBox1.Controls.Add(cmbLek);
            y += 35;

            label3 = new Label { Text = "Ilość:", Location = new Point(15, y), Size = new Size(140, 20) };
            groupBox1.Controls.Add(label3);
            txtIlosc = new TextBox { Location = new Point(160, y), Size = new Size(230, 20) };
            groupBox1.Controls.Add(txtIlosc);
            y += 35;

            label4 = new Label { Text = "Status:", Location = new Point(15, y), Size = new Size(140, 20) };
            groupBox1.Controls.Add(label4);
            cmbStatus = new ComboBox { Location = new Point(160, y), Size = new Size(230, 20), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbStatus.Items.AddRange(new object[] { "dostępny", "zarezerwowany", "niedostępny" });
            groupBox1.Controls.Add(cmbStatus);
            y += 35;

            label5 = new Label { Text = "Data ważności:", Location = new Point(15, y), Size = new Size(140, 20) };
            groupBox1.Controls.Add(label5);
            dtpDataWaznosci = new DateTimePicker { Location = new Point(160, y), Size = new Size(230, 20), Format = DateTimePickerFormat.Short };
            groupBox1.Controls.Add(dtpDataWaznosci);
            y += 55;

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
            lblLiczbaRekordow.Location = new Point(820, 565);
            lblLiczbaRekordow.Size = new Size(200, 20);
            lblLiczbaRekordow.Font = new Font(lblLiczbaRekordow.Font, FontStyle.Bold);
            this.Controls.Add(lblLiczbaRekordow);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void StanMagazynowyDataSetForm_Load(object sender, EventArgs e)
        {
            try { ZaladujDane(); }
            catch (Exception ex) { MessageBox.Show("Błąd: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void ZaladujDane()
        {
            dataSet = dataAdapter.PobierzDataSet();
            dataTable = dataSet.Tables["StanyMagazynowe"];
            bindingSource = new BindingSource { DataSource = dataTable };
            dgvStany.DataSource = bindingSource;
            bindingNavigator1.BindingSource = bindingSource;

            cmbLokalizacja.DataBindings.Clear();
            cmbLek.DataBindings.Clear();
            txtIlosc.DataBindings.Clear();
            cmbStatus.DataBindings.Clear();
            dtpDataWaznosci.DataBindings.Clear();

            cmbLokalizacja.DataSource = dataSet.Tables["Lokalizacje"];
            cmbLokalizacja.DisplayMember = "nazwa";
            cmbLokalizacja.ValueMember = "id";
            cmbLokalizacja.DataBindings.Add("SelectedValue", bindingSource, "id_lokalizacji", true, DataSourceUpdateMode.OnPropertyChanged);

            cmbLek.DataSource = dataSet.Tables["Leki"];
            cmbLek.DisplayMember = "nazwa_handlowa";
            cmbLek.ValueMember = "id";
            cmbLek.DataBindings.Add("SelectedValue", bindingSource, "id_leku", true, DataSourceUpdateMode.OnPropertyChanged);

            txtIlosc.DataBindings.Add("Text", bindingSource, "ilosc", true, DataSourceUpdateMode.OnPropertyChanged);
            cmbStatus.DataBindings.Add("Text", bindingSource, "status", true, DataSourceUpdateMode.OnPropertyChanged);
            dtpDataWaznosci.DataBindings.Add("Value", bindingSource, "data_waznosci", true, DataSourceUpdateMode.OnPropertyChanged);

            FormatujKolumny();
            lblLiczbaRekordow.Text = $"Liczba rekordów: {dataTable.Rows.Count}";
        }

        private void FormatujKolumny()
        {
            if (dgvStany.Columns.Count > 0)
            {
                dgvStany.Columns["id"].HeaderText = "ID";
                dgvStany.Columns["id"].Width = 50;
                dgvStany.Columns["id_lokalizacji"].Visible = false;
                dgvStany.Columns["id_leku"].Visible = false;
                dgvStany.Columns["nazwa_lokalizacji"].HeaderText = "Lokalizacja";
                dgvStany.Columns["nazwa_lokalizacji"].Width = 150;
                dgvStany.Columns["nazwa_leku"].HeaderText = "Lek";
                dgvStany.Columns["nazwa_leku"].Width = 200;
                dgvStany.Columns["ilosc"].HeaderText = "Ilość";
                dgvStany.Columns["ilosc"].Width = 70;
                dgvStany.Columns["status"].HeaderText = "Status";
                dgvStany.Columns["status"].Width = 100;
                dgvStany.Columns["data_waznosci"].HeaderText = "Data ważności";
                dgvStany.Columns["data_waznosci"].Width = 120;
                dgvStany.Columns["data_waznosci"].DefaultCellStyle.Format = "dd.MM.yyyy";
            }
        }

        private void dgvStany_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                // Sprawdź czy to kolumna ilości
                if (dgvStany.Columns[e.ColumnIndex].Name == "ilosc" && e.RowIndex >= 0)
                {
                    DataGridViewRow row = dgvStany.Rows[e.RowIndex];
                    if (row.DataBoundItem != null)
                    {
                        DataRowView rowView = row.DataBoundItem as DataRowView;
                        if (rowView != null && rowView["ilosc"] != DBNull.Value)
                        {
                            int ilosc = Convert.ToInt32(rowView["ilosc"]);
                            if (ilosc < 10)
                            {
                                // Cały wiersz na czerwono
                                row.DefaultCellStyle.BackColor = Color.LightCoral;
                                row.DefaultCellStyle.ForeColor = Color.DarkRed;
                                row.DefaultCellStyle.Font = new Font(dgvStany.Font, FontStyle.Bold);
                            }
                            else
                            {
                                // Przywróć domyślne kolory
                                row.DefaultCellStyle.BackColor = dgvStany.DefaultCellStyle.BackColor;
                                row.DefaultCellStyle.ForeColor = dgvStany.DefaultCellStyle.ForeColor;
                                row.DefaultCellStyle.Font = dgvStany.Font;
                            }
                        }
                    }
                }
            }
            catch { /* Ignoruj błędy formatowania */ }
        }

        private void btnDodaj_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataSet.Tables["Lokalizacje"].Rows.Count == 0) { MessageBox.Show("Brak lokalizacji w bazie.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                if (dataSet.Tables["Leki"].Rows.Count == 0) { MessageBox.Show("Brak leków w bazie.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                DataRow newRow = dataTable.NewRow();
                newRow["id_lokalizacji"] = dataSet.Tables["Lokalizacje"].Rows[0]["id"];
                newRow["id_leku"] = dataSet.Tables["Leki"].Rows[0]["id"];
                newRow["ilosc"] = 0;
                newRow["status"] = "dostępny";
                newRow["data_waznosci"] = DateTime.Now.AddMonths(12);
                newRow["nazwa_lokalizacji"] = dataSet.Tables["Lokalizacje"].Rows[0]["nazwa"];
                newRow["nazwa_leku"] = dataSet.Tables["Leki"].Rows[0]["nazwa_handlowa"];
                dataTable.Rows.Add(newRow);
                bindingSource.MoveLast();
                lblLiczbaRekordow.Text = $"Liczba rekordów: {dataTable.Rows.Count}";

                // Wymuś focus na pierwsze pole edytowalne
                txtIlosc.Focus();
                txtIlosc.SelectAll();
            }
            catch (Exception ex) { MessageBox.Show("Błąd: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnUsun_Click(object sender, EventArgs e)
        {
            try
            {
                if (bindingSource.Current == null) return;
                DataRowView currentRow = (DataRowView)bindingSource.Current;
                if (MessageBox.Show($"Usunąć stan magazynowy ID '{currentRow["id"]}'?", "Potwierdzenie", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
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
                DataTable changedTable = dataSet.Tables["StanyMagazynowe"].GetChanges();
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

        private void StanMagazynowyDataSetForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (dataSet != null && dataSet.HasChanges() && MessageBox.Show("Masz niezapisane zmiany. Zamknąć?", "Ostrzeżenie", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                e.Cancel = true;
        }
    }
}
