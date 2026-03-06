// =============================================================================
// Fisier: FormLegislatie.cs
// Autor: David
// Descriere: Interfata pentru afisarea lectiilor teoretice si salvarea progresului
// Functionalitate: Vizualizare, cautare si bifare a lectiilor parcurse din legislatie
// Data: 2025-05-25
// =============================================================================

using ChestionarAuto.Core;
using ChestionarAuto.Legislatie;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ChestionarAuto.UI
{
    public partial class FormLegislatie : Form
    {
        private CheckedListBox listaLectii;
        private TextBox textContinut;
        private Button buttonSalveazaProgres;
        private Label labelProgres;
        private TextBox textCautare;
        private LectieManager manager;
        private string username;

        // Constructor care primeste username-ul utilizatorului conectat
        public FormLegislatie(string user)
        {
            username = user;
            manager = new LectieManager(username);
            InitializeComponent();
            AfiseazaLectii();
            ActualizeazaProgres();
        }

        // Initializeaza componentele vizuale
        private void InitializeComponent()
        {
            this.Text = "Legislatie Auto";
            this.Size = new Size(900, 550);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Lista lectiilor teoretice
            listaLectii = new CheckedListBox()
            {
                Location = new Point(20, 60),
                Size = new Size(300, 400)
            };
            listaLectii.SelectedIndexChanged += ListaLectii_SelectedIndexChanged;
            listaLectii.ItemCheck += (s, e) =>
            {
                if (listaLectii.IsHandleCreated)
                    BeginInvoke(new Action(ActualizeazaProgres));
            };


            // Afisare continut lectie
            textContinut = new TextBox()
            {
                Location = new Point(340, 60),
                Size = new Size(520, 400),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                Font = new Font("Segoe UI", 12F)
            };

            // Buton salvare progres bifat
            buttonSalveazaProgres = new Button()
            {
                Text = "Salveaza Progres",
                Location = new Point(340, 470),
                Size = new Size(150, 30)
            };
            buttonSalveazaProgres.Click += ButtonSalveazaProgres_Click;

            // Afiseaza progresul curent bifat
            labelProgres = new Label()
            {
                Text = "Progres: 0/0 lectii",
                Location = new Point(20, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            // TextBox pentru cautare in continutul lectiilor
            textCautare = new TextBox()
            {
                Location = new Point(340, 20),
                Size = new Size(300, 25),
                ForeColor = Color.Gray,
                Text = "Cauta in continut..."
            };
            textCautare.GotFocus += (s, e) =>
            {
                if (textCautare.Text == "Cauta in continut...")
                {
                    textCautare.Text = "";
                    textCautare.ForeColor = Color.Black;
                }
            };
            textCautare.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(textCautare.Text))
                {
                    textCautare.Text = "Cauta in continut...";
                    textCautare.ForeColor = Color.Gray;
                }
            };
            textCautare.TextChanged += TextCautare_TextChanged;

            // Adaugare controale pe formular
            this.Controls.Add(listaLectii);
            this.Controls.Add(textContinut);
            this.Controls.Add(buttonSalveazaProgres);
            this.Controls.Add(labelProgres);
            this.Controls.Add(textCautare);
        }

        // Afiseaza titlurile lectiilor in lista si aplica progresul bifat
        private void AfiseazaLectii()
        {
            listaLectii.Items.Clear();
            foreach (var lectie in manager.Lectii)
            {
                listaLectii.Items.Add(lectie.Titlu);
            }
            manager.AplicareProgres(listaLectii);
        }

        // Cand se selecteaza o lectie, afiseaza continutul acesteia
        private void ListaLectii_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = listaLectii.SelectedIndex;
            if (index >= 0 && index < manager.Lectii.Count)
            {
                textContinut.Text = manager.Lectii[index].Continut;
            }
        }

        // Salveaza progresul curent bifat in fisierul de progres
        private void ButtonSalveazaProgres_Click(object sender, EventArgs e)
        {
            manager.SalveazaProgres(listaLectii);
            MessageBox.Show("Progres salvat cu succes!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Actualizeaza eticheta cu progresul curent bifat
        private void ActualizeazaProgres()
        {
            int total = listaLectii.Items.Count;
            int bifate = listaLectii.CheckedItems.Count;
            labelProgres.Text = $"Progres: {bifate}/{total} lectii";
        }

        // Cauta in titlul si continutul lectiilor si afiseaza primul rezultat gasit
        private void TextCautare_TextChanged(object sender, EventArgs e)
        {
            if (textCautare.Text == "Cauta in continut...") return;

            string cautare = textCautare.Text.ToLower();
            for (int i = 0; i < manager.Lectii.Count; i++)
            {
                if (manager.Lectii[i].Continut.ToLower().Contains(cautare) ||
                    manager.Lectii[i].Titlu.ToLower().Contains(cautare))
                {
                    listaLectii.SelectedIndex = i;
                    return;
                }
            }

            textContinut.Text = !string.IsNullOrWhiteSpace(cautare)
                ? "Niciun rezultat gasit pentru cautarea introdusa."
                : "";
        }
    }
}
