// =============================================================================
// Fisier: FormLogin.cs
// Autor: David
// Descriere: Formular pentru autentificare utilizator, înregistrare și recuperare parolă
// Functionalitate: Permite login-ul și înregistrarea utilizatorilor + deschiderea meniului principal
// Data: 2025-05-25
// =============================================================================

using ChestionarAuto.Core;
using ChestionarAuto.Login;
using ChestionarAutoApp;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ChestionarAuto.UI
{
    public class FormLogin : Form
    {
        private LoginManager loginManager;

        private Label labelUser;
        private Label labelParola;
        private TextBox textBoxUser;
        private TextBox textBoxParola;
        private Button buttonLogin;
        private Button buttonInregistrare;
        private Button buttonRecuperare;

        // Constructor formular de login
        public FormLogin()
        {
            InitializeComponent();
            loginManager = new LoginManager();
        }

        // Setari grafice + controale
        private void InitializeComponent()
        {
            labelUser = new Label();
            labelParola = new Label();
            textBoxUser = new TextBox();
            textBoxParola = new TextBox();
            buttonLogin = new Button();
            buttonInregistrare = new Button();
            buttonRecuperare = new Button();

            int latimeForm = 400;
            int latimeCamp = 220;
            int latimeButon = 110;

            this.ClientSize = new Size(latimeForm, 220);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Autentificare";
            this.BackColor = Color.WhiteSmoke;
            this.Font = new Font("Segoe UI", 10);

            // Eticheta Username
            labelUser.Text = "Username:";
            labelUser.TextAlign = ContentAlignment.MiddleRight;
            labelUser.Size = new Size(80, 26);
            labelUser.Location = new Point((latimeForm - latimeCamp) / 2 - 80, 30);

            // TextBox Username
            textBoxUser.Size = new Size(latimeCamp, 26);
            textBoxUser.Location = new Point((latimeForm - latimeCamp) / 2, 30);

            // Eticheta Parola
            labelParola.Text = "Parola:";
            labelParola.TextAlign = ContentAlignment.MiddleRight;
            labelParola.Size = new Size(80, 26);
            labelParola.Location = new Point((latimeForm - latimeCamp) / 2 - 80, 70);

            // TextBox Parola
            textBoxParola.Size = new Size(latimeCamp, 26);
            textBoxParola.Location = new Point((latimeForm - latimeCamp) / 2, 70);
            textBoxParola.UseSystemPasswordChar = true;

            // Buton Inregistrare
            buttonInregistrare.Text = "Inregistrare";
            buttonInregistrare.Size = new Size(latimeButon, 30);
            buttonInregistrare.Location = new Point(30, 130);
            buttonInregistrare.BackColor = Color.LightSteelBlue;
            buttonInregistrare.FlatStyle = FlatStyle.Flat;
            buttonInregistrare.Click += buttonInregistrare_Click;

            // Buton Recuperare parola
            buttonRecuperare.Text = "Recuperare";
            buttonRecuperare.Size = new Size(latimeButon, 30);
            buttonRecuperare.Location = new Point(latimeForm - latimeButon - 30, 130);
            buttonRecuperare.BackColor = Color.LightSteelBlue;
            buttonRecuperare.FlatStyle = FlatStyle.Flat;
            buttonRecuperare.Click += buttonRecuperare_Click;

            // Buton Login
            buttonLogin.Text = "Login";
            buttonLogin.Size = new Size(latimeButon, 30);
            buttonLogin.Location = new Point((latimeForm - latimeButon) / 2, 130);
            buttonLogin.BackColor = Color.LightGreen;
            buttonLogin.FlatStyle = FlatStyle.Flat;
            buttonLogin.Click += buttonLogin_Click;

            this.AcceptButton = buttonLogin; // Enter activeaza login

            // Adauga toate controalele pe formular
            Controls.AddRange(new Control[] {
                labelUser, textBoxUser,
                labelParola, textBoxParola,
                buttonInregistrare, buttonRecuperare, buttonLogin
            });
        }

        // Login: verifica datele introduse si deschide meniul daca sunt corecte
        private void buttonLogin_Click(object sender, EventArgs e)
        {
            string user = textBoxUser.Text.Trim();
            string parola = textBoxParola.Text;

            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(parola))
            {
                MessageBox.Show("Te rugam sa introduci atat username-ul cat si parola.");
                return;
            }

            if (loginManager.Autentificare(user, parola))
            {
                MessageBox.Show("Autentificare reusita!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Hide();
                FormMeniu meniu = new FormMeniu(user);
                meniu.ShowDialog();
                this.Close();
            }
            else
            {
                MessageBox.Show("Utilizator sau parola incorecta.", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Inregistrare: creeaza un nou cont daca nu exista deja
        private void buttonInregistrare_Click(object sender, EventArgs e)
        {
            string user = textBoxUser.Text.Trim();
            string parola = textBoxParola.Text;

            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(parola))
            {
                MessageBox.Show("Completeaza username si parola pentru inregistrare.");
                return;
            }

            if (!loginManager.Inregistreaza(user, parola))
            {
                MessageBox.Show("Utilizatorul exista deja.");
                return;
            }

            MessageBox.Show("Utilizator inregistrat cu succes!");
        }

        // Recuperare parola pe baza username-ului
        private void buttonRecuperare_Click(object sender, EventArgs e)
        {
            string user = textBoxUser.Text.Trim();

            if (string.IsNullOrEmpty(user))
            {
                MessageBox.Show("Introdu un username pentru a recupera parola.");
                return;
            }

            string parola = loginManager.RecuperareParola(user);
            if (parola != null)
                MessageBox.Show($"Parola pentru {user} este: {parola}");
            else
                MessageBox.Show("Utilizatorul nu a fost gasit.");
        }
    }
}
