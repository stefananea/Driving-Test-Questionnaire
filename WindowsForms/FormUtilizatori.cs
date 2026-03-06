// =============================================================================
// Fisier: FormUtilizatori.cs
// Autor: David
// Descriere: Formular pentru gestionarea utilizatorilor (admin)
// Functionalitate: Permite adaugarea, editarea, stergerea si resetarea progresului
// Data: 2025-05-25
// =============================================================================

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ChestionarAuto.Core;

namespace ChestionarAuto.UI
{
    public class FormUtilizatori : Form
    {
        private ListBox listaUtilizatori;
        private TextBox textBoxUsername;
        private TextBox textBoxParola;
        private Button buttonAdauga;
        private Button buttonEditeaza;
        private Button buttonSterge;
        private Button buttonResetProgres;
        private List<User> utilizatori;

        private string caleUtilizatori = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppData", "utilizatori.json");

        public FormUtilizatori()
        {
            InitializeComponent();
            IncarcaUtilizatori();
        }

        // Initializarea elementelor vizuale
        private void InitializeComponent()
        {
            this.Text = "Gestionare Utilizatori";
            this.Size = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            // Lista de utilizatori
            listaUtilizatori = new ListBox()
            {
                Location = new Point(20, 20),
                Size = new Size(250, 300),
                Font = new Font("Segoe UI", 10)
            };
            listaUtilizatori.SelectedIndexChanged += ListaUtilizatori_SelectedIndexChanged;

            // TextBox pentru username
            textBoxUsername = new TextBox()
            {
                Location = new Point(300, 40),
                Size = new Size(250, 25),
                ForeColor = Color.Gray,
                Text = "Username"
            };
            textBoxUsername.GotFocus += (s, e) =>
            {
                if (textBoxUsername.Text == "Username")
                {
                    textBoxUsername.Text = "";
                    textBoxUsername.ForeColor = Color.Black;
                }
            };
            textBoxUsername.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBoxUsername.Text))
                {
                    textBoxUsername.Text = "Username";
                    textBoxUsername.ForeColor = Color.Gray;
                }
            };

            // TextBox pentru parola
            textBoxParola = new TextBox()
            {
                Location = new Point(300, 80),
                Size = new Size(250, 25),
                ForeColor = Color.Gray,
                Text = "Parola"
            };
            textBoxParola.GotFocus += (s, e) =>
            {
                if (textBoxParola.Text == "Parola")
                {
                    textBoxParola.Text = "";
                    textBoxParola.ForeColor = Color.Black;
                }
            };
            textBoxParola.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBoxParola.Text))
                {
                    textBoxParola.Text = "Parola";
                    textBoxParola.ForeColor = Color.Gray;
                }
            };

            // Buton pentru adaugare utilizator
            buttonAdauga = new Button()
            {
                Text = "Adauga",
                Location = new Point(300, 120),
                Size = new Size(100, 30),
                BackColor = Color.LightGreen,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            buttonAdauga.Click += ButtonAdauga_Click;

            // Buton pentru editare utilizator
            buttonEditeaza = new Button()
            {
                Text = "Editeaza",
                Location = new Point(410, 120),
                Size = new Size(100, 30),
                BackColor = Color.Khaki,
                Font = new Font("Segoe UI", 9F)
            };
            buttonEditeaza.Click += ButtonEditeaza_Click;

            // Buton pentru stergere utilizator
            buttonSterge = new Button()
            {
                Text = "Sterge",
                Location = new Point(300, 160),
                Size = new Size(100, 30),
                BackColor = Color.LightCoral,
                Font = new Font("Segoe UI", 9F)
            };
            buttonSterge.Click += ButtonSterge_Click;

            // Buton pentru reset progres utilizator
            buttonResetProgres = new Button()
            {
                Text = "Reset Progres",
                Location = new Point(410, 160),
                Size = new Size(140, 30),
                BackColor = Color.LightSkyBlue,
                Font = new Font("Segoe UI", 9F)
            };
            buttonResetProgres.Click += ButtonResetProgres_Click;

            this.Controls.Add(listaUtilizatori);
            this.Controls.Add(textBoxUsername);
            this.Controls.Add(textBoxParola);
            this.Controls.Add(buttonAdauga);
            this.Controls.Add(buttonEditeaza);
            this.Controls.Add(buttonSterge);
            this.Controls.Add(buttonResetProgres);
        }

        // Incarca lista de utilizatori din fisierul JSON
        private void IncarcaUtilizatori()
        {
            try
            {
                if (File.Exists(caleUtilizatori))
                {
                    string json = File.ReadAllText(caleUtilizatori);
                    utilizatori = JsonConvert.DeserializeObject<List<User>>(json) ?? new List<User>();
                }
                else
                {
                    utilizatori = new List<User>();
                }

                ActualizeazaLista();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare la incarcarea utilizatorilor: " + ex.Message);
                utilizatori = new List<User>();
            }
        }

        // Actualizeaza lista din interfata cu utilizatorii existenti
        private void ActualizeazaLista()
        {
            listaUtilizatori.Items.Clear();
            foreach (var u in utilizatori)
            {
                if (u.Username.ToLower() != "admin")
                    listaUtilizatori.Items.Add(u.Username);
            }
        }

        // Cand selectezi un utilizator, se incarca datele in textbox-uri
        private void ListaUtilizatori_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = listaUtilizatori.SelectedIndex;
            if (index >= 0)
            {
                var user = utilizatori.FirstOrDefault(u => u.Username == listaUtilizatori.SelectedItem.ToString());
                if (user != null)
                {
                    textBoxUsername.Text = user.Username;
                    textBoxParola.Text = user.Parola;
                }
            }
        }

        // Adauga un utilizator nou
        private void ButtonAdauga_Click(object sender, EventArgs e)
        {
            string nume = textBoxUsername.Text.Trim();
            string parola = textBoxParola.Text;

            if (string.IsNullOrWhiteSpace(nume) || string.IsNullOrWhiteSpace(parola))
            {
                MessageBox.Show("Completeaza username si parola.");
                return;
            }

            if (utilizatori.Any(u => u.Username == nume))
            {
                MessageBox.Show("Utilizatorul exista deja.");
                return;
            }

            utilizatori.Add(new User(nume, parola));
            Salveaza();
            ActualizeazaLista();
        }

        // Editeaza utilizatorul selectat
        private void ButtonEditeaza_Click(object sender, EventArgs e)
        {
            string numeNou = textBoxUsername.Text.Trim();
            string parolaNoua = textBoxParola.Text;

            var user = utilizatori.FirstOrDefault(u => u.Username == listaUtilizatori.SelectedItem?.ToString());
            if (user != null)
            {
                user.Username = numeNou;
                user.Parola = parolaNoua;
                Salveaza();
                ActualizeazaLista();
            }
        }

        // Sterge utilizatorul selectat
        private void ButtonSterge_Click(object sender, EventArgs e)
        {
            string userDeSters = listaUtilizatori.SelectedItem?.ToString();
            if (userDeSters != null)
            {
                utilizatori.RemoveAll(u => u.Username == userDeSters);
                Salveaza();
                ActualizeazaLista();
            }
        }

        // Reseteaza progresul utilizatorului selectat (rezultate + legislatie)
        private void ButtonResetProgres_Click(object sender, EventArgs e)
        {
            string userTarget = listaUtilizatori.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(userTarget)) return;

            string caleRezultate = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppData", "rezultate.json");
            if (File.Exists(caleRezultate))
            {
                string json = File.ReadAllText(caleRezultate);
                var rezultate = JsonConvert.DeserializeObject<List<Rezultat>>(json) ?? new List<Rezultat>();
                rezultate.RemoveAll(r => r.Username == userTarget);
                File.WriteAllText(caleRezultate, JsonConvert.SerializeObject(rezultate, Formatting.Indented));
            }

            string caleProgres = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppData", $"progres_{userTarget}.json");
            if (File.Exists(caleProgres))
                File.Delete(caleProgres);

            MessageBox.Show("Progresul utilizatorului a fost resetat.");
        }

        // Salveaza utilizatorii actuali in fisierul JSON
        private void Salveaza()
        {
            try
            {
                string json = JsonConvert.SerializeObject(utilizatori, Formatting.Indented);
                File.WriteAllText(caleUtilizatori, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare la salvarea utilizatorilor: " + ex.Message);
            }
        }
    }
}
