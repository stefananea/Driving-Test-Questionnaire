// =============================================================================
// Fisier: FormRezultate.cs
// Autor: David
// Descriere: Afiseaza rezultatele testelor pentru utilizatorul curent sau toti utilizatorii (admin)
// Functionalitate: Citeste si afiseaza scorurile testelor din fisierul rezultate.json
// Data: 2025-05-25
// =============================================================================

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using ChestionarAuto.Core;

namespace ChestionarAuto.UI
{
    public class FormRezultate : Form
    {
        private readonly string username; // Username-ul utilizatorului conectat
        private ListBox listBoxRezultate; // Lista in care se afiseaza rezultatele
        private Label labelTitlu;         // Titlu din partea de sus a formularului
        private Button buttonInchide;     // Buton pentru inchidere formular

        public FormRezultate(string user)
        {
            username = user;
            InitializeComponent();
            AfiseazaRezultate();
        }

        // Initializare componente UI
        private void InitializeComponent()
        {
            this.Text = "Rezultate Teste";
            this.Size = new Size(700, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;
            this.Font = new Font("Segoe UI", 10F);

            labelTitlu = new Label()
            {
                Text = username.ToLower() == "admin"
                    ? "Rezultatele tuturor utilizatorilor"
                    : $"Rezultatele tale, {username}",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.DarkSlateGray,
                AutoSize = true,
                Location = new Point(20, 20)
            };

            listBoxRezultate = new ListBox()
            {
                Location = new Point(20, 60),
                Size = new Size(640, 320),
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.White
            };

            buttonInchide = new Button()
            {
                Text = "Închide",
                Size = new Size(100, 35),
                Location = new Point(560, 400),
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat
            };
            buttonInchide.Click += (s, e) => this.Close();

            this.Controls.Add(labelTitlu);
            this.Controls.Add(listBoxRezultate);
            this.Controls.Add(buttonInchide);
        }

        // Citeste rezultatele din fisier si le afiseaza in functie de utilizator
        private void AfiseazaRezultate()
        {
            try
            {
                string cale = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppData", "rezultate.json");
                if (!File.Exists(cale))
                {
                    listBoxRezultate.Items.Add("Nu există rezultate salvate.");
                    return;
                }

                string json = File.ReadAllText(cale);
                var rezultate = JsonConvert.DeserializeObject<List<Rezultat>>(json) ?? new List<Rezultat>();

                // Filtreaza rezultatele in functie de utilizator (admin vede tot)
                var rezultateFiltrate = username.ToLower() == "admin"
                    ? rezultate
                    : rezultate.Where(r => r.Username.Equals(username, StringComparison.OrdinalIgnoreCase)).ToList();

                listBoxRezultate.Items.Clear();

                if (rezultateFiltrate.Count == 0)
                {
                    listBoxRezultate.Items.Add("Nu există rezultate disponibile pentru acest utilizator.");
                    return;
                }

                // Afiseaza rezultatele sortate descrescator dupa data
                foreach (var r in rezultateFiltrate.OrderByDescending(r => r.DataTimp))
                {
                    string linie = $"{r.Username,-15} | Scor: {r.Scor}/{r.TotalIntrebari,-5} | Data: {r.DataTimp:g}";
                    listBoxRezultate.Items.Add(linie);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare la afișarea rezultatelor: " + ex.Message);
            }
        }
    }
}
