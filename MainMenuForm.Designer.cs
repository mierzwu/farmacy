namespace FarmacySystem
{
    partial class MainMenuForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnLokalizacje;
        private System.Windows.Forms.Button btnPracownicy;
        private System.Windows.Forms.Button btnKlienci;
        private System.Windows.Forms.Button btnLeki;
        private System.Windows.Forms.Button btnRecepty;
        private System.Windows.Forms.Button btnMagazyn;
        private System.Windows.Forms.Button btnSprzedaz;
        private System.Windows.Forms.Button btnWyjscie;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.GroupBox groupBox1;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnLokalizacje = new System.Windows.Forms.Button();
            this.btnPracownicy = new System.Windows.Forms.Button();
            this.btnKlienci = new System.Windows.Forms.Button();
            this.btnLeki = new System.Windows.Forms.Button();
            this.btnRecepty = new System.Windows.Forms.Button();
            this.btnMagazyn = new System.Windows.Forms.Button();
            this.btnSprzedaz = new System.Windows.Forms.Button();
            this.btnWyjscie = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblTitle.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblTitle.Location = new System.Drawing.Point(140, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(320, 29);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "System Zarządzania Apteką";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnLokalizacje);
            this.groupBox1.Controls.Add(this.btnPracownicy);
            this.groupBox1.Controls.Add(this.btnKlienci);
            this.groupBox1.Controls.Add(this.btnLeki);
            this.groupBox1.Controls.Add(this.btnRecepty);
            this.groupBox1.Controls.Add(this.btnMagazyn);
            this.groupBox1.Controls.Add(this.btnSprzedaz);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.groupBox1.Location = new System.Drawing.Point(30, 70);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(540, 360);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Moduły systemu";
            // 
            // btnLokalizacje
            // 
            this.btnLokalizacje.BackColor = System.Drawing.Color.LightBlue;
            this.btnLokalizacje.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnLokalizacje.Location = new System.Drawing.Point(30, 35);
            this.btnLokalizacje.Name = "btnLokalizacje";
            this.btnLokalizacje.Size = new System.Drawing.Size(230, 60);
            this.btnLokalizacje.TabIndex = 0;
            this.btnLokalizacje.Text = "Lokalizacje (Apteki)";
            this.btnLokalizacje.UseVisualStyleBackColor = false;
            this.btnLokalizacje.Click += new System.EventHandler(this.btnLokalizacje_Click);
            // 
            // btnPracownicy
            // 
            this.btnPracownicy.BackColor = System.Drawing.Color.LightGreen;
            this.btnPracownicy.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnPracownicy.Location = new System.Drawing.Point(280, 35);
            this.btnPracownicy.Name = "btnPracownicy";
            this.btnPracownicy.Size = new System.Drawing.Size(230, 60);
            this.btnPracownicy.TabIndex = 1;
            this.btnPracownicy.Text = "Pracownicy";
            this.btnPracownicy.UseVisualStyleBackColor = false;
            this.btnPracownicy.Click += new System.EventHandler(this.btnPracownicy_Click);
            // 
            // btnKlienci
            // 
            this.btnKlienci.BackColor = System.Drawing.Color.LightYellow;
            this.btnKlienci.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnKlienci.Location = new System.Drawing.Point(30, 110);
            this.btnKlienci.Name = "btnKlienci";
            this.btnKlienci.Size = new System.Drawing.Size(230, 60);
            this.btnKlienci.TabIndex = 2;
            this.btnKlienci.Text = "Klienci";
            this.btnKlienci.UseVisualStyleBackColor = false;
            this.btnKlienci.Click += new System.EventHandler(this.btnKlienci_Click);
            // 
            // btnLeki
            // 
            this.btnLeki.BackColor = System.Drawing.Color.LightCoral;
            this.btnLeki.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnLeki.Location = new System.Drawing.Point(280, 110);
            this.btnLeki.Name = "btnLeki";
            this.btnLeki.Size = new System.Drawing.Size(230, 60);
            this.btnLeki.TabIndex = 3;
            this.btnLeki.Text = "Leki";
            this.btnLeki.UseVisualStyleBackColor = false;
            this.btnLeki.Click += new System.EventHandler(this.btnLeki_Click);
            // 
            // btnRecepty
            // 
            this.btnRecepty.BackColor = System.Drawing.Color.LightSalmon;
            this.btnRecepty.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnRecepty.Location = new System.Drawing.Point(30, 185);
            this.btnRecepty.Name = "btnRecepty";
            this.btnRecepty.Size = new System.Drawing.Size(230, 60);
            this.btnRecepty.TabIndex = 4;
            this.btnRecepty.Text = "Recepty";
            this.btnRecepty.UseVisualStyleBackColor = false;
            this.btnRecepty.Click += new System.EventHandler(this.btnRecepty_Click);
            // 
            // btnMagazyn
            // 
            this.btnMagazyn.BackColor = System.Drawing.Color.PaleGreen;
            this.btnMagazyn.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnMagazyn.Location = new System.Drawing.Point(280, 185);
            this.btnMagazyn.Name = "btnMagazyn";
            this.btnMagazyn.Size = new System.Drawing.Size(230, 60);
            this.btnMagazyn.TabIndex = 5;
            this.btnMagazyn.Text = "Stany Magazynowe";
            this.btnMagazyn.UseVisualStyleBackColor = false;
            this.btnMagazyn.Click += new System.EventHandler(this.btnMagazyn_Click);
            // 
            // btnSprzedaz
            // 
            this.btnSprzedaz.BackColor = System.Drawing.Color.Gold;
            this.btnSprzedaz.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnSprzedaz.Location = new System.Drawing.Point(30, 260);
            this.btnSprzedaz.Name = "btnSprzedaz";
            this.btnSprzedaz.Size = new System.Drawing.Size(480, 60);
            this.btnSprzedaz.TabIndex = 6;
            this.btnSprzedaz.Text = "SPRZEDAŻ LEKÓW";
            this.btnSprzedaz.UseVisualStyleBackColor = false;
            this.btnSprzedaz.Click += new System.EventHandler(this.btnSprzedaz_Click);
            // 
            // btnWyjscie
            // 
            this.btnWyjscie.BackColor = System.Drawing.Color.LightGray;
            this.btnWyjscie.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnWyjscie.Location = new System.Drawing.Point(230, 450);
            this.btnWyjscie.Name = "btnWyjscie";
            this.btnWyjscie.Size = new System.Drawing.Size(140, 40);
            this.btnWyjscie.TabIndex = 7;
            this.btnWyjscie.Text = "Wyjście";
            this.btnWyjscie.UseVisualStyleBackColor = false;
            this.btnWyjscie.Click += new System.EventHandler(this.btnWyjscie_Click);
            // 
            // MainMenuForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(600, 510);
            this.Controls.Add(this.btnWyjscie);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainMenuForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "System Apteczny - Menu Główne";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
