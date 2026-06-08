using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using FarmacySystem.Repositories;

namespace FarmacySystem
{
    public partial class SprzedazForm : Form
    {
        private SprzedazDataAdapter dataAdapter;
        private DataSet dataSet;

        private ComboBox cmbLokalizacja, cmbKlient, cmbLek;
        private TextBox txtIlosc, txtCenaJednostkowa, txtRazem, txtStanMagazynowy;
        private CheckBox chkCzyNaRecepte;
        private Label lblReceptaStatus;
        private Button btnDodaj, btnZrealizuj, btnAnuluj, btnZamknij;
        private DataGridView dgvKoszyk;
        private Label lblSumaCalkowita;

        private DataTable dtKoszyk;
        private System.ComponentModel.IContainer components = null;

        public SprzedazForm()
        {
            InitializeComponent();
            dataAdapter = new SprzedazDataAdapter();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.Text = "Sprzedaż leków";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Load += SprzedazForm_Load;

            // Panel wyboru
            GroupBox groupBox1 = new GroupBox();
            groupBox1.Text = "Dane sprzedaży";
            groupBox1.Location = new Point(15, 15);
            groupBox1.Size = new Size(950, 220);
            this.Controls.Add(groupBox1);

            int y = 30;

            // Lokalizacja
            Label lblLokalizacja = new Label { Text = "Lokalizacja:", Location = new Point(15, y), Size = new Size(120, 20) };
            groupBox1.Controls.Add(lblLokalizacja);
            cmbLokalizacja = new ComboBox { Location = new Point(140, y), Size = new Size(300, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbLokalizacja.SelectedIndexChanged += cmbLokalizacja_SelectedIndexChanged;
            groupBox1.Controls.Add(cmbLokalizacja);
            y += 35;

            // Klient
            Label lblKlient = new Label { Text = "Klient:", Location = new Point(15, y), Size = new Size(120, 20) };
            groupBox1.Controls.Add(lblKlient);
            cmbKlient = new ComboBox { Location = new Point(140, y), Size = new Size(300, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbKlient.SelectedIndexChanged += cmbKlient_SelectedIndexChanged;
            groupBox1.Controls.Add(cmbKlient);
            y += 35;

            // Lek
            Label lblLek = new Label { Text = "Lek:", Location = new Point(15, y), Size = new Size(120, 20) };
            groupBox1.Controls.Add(lblLek);
            cmbLek = new ComboBox { Location = new Point(140, y), Size = new Size(300, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbLek.SelectedIndexChanged += cmbLek_SelectedIndexChanged;
            groupBox1.Controls.Add(cmbLek);

            chkCzyNaRecepte = new CheckBox { Text = "Wymaga recepty", Location = new Point(450, y), Size = new Size(150, 25), Enabled = false };
            groupBox1.Controls.Add(chkCzyNaRecepte);
            y += 35;

            // Stan magazynowy
            Label lblStan = new Label { Text = "Stan magazynowy:", Location = new Point(15, y), Size = new Size(120, 20) };
            groupBox1.Controls.Add(lblStan);
            txtStanMagazynowy = new TextBox { Location = new Point(140, y), Size = new Size(100, 25), ReadOnly = true, BackColor = Color.LightGray };
            groupBox1.Controls.Add(txtStanMagazynowy);

            Label lblIlosc = new Label { Text = "Ilość:", Location = new Point(260, y), Size = new Size(50, 20) };
            groupBox1.Controls.Add(lblIlosc);
            txtIlosc = new TextBox { Location = new Point(315, y), Size = new Size(80, 25) };
            txtIlosc.TextChanged += txtIlosc_TextChanged;
            groupBox1.Controls.Add(txtIlosc);
            y += 35;

            // Cena
            Label lblCena = new Label { Text = "Cena jedn.:", Location = new Point(15, y), Size = new Size(120, 20) };
            groupBox1.Controls.Add(lblCena);
            txtCenaJednostkowa = new TextBox { Location = new Point(140, y), Size = new Size(100, 25), ReadOnly = true, BackColor = Color.LightGray };
            groupBox1.Controls.Add(txtCenaJednostkowa);

            Label lblRazem = new Label { Text = "Razem:", Location = new Point(260, y), Size = new Size(50, 20) };
            groupBox1.Controls.Add(lblRazem);
            txtRazem = new TextBox { Location = new Point(315, y), Size = new Size(125, 25), ReadOnly = true, BackColor = Color.LightYellow, Font = new Font(this.Font, FontStyle.Bold) };
            groupBox1.Controls.Add(txtRazem);

            btnDodaj = new Button { Text = "Dodaj do koszyka", Location = new Point(460, y), Size = new Size(150, 30) };
            btnDodaj.Click += btnDodaj_Click;
            groupBox1.Controls.Add(btnDodaj);

            // Status recepty - poza GroupBoxem, aby był zawsze widoczny
            lblReceptaStatus = new Label { 
                Location = new Point(15, 240), 
                Size = new Size(950, 35), 
                ForeColor = Color.White, 
                Font = new Font(this.Font.FontFamily, 11, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent,
                Padding = new Padding(5, 0, 0, 0)
            };
            this.Controls.Add(lblReceptaStatus);

            // Koszyk
            Label lblKoszyk = new Label { Text = "Koszyk zakupów:", Location = new Point(15, 280), Size = new Size(150, 20), Font = new Font(this.Font, FontStyle.Bold) };
            this.Controls.Add(lblKoszyk);

            dgvKoszyk = new DataGridView();
            dgvKoszyk.Location = new Point(15, 305);
            dgvKoszyk.Size = new Size(950, 270);
            dgvKoszyk.ReadOnly = true;
            dgvKoszyk.AllowUserToAddRows = false;
            dgvKoszyk.AllowUserToDeleteRows = false;
            dgvKoszyk.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.Controls.Add(dgvKoszyk);

            // Suma całkowita
            lblSumaCalkowita = new Label { Location = new Point(700, 585), Size = new Size(265, 25), Font = new Font(this.Font.FontFamily, 14, FontStyle.Bold), TextAlign = ContentAlignment.MiddleRight };
            this.Controls.Add(lblSumaCalkowita);

            // Przyciski
            btnZrealizuj = new Button { Text = "Zrealizuj sprzedaż", Location = new Point(15, 620), Size = new Size(150, 35), BackColor = Color.LightGreen };
            btnZrealizuj.Click += btnZrealizuj_Click;
            this.Controls.Add(btnZrealizuj);

            btnAnuluj = new Button { Text = "Anuluj (wyczyść)", Location = new Point(175, 620), Size = new Size(150, 35) };
            btnAnuluj.Click += btnAnuluj_Click;
            this.Controls.Add(btnAnuluj);

            btnZamknij = new Button { Text = "Zamknij", Location = new Point(815, 620), Size = new Size(150, 35) };
            btnZamknij.Click += btnZamknij_Click;
            this.Controls.Add(btnZamknij);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void SprzedazForm_Load(object sender, EventArgs e)
        {
            try
            {
                ZaladujDane();
                InicjujKoszyk();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ZaladujDane()
        {
            // Wyłącz eventy podczas ładowania
            cmbLokalizacja.SelectedIndexChanged -= cmbLokalizacja_SelectedIndexChanged;
            cmbKlient.SelectedIndexChanged -= cmbKlient_SelectedIndexChanged;
            cmbLek.SelectedIndexChanged -= cmbLek_SelectedIndexChanged;

            dataSet = dataAdapter.PobierzDaneDoSprzedazy();

            cmbLokalizacja.DataSource = dataSet.Tables["Lokalizacje"];
            cmbLokalizacja.DisplayMember = "nazwa";
            cmbLokalizacja.ValueMember = "id";

            cmbKlient.DataSource = dataSet.Tables["Klienci"];
            cmbKlient.DisplayMember = "nazwa_pelna";
            cmbKlient.ValueMember = "id";

            cmbLek.DataSource = dataSet.Tables["Leki"];
            cmbLek.DisplayMember = "nazwa_handlowa";
            cmbLek.ValueMember = "id";

            // Włącz eventy ponownie
            cmbLokalizacja.SelectedIndexChanged += cmbLokalizacja_SelectedIndexChanged;
            cmbKlient.SelectedIndexChanged += cmbKlient_SelectedIndexChanged;
            cmbLek.SelectedIndexChanged += cmbLek_SelectedIndexChanged;
        }

        private void InicjujKoszyk()
        {
            dtKoszyk = new DataTable();
            dtKoszyk.Columns.Add("IdLeku", typeof(int));
            dtKoszyk.Columns.Add("Lek", typeof(string));
            dtKoszyk.Columns.Add("Ilosc", typeof(int));
            dtKoszyk.Columns.Add("CenaJednostkowa", typeof(decimal));
            dtKoszyk.Columns.Add("Razem", typeof(decimal));
            dtKoszyk.Columns.Add("CzyNaRecepte", typeof(bool));

            dgvKoszyk.DataSource = dtKoszyk;

            if (dgvKoszyk.Columns.Count > 0)
            {
                dgvKoszyk.Columns["IdLeku"].Visible = false;
                dgvKoszyk.Columns["Lek"].HeaderText = "Nazwa leku";
                dgvKoszyk.Columns["Lek"].Width = 300;
                dgvKoszyk.Columns["Ilosc"].HeaderText = "Ilość";
                dgvKoszyk.Columns["Ilosc"].Width = 80;
                dgvKoszyk.Columns["CenaJednostkowa"].HeaderText = "Cena jedn.";
                dgvKoszyk.Columns["CenaJednostkowa"].Width = 100;
                dgvKoszyk.Columns["CenaJednostkowa"].DefaultCellStyle.Format = "C2";
                dgvKoszyk.Columns["Razem"].HeaderText = "Razem";
                dgvKoszyk.Columns["Razem"].Width = 120;
                dgvKoszyk.Columns["Razem"].DefaultCellStyle.Format = "C2";
                dgvKoszyk.Columns["CzyNaRecepte"].HeaderText = "Na receptę";
                dgvKoszyk.Columns["CzyNaRecepte"].Width = 100;
            }

            AktualizujSumeCalkowita();
        }

        private void cmbLokalizacja_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbLokalizacja.SelectedValue != null)
            {
                AktualizujStanMagazynowy();
            }
        }

        private void cmbKlient_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbKlient.SelectedValue != null)
            {
                SprawdzRecepte();
            }
        }

        private void cmbLek_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbLek.SelectedValue == null) return;

            DataRowView row = (DataRowView)cmbLek.SelectedItem;
            bool czyNaRecepte = Convert.ToBoolean(row["czy_na_recepte"]);
            decimal cena = Convert.ToDecimal(row["cena_brutto"]);

            chkCzyNaRecepte.Checked = czyNaRecepte;
            txtCenaJednostkowa.Text = cena.ToString("0.00");

            AktualizujStanMagazynowy();
            SprawdzRecepte();
            ObliczRazem();
        }

        private void txtIlosc_TextChanged(object sender, EventArgs e)
        {
            ObliczRazem();
        }

        private void AktualizujStanMagazynowy()
        {
            if (cmbLokalizacja.SelectedValue == null || cmbLek.SelectedValue == null)
            {
                txtStanMagazynowy.Text = "";
                return;
            }

            try
            {
                int idLokalizacji = Convert.ToInt32(cmbLokalizacja.SelectedValue);
                int idLeku = Convert.ToInt32(cmbLek.SelectedValue);

                int stan = dataAdapter.PobierzStanMagazynowy(idLokalizacji, idLeku);
                txtStanMagazynowy.Text = stan.ToString();
                txtStanMagazynowy.BackColor = stan < 10 ? Color.LightCoral : Color.LightGreen;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SprawdzRecepte()
        {
            lblReceptaStatus.Text = "";
            lblReceptaStatus.BackColor = Color.Transparent;

            if (!chkCzyNaRecepte.Checked)
            {
                lblReceptaStatus.Text = "";
                return;
            }

            if (cmbKlient.SelectedValue == null || cmbLek.SelectedValue == null) return;

            try
            {
                int idKlienta = Convert.ToInt32(cmbKlient.SelectedValue);
                int idLeku = Convert.ToInt32(cmbLek.SelectedValue);

                bool maRecepte = dataAdapter.KlientPosiadaRecepteNaLek(idKlienta, idLeku);

                if (maRecepte)
                {
                    lblReceptaStatus.Text = "✓ Klient posiada ważną receptę na ten lek.";
                    lblReceptaStatus.ForeColor = Color.White;
                    lblReceptaStatus.BackColor = Color.Green;
                }
                else
                {
                    lblReceptaStatus.Text = "⚠ UWAGA: Klient NIE posiada ważnej recepty na ten lek! Nie można dodać do koszyka.";
                    lblReceptaStatus.ForeColor = Color.White;
                    lblReceptaStatus.BackColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ObliczRazem()
        {
            if (string.IsNullOrWhiteSpace(txtCenaJednostkowa.Text) || string.IsNullOrWhiteSpace(txtIlosc.Text))
            {
                txtRazem.Text = "";
                return;
            }

            try
            {
                decimal cena = decimal.Parse(txtCenaJednostkowa.Text);
                int ilosc = int.Parse(txtIlosc.Text);
                decimal razem = cena * ilosc;
                txtRazem.Text = razem.ToString("0.00") + " zł";
            }
            catch
            {
                txtRazem.Text = "";
            }
        }

        private void btnDodaj_Click(object sender, EventArgs e)
        {
            try
            {
                // Walidacja
                if (cmbLokalizacja.SelectedValue == null)
                {
                    MessageBox.Show("Wybierz lokalizację.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cmbKlient.SelectedValue == null)
                {
                    MessageBox.Show("Wybierz klienta.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cmbLek.SelectedValue == null)
                {
                    MessageBox.Show("Wybierz lek.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtIlosc.Text) || !int.TryParse(txtIlosc.Text, out int ilosc) || ilosc <= 0)
                {
                    MessageBox.Show("Podaj prawidłową ilość (liczba > 0).", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int stan = int.Parse(txtStanMagazynowy.Text);
                if (ilosc > stan)
                {
                    MessageBox.Show($"Niewystarczająca ilość w magazynie. Dostępne: {stan}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int idKlienta = Convert.ToInt32(cmbKlient.SelectedValue);
                int idLeku = Convert.ToInt32(cmbLek.SelectedValue);

                // Sprawdź receptę
                if (chkCzyNaRecepte.Checked)
                {
                    if (!dataAdapter.KlientPosiadaRecepteNaLek(idKlienta, idLeku))
                    {
                        MessageBox.Show(
                            "Nie można dodać do koszyka!\n\nKlient nie posiada ważnej recepty na ten lek wymagający recepty.",
                            "Brak recepty",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }
                }

                // Dodaj do koszyka
                DataRowView lekRow = (DataRowView)cmbLek.SelectedItem;
                string nazwaLeku = lekRow["nazwa_handlowa"].ToString();
                decimal cena = Convert.ToDecimal(lekRow["cena_brutto"]);
                decimal razem = cena * ilosc;

                DataRow newRow = dtKoszyk.NewRow();
                newRow["IdLeku"] = idLeku;
                newRow["Lek"] = nazwaLeku;
                newRow["Ilosc"] = ilosc;
                newRow["CenaJednostkowa"] = cena;
                newRow["Razem"] = razem;
                newRow["CzyNaRecepte"] = chkCzyNaRecepte.Checked;
                dtKoszyk.Rows.Add(newRow);

                AktualizujSumeCalkowita();
                WyczyscPola();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnZrealizuj_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtKoszyk.Rows.Count == 0)
                {
                    MessageBox.Show("Koszyk jest pusty.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cmbKlient.SelectedValue == null || cmbLokalizacja.SelectedValue == null)
                {
                    MessageBox.Show("Wybierz klienta i lokalizację.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int idKlienta = Convert.ToInt32(cmbKlient.SelectedValue);
                int idLokalizacji = Convert.ToInt32(cmbLokalizacja.SelectedValue);

                // Sprawdź czy wszystkie leki na receptę w koszyku mają recepty
                foreach (DataRow row in dtKoszyk.Rows)
                {
                    bool czyNaRecepte = Convert.ToBoolean(row["CzyNaRecepte"]);
                    if (czyNaRecepte)
                    {
                        int idLeku = Convert.ToInt32(row["IdLeku"]);
                        if (!dataAdapter.KlientPosiadaRecepteNaLek(idKlienta, idLeku))
                        {
                            string nazwaLeku = row["Lek"].ToString();
                            MessageBox.Show(
                                $"Nie można zrealizować sprzedaży!\n\nKlient nie posiada ważnej recepty na lek:\n{nazwaLeku}\n\nUsuń ten lek z koszyka lub wybierz innego klienta.",
                                "Brak recepty",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            return;
                        }
                    }
                }

                decimal sumaCalkowita = 0;
                foreach (DataRow row in dtKoszyk.Rows)
                {
                    sumaCalkowita += Convert.ToDecimal(row["Razem"]);
                }

                var result = MessageBox.Show(
                    $"Czy zrealizować sprzedaż na kwotę {sumaCalkowita:C2}?",
                    "Potwierdzenie",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    foreach (DataRow row in dtKoszyk.Rows)
                    {
                        int idLeku = Convert.ToInt32(row["IdLeku"]);
                        int ilosc = Convert.ToInt32(row["Ilosc"]);
                        decimal kwota = Convert.ToDecimal(row["Razem"]);

                        int idRecepty;
                        dataAdapter.ZrealizujSprzedaz(idKlienta, idLeku, idLokalizacji, ilosc, kwota, out idRecepty);
                    }

                    MessageBox.Show($"Sprzedaż zrealizowana pomyślnie!\nKwota: {sumaCalkowita:C2}", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dtKoszyk.Clear();
                    AktualizujSumeCalkowita();
                    WyczyscPola();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAnuluj_Click(object sender, EventArgs e)
        {
            dtKoszyk.Clear();
            AktualizujSumeCalkowita();
            WyczyscPola();
        }

        private void btnZamknij_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void WyczyscPola()
        {
            txtIlosc.Clear();
            txtRazem.Clear();
            lblReceptaStatus.Text = "";
        }

        private void AktualizujSumeCalkowita()
        {
            decimal suma = 0;
            foreach (DataRow row in dtKoszyk.Rows)
            {
                suma += Convert.ToDecimal(row["Razem"]);
            }
            lblSumaCalkowita.Text = $"SUMA: {suma:C2}";
        }
    }
}
