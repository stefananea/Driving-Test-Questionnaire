// =============================================================================
// Fisier: FormIntrebari.cs
// Autor: David
// Descriere: Interfata grafica pentru gestionarea intrebarilor din chestionar
// Functionalitate: Adaugare, stergere, editare si vizualizare intrebari
// Data: 2025-05-25
// =============================================================================

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ChestionarAuto.Core;
using ChestionarAuto.Intrebari;

namespace ChestionarAuto.UI
{
    public partial class FormIntrebari : Form
    {
        private readonly QuestionManager questionManager;
        private ListBox listBoxIntrebari;
        private TextBox textBoxIntrebare;
        private TextBox textBoxRasp1;
        private TextBox textBoxRasp2;
        private TextBox textBoxRasp3;
        private ComboBox comboCorect;
        private Button buttonAdauga;
        private Button buttonSterge;
        private Button buttonEditeaza;

        // Constructorul formularului de intrebari
        public FormIntrebari()
        {
            questionManager = new QuestionManager(); // initializeaza managerul de intrebari
            InitializeComponent(); // initializeaza componentele grafice
            IncarcaIntrebari(); // incarca intrebarile existente in lista
        }

        // Componentele vizuale ale formularului
        private void InitializeComponent()
        {
            this.Text = "Gestionare Întrebări";
            this.Size = new Size(750, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;
            this.Font = new Font("Segoe UI", 10F);

            // Lista intrebari afisate
            listBoxIntrebari = new ListBox()
            {
                Location = new Point(30, 30),
                Size = new Size(680, 150),
                Font = new Font("Segoe UI", 10F)
            };
            listBoxIntrebari.SelectedIndexChanged += listBoxIntrebari_SelectedIndexChanged;
            this.Controls.Add(listBoxIntrebari);

            int xLabel = 30, xInput = 150, wInput = 560;

            // Etichete si textbox-uri pentru campuri
            Label lbl1 = new Label() { Text = "Întrebare:", Location = new Point(xLabel, 200), Size = new Size(100, 25) };
            textBoxIntrebare = new TextBox()
            {
                Location = new Point(xInput, 200),
                Width = wInput,
                Multiline = true,
                Height = 50,
                ScrollBars = ScrollBars.Vertical,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lbl2 = new Label() { Text = "Răspuns 1:", Location = new Point(xLabel, 270), Size = new Size(100, 25) };
            textBoxRasp1 = new TextBox() { Location = new Point(xInput, 270), Width = wInput };

            Label lbl3 = new Label() { Text = "Răspuns 2:", Location = new Point(xLabel, 310), Size = new Size(100, 25) };
            textBoxRasp2 = new TextBox() { Location = new Point(xInput, 310), Width = wInput };

            Label lbl4 = new Label() { Text = "Răspuns 3:", Location = new Point(xLabel, 350), Size = new Size(100, 25) };
            textBoxRasp3 = new TextBox() { Location = new Point(xInput, 350), Width = wInput };

            // ComboBox pentru raspunsul corect
            Label lblCorect = new Label() { Text = "Răspuns corect:", Location = new Point(xLabel, 390), Size = new Size(120, 25) };
            comboCorect = new ComboBox()
            {
                Location = new Point(xInput, 390),
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F)
            };
            comboCorect.Items.AddRange(new object[] { "1", "2", "3" });
            comboCorect.SelectedIndex = 0;

            // Buton Adaugare
            buttonAdauga = new Button()
            {
                Text = "Adaugă întrebare",
                Location = new Point(180, 440),
                Size = new Size(160, 35),
                BackColor = Color.LightGreen,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            buttonAdauga.Click += buttonAdauga_Click;

            // Buton Stergere
            buttonSterge = new Button()
            {
                Text = "Șterge selectata",
                Location = new Point(360, 440),
                Size = new Size(160, 35),
                BackColor = Color.IndianRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F)
            };
            buttonSterge.Click += buttonSterge_Click;

            // Buton Editare
            buttonEditeaza = new Button()
            {
                Text = "Editează întrebare",
                Location = new Point(540, 440),
                Size = new Size(160, 35),
                BackColor = Color.Khaki,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F)
            };
            buttonEditeaza.Click += buttonEditeaza_Click;

            // Adaugare toate controalele pe formular
            this.Controls.AddRange(new Control[]
            {
                lbl1, textBoxIntrebare,
                lbl2, textBoxRasp1,
                lbl3, textBoxRasp2,
                lbl4, textBoxRasp3,
                lblCorect, comboCorect,
                buttonAdauga, buttonSterge, buttonEditeaza
            });
        }

