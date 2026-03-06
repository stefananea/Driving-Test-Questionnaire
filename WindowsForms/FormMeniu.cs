// =============================================================================
// Fisier: FormMeniu.cs
// Autor: David
// Descriere: Formular principal al aplicatiei - afiseaza meniul dupa autentificare
// Functionalitate: Acces la legislatie, testare, progres, gestionare intrebari/utilizatori
// Data: 2025-05-25
// =============================================================================

using System;
using System.Drawing;
using System.Windows.Forms;

using ChestionarAuto.UI;
using ChestionarAuto.Teste;
using ChestionarAuto.Legislatie;
using ChestionarAuto.Login;
using ChestionarAuto.Utilizator;
using System.IO;

namespace ChestionarAutoApp
{
    public class FormMeniu : Form
    {
        private string username;
        private Label labelBunVenit;
        private Button buttonLegislatie;
        private Button buttonSimulare;
        private Button buttonProgres;
        private Button buttonUtilizatori;
        private Button buttonIntrebari;
        private Button buttonIesire;
        private Button buttonDelogare;

        public FormMeniu(string user)
        {
            username = user;
            InitializeComponent();
        }

        // Initializare elemente grafice si logica pentru butoane
        private void InitializeComponent()
        {
            this.Text = "Meniu Principal";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            // === Buton Delogare ===
            buttonDelogare = new Button()
            {
                Text = "Delogare",
                Size = new Size(80, 30),
                Location = new Point(10, 10),
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat
            };
            buttonDelogare.Click += (s, e) =>
            {
                try
                {
                    this.Hide();
                    FormLogin login = new FormLogin();
                    login.ShowDialog();
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare la delogare:\n" + ex.Message);
                }
            };

            // === Buton Iesire ===
            buttonIesire = new Button()
            {
                Text = "Iesire",
                Size = new Size(80, 30),
                BackColor = Color.IndianRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            buttonIesire.Click += (s, e) => Application.Exit();

            // === Eticheta Bun venit ===
            labelBunVenit = new Label()
            {
                Text = $"Bun venit, {username}!",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(160, 60),
                ForeColor = Color.DarkSlateGray
            };

            // === Panel pentru butoane ===
            Panel panelButoane = new Panel()
            {
                Size = new Size(400, 250),
                Location = new Point(50, 100)
            };

            // === Buton Legislatie ===
            buttonLegislatie = new Button()
            {
                Text = "Legislatie Auto",
                Size = new Size(180, 40),
                Location = new Point(110, 10),
                BackColor = Color.LightSteelBlue,
                FlatStyle = FlatStyle.Flat
            };
            buttonLegislatie.Click += (s, e) =>
            {
                try
                {
                    FormLegislatie legislatie = new FormLegislatie(username);
                    legislatie.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare la deschiderea paginii de legislatie:\n" + ex.Message);
                }
            };

            // === Buton Simulare Examen ===
            buttonSimulare = new Button()
            {
                Text = "Simulare Examen",
                Size = new Size(180, 40),
                Location = new Point(110, 60),
                BackColor = Color.LightSteelBlue,
                FlatStyle = FlatStyle.Flat
            };
            buttonSimulare.Click += (s, e) =>
            {
                try
                {
                    FormTest test = new FormTest(username);
                    test.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare la deschiderea testului:\n" + ex.Message);
                }
            };

            // === Buton Progres ===
            buttonProgres = new Button()
            {
                Text = "Analiza Progresului",
                Size = new Size(180, 40),
                Location = new Point(110, 110),
                BackColor = Color.LightSteelBlue,
                FlatStyle = FlatStyle.Flat
            };
            buttonProgres.Click += (s, e) =>
            {
                try
                {
                    FormRezultate rezultate = new FormRezultate(username);
                    rezultate.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare la analiza progresului:\n" + ex.Message);
                }
            };

            // === Buton Gestionare Utilizatori (vizibil doar admin) ===
            buttonUtilizatori = new Button()
            {
                Text = "Gestionare Utilizatori",
                Size = new Size(180, 40),
                Location = new Point(110, 10),
                Visible = false,
                BackColor = Color.LightGoldenrodYellow,
                FlatStyle = FlatStyle.Flat
            };
            buttonUtilizatori.Click += (s, e) =>
            {
                try
                {
                    FormUtilizatori f = new FormUtilizatori();
                    f.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare la gestionarea utilizatorilor:\n" + ex.Message);
                }
            };

            // === Buton Gestionare Intrebari (vizibil doar admin) ===
            buttonIntrebari = new Button()
            {
                Text = "Gestionare Intrebari",
                Size = new Size(180, 40),
                Location = new Point(110, 60),
                Visible = false,
                BackColor = Color.LightGoldenrodYellow,
                FlatStyle = FlatStyle.Flat
            };
            buttonIntrebari.Click += (s, e) =>
            {
                try
                {
                    FormIntrebari f = new FormIntrebari();
                    f.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare la gestionarea intrebarilor:\n" + ex.Message);
                }
            };

            // === Verifica daca utilizatorul este admin ===
            bool esteAdmin = username.ToLower() == "admin";

            buttonLegislatie.Visible = !esteAdmin;
            buttonSimulare.Visible = !esteAdmin;
            buttonProgres.Visible = true;
            buttonUtilizatori.Visible = esteAdmin;
            buttonIntrebari.Visible = esteAdmin;

            // === Adaugare butoane in panel ===
            panelButoane.Controls.Add(buttonLegislatie);
            panelButoane.Controls.Add(buttonSimulare);
            panelButoane.Controls.Add(buttonProgres);
            panelButoane.Controls.Add(buttonUtilizatori);
            panelButoane.Controls.Add(buttonIntrebari);

            // === Adaugare controale in formular ===
            this.Controls.Add(labelBunVenit);
            this.Controls.Add(panelButoane);
            this.Controls.Add(buttonDelogare);
            this.Controls.Add(buttonIesire);

            // === Seteaza pozitia butonului de Iesire la incarcarea formularului ===
            this.Load += (s, e) =>
            {
                buttonIesire.Location = new Point(this.ClientSize.Width - 90, 10);
            };
        }

        // Functie override pentru afisarea help-ului la apasarea tastei F1
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F1)
            {
                string caleHelp = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppData", "Help.hnd");

                if (System.IO.File.Exists(caleHelp))
                {
                    Help.ShowHelp(this, caleHelp);
                }
                else
                {
                    MessageBox.Show("Fisierul Help.chm nu a fost gasit.");
                }
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