        // Incarca toate intrebarile existente si le afiseaza in lista
        private void IncarcaIntrebari()
        {
            listBoxIntrebari.Items.Clear();
            foreach (var intrebare in questionManager.ListaIntrebari)
            {
                listBoxIntrebari.Items.Add($"{intrebare.TextIntrebare} (Corect: {intrebare.VarianteRaspuns[intrebare.IndexRaspunsCorect]})");
            }
        }

        // Populeaza campurile cu datele intrebarii selectate
        private void listBoxIntrebari_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = listBoxIntrebari.SelectedIndex;
            if (index < 0 || index >= questionManager.ListaIntrebari.Count)
                return;

            var intrebare = questionManager.ListaIntrebari[index];
            textBoxIntrebare.Text = intrebare.TextIntrebare;
            textBoxRasp1.Text = intrebare.VarianteRaspuns[0];
            textBoxRasp2.Text = intrebare.VarianteRaspuns[1];
            textBoxRasp3.Text = intrebare.VarianteRaspuns[2];
            comboCorect.SelectedIndex = intrebare.IndexRaspunsCorect;
        }

        // Adauga o noua intrebare in lista si salveaza
        private void buttonAdauga_Click(object sender, EventArgs e)
        {
            string text = textBoxIntrebare.Text.Trim();
            string r1 = textBoxRasp1.Text.Trim();
            string r2 = textBoxRasp2.Text.Trim();
            string r3 = textBoxRasp3.Text.Trim();

            if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(r1) || string.IsNullOrWhiteSpace(r2) || string.IsNullOrWhiteSpace(r3))
            {
                MessageBox.Show("Completează toate câmpurile.");
                return;
            }

            int indexCorect = comboCorect.SelectedIndex;
            var variante = new List<string> { r1, r2, r3 };

            var intrebare = new Question
            {
                TextIntrebare = text,
                VarianteRaspuns = variante,
                IndexRaspunsCorect = indexCorect
            };

            try
            {
                questionManager.AdaugaIntrebare(intrebare);
                MessageBox.Show("Întrebare adăugată!");
                IncarcaIntrebari();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare: " + ex.Message);
            }
        }

        // Sterge intrebarea selectata din lista si fisier
        private void buttonSterge_Click(object sender, EventArgs e)
        {
            int index = listBoxIntrebari.SelectedIndex;
            if (index < 0 || index >= questionManager.ListaIntrebari.Count)
            {
                MessageBox.Show("Selectează o întrebare din listă.");
                return;
            }

            var intrebare = questionManager.ListaIntrebari[index];

            try
            {
                questionManager.StergeIntrebare(intrebare);
                MessageBox.Show("Întrebare ștearsă.");
                IncarcaIntrebari();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare: " + ex.Message);
            }
        }

        // Editeaza intrebarea selectata cu noile valori introduse
        private void buttonEditeaza_Click(object sender, EventArgs e)
        {
            int index = listBoxIntrebari.SelectedIndex;
            if (index < 0 || index >= questionManager.ListaIntrebari.Count)
            {
                MessageBox.Show("Selectează o întrebare pentru a edita.");
                return;
            }

            string text = textBoxIntrebare.Text.Trim();
            string r1 = textBoxRasp1.Text.Trim();
            string r2 = textBoxRasp2.Text.Trim();
            string r3 = textBoxRasp3.Text.Trim();

            if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(r1) ||
                string.IsNullOrWhiteSpace(r2) || string.IsNullOrWhiteSpace(r3))
            {
                MessageBox.Show("Completează toate câmpurile.");
                return;
            }

            int indexCorect = comboCorect.SelectedIndex;
            var variante = new List<string> { r1, r2, r3 };

            var intrebareNoua = new Question
            {
                TextIntrebare = text,
                VarianteRaspuns = variante,
                IndexRaspunsCorect = indexCorect
            };

            try
            {
                questionManager.ListaIntrebari[index] = intrebareNoua;
                questionManager.SalveazaIntrebari();
                MessageBox.Show("Întrebare editată cu succes.");
                IncarcaIntrebari();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare la editare: " + ex.Message);
            }
        }

        // Curata toate campurile si deselecteaza intrebarile
        private void ClearFields()
        {
            textBoxIntrebare.Clear();
            textBoxRasp1.Clear();
            textBoxRasp2.Clear();
            textBoxRasp3.Clear();
            comboCorect.SelectedIndex = 0;
            listBoxIntrebari.ClearSelected();
        }
    }
}
