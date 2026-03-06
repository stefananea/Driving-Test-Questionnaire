// =============================================================================
// Fisier: FormTest.cs
// Autor: David
// Descriere: Formularul principal pentru simularea unui test auto cu 10 intrebari
// Functionalitate: Afiseaza intrebarile, gestioneaza scorul, timer-ul si salvarea rezultatului
// Data: 2025-05-25
// =============================================================================

using ChestionarAuto.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ChestionarAuto.Teste;

namespace ChestionarAuto.UI
{
    public class FormTest : Form
    {
        private TestManager managerTest;
        private int indexIntrebareCurenta = 0;
        private int scor = 0;
        private int greseli = 0;
        private string username;

        private Timer timerTest;
        private int secundeRamase = 600; // 10 minute

        private List<int> intrebariSarit = new List<int>();
        private int indexSkip = 0;
        private bool inSkipMode = false;

        private Label labelIntrebare;
        private RadioButton radioRaspuns1;
        private RadioButton radioRaspuns2;
        private RadioButton radioRaspuns3;
        private Button buttonUrmatoare;
        private Button buttonSkip;
        private Label labelScor;
        private Label labelTimer;

        public FormTest(string user)
        {
            username = user;
            InitializeComponent();
            var intrebari = IncarcaIntrebari();
            managerTest = new TestManager(intrebari);
            timerTest.Tick += TimerTest_Tick;
            timerTest.Start();
            AfiseazaIntrebareCurenta();
        }

        // Initializeaza componentele vizuale ale formularului
        private void InitializeComponent()
        {
            labelIntrebare = new Label();
            radioRaspuns1 = new RadioButton();
            radioRaspuns2 = new RadioButton();
            radioRaspuns3 = new RadioButton();
            buttonUrmatoare = new Button();
            buttonSkip = new Button();
            labelScor = new Label();
            labelTimer = new Label();
            timerTest = new Timer();

            this.Text = "Simulare Examen Auto";
            this.Size = new Size(700, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;
            this.Font = new Font("Segoe UI", 10F);

            // Label intrebare
            labelIntrebare.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            labelIntrebare.ForeColor = Color.DarkSlateBlue;
            labelIntrebare.Location = new Point(30, 30);
            labelIntrebare.Size = new Size(620, 60);
            labelIntrebare.TextAlign = ContentAlignment.MiddleLeft;

            // Radio raspunsuri
            Font fontRaspuns = new Font("Segoe UI", 10F);
            Size sizeRaspuns = new Size(600, 30);

            radioRaspuns1.Font = fontRaspuns;
            radioRaspuns1.Location = new Point(50, 110);
            radioRaspuns1.Size = sizeRaspuns;

            radioRaspuns2.Font = fontRaspuns;
            radioRaspuns2.Location = new Point(50, 150);
            radioRaspuns2.Size = sizeRaspuns;

            radioRaspuns3.Font = fontRaspuns;
            radioRaspuns3.Location = new Point(50, 190);
            radioRaspuns3.Size = sizeRaspuns;

            // Buton urmatoarea intrebare
            buttonUrmatoare.Text = "Urmatoarea intrebare";
            buttonUrmatoare.Location = new Point(180, 250);
            buttonUrmatoare.Size = new Size(180, 40);
            buttonUrmatoare.BackColor = Color.LightGreen;
            buttonUrmatoare.FlatStyle = FlatStyle.Flat;
            buttonUrmatoare.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            buttonUrmatoare.Click += buttonUrmatoare_Click;

            // Buton skip
            buttonSkip.Text = "Sari peste";
            buttonSkip.Location = new Point(380, 250);
            buttonSkip.Size = new Size(120, 40);
            buttonSkip.BackColor = Color.Khaki;
            buttonSkip.FlatStyle = FlatStyle.Flat;
            buttonSkip.Font = new Font("Segoe UI", 9F);
            buttonSkip.Click += buttonSkip_Click;

            // Label scor
            labelScor.Location = new Point(520, 10);
            labelScor.Size = new Size(150, 20);
            labelScor.TextAlign = ContentAlignment.MiddleRight;
            labelScor.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            labelScor.ForeColor = Color.DarkGreen;

            // Label timer
            labelTimer.Location = new Point(360, 10);
            labelTimer.Size = new Size(150, 20);
            labelTimer.Text = "Timp ramas: 10:00";
            labelTimer.TextAlign = ContentAlignment.MiddleRight;
            labelTimer.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            labelTimer.ForeColor = Color.DarkRed;

            // Timer
            timerTest.Interval = 1000;

            // Adauga controale
            this.Controls.AddRange(new Control[] {
             labelIntrebare,
             radioRaspuns1,
            radioRaspuns2,
            radioRaspuns3,
            buttonUrmatoare,
            buttonSkip,
            labelScor,
            labelTimer
            });
        }

        // Incarca 10 intrebari aleatorii din fisierul JSON
        private List<Question> IncarcaIntrebari()
        {
            string caleFisier = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppData", "intrebari.json");
            if (!File.Exists(caleFisier))
            {
                MessageBox.Show("Fisierul cu intrebari nu a fost gasit.");
                return new List<Question>();
            }

            string json = File.ReadAllText(caleFisier);
            var toate = JsonConvert.DeserializeObject<List<Question>>(json) ?? new List<Question>();

            // Amestecă și ia primele 10
            Random rand = new Random();
            return toate.OrderBy(q => rand.Next()).Take(10).ToList();
        }

        // Gestionarea timpului ramas
        private void TimerTest_Tick(object sender, EventArgs e)
        {
            secundeRamase--;
            int minute = secundeRamase / 60;
            int secunde = secundeRamase % 60;
            labelTimer.Text = $"Timp ramas: {minute:D2}:{secunde:D2}";

            if (secundeRamase <= 0)
            {
                timerTest.Stop();
                MessageBox.Show("Timpul a expirat! Testul a fost incheiat.");
                IncheieTest();
            }
        }

        // Marcheaza intrebarea actuala ca "sarit peste"
        private void buttonSkip_Click(object sender, EventArgs e)
        {
            if (!inSkipMode)
            {
                intrebariSarit.Add(indexIntrebareCurenta);
                indexIntrebareCurenta++;
                AfiseazaIntrebareCurenta();
            }
        }

        // Afiseaza intrebarea curenta in functie de starea testului
        private void AfiseazaIntrebareCurenta()
        {
            if (!inSkipMode && indexIntrebareCurenta >= managerTest.ListaIntrebari.Count)
            {
                if (intrebariSarit.Count > 0)
                {
                    inSkipMode = true;
                    indexSkip = 0;
                }
                else
                {
                    FinalizeazaTest();
                    return;
                }
            }

            if (inSkipMode && indexSkip >= intrebariSarit.Count)
            {
                FinalizeazaTest();
                return;
            }

            int indexAfisat = inSkipMode ? intrebariSarit[indexSkip] : indexIntrebareCurenta;
            Question intrebare = managerTest.ListaIntrebari[indexAfisat];

            if (!intrebare.IsValid())
            {
                MessageBox.Show("Intrebarea nu este valida!");
                return;
            }

            labelIntrebare.Text = intrebare.TextIntrebare;
            radioRaspuns1.Text = intrebare.VarianteRaspuns[0];
            radioRaspuns2.Text = intrebare.VarianteRaspuns[1];
            radioRaspuns3.Text = intrebare.VarianteRaspuns[2];

            radioRaspuns1.Checked = false;
            radioRaspuns2.Checked = false;
            radioRaspuns3.Checked = false;

            labelScor.Text = $"Scor: {scor}/{managerTest.ListaIntrebari.Count}";
        }

        // Procesarea raspunsului si trecerea la urmatoarea intrebare
        private void buttonUrmatoare_Click(object sender, EventArgs e)
        {
            int raspunsSelectat = -1;
            if (radioRaspuns1.Checked) raspunsSelectat = 0;
            else if (radioRaspuns2.Checked) raspunsSelectat = 1;
            else if (radioRaspuns3.Checked) raspunsSelectat = 2;

            if (raspunsSelectat == -1)
            {
                MessageBox.Show("Selecteaza un raspuns inainte de a continua.");
                return;
            }

            int indexCurent = inSkipMode ? intrebariSarit[indexSkip] : indexIntrebareCurenta;
            Question intrebare = managerTest.ListaIntrebari[indexCurent];

            if (raspunsSelectat == intrebare.IndexRaspunsCorect)
            {
                scor++;
            }
            else
            {
                greseli++;
                if (greseli >= 3)
                {
                    MessageBox.Show("Ai raspuns gresit de 3 ori. Testul a fost oprit.");
                    IncheieTest();
                    return;
                }
            }

            if (inSkipMode)
                indexSkip++;
            else
                indexIntrebareCurenta++;

            AfiseazaIntrebareCurenta();
        }

        // Marcheaza testul ca finalizat si afiseaza scorul
        private void FinalizeazaTest()
        {
            MessageBox.Show("Test finalizat! Scor final: " + scor + "/" + managerTest.ListaIntrebari.Count);
            IncheieTest();
        }

        // Opreste testul si permite reluarea sau iesirea
        private void IncheieTest()
        {
            timerTest.Stop();
            labelScor.Text = $"Scor final: {scor}/{managerTest.ListaIntrebari.Count}";
            buttonUrmatoare.Enabled = false;
            buttonSkip.Enabled = false;
            SalveazaRezultat();

            DialogResult rezultat = MessageBox.Show("Ai finalizat testul! Vrei sa reincepi testul?", "Test incheiat",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (rezultat == DialogResult.Yes)
            {
                scor = 0;
                greseli = 0;
                indexIntrebareCurenta = 0;
                secundeRamase = 600;
                intrebariSarit.Clear();
                inSkipMode = false;
                indexSkip = 0;
                timerTest.Start();
                buttonUrmatoare.Enabled = true;
                buttonSkip.Enabled = true;
                AfiseazaIntrebareCurenta();
            }
            else
            {
                this.Close();
            }
        }

        // Salveaza rezultatul testului in fisierul rezultate.json
        private void SalveazaRezultat()
        {
            try
            {
                string caleFisier = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppData", "rezultate.json");
                List<Rezultat> listaRezultate;

                if (File.Exists(caleFisier))
                {
                    string json = File.ReadAllText(caleFisier);
                    listaRezultate = JsonConvert.DeserializeObject<List<Rezultat>>(json) ?? new List<Rezultat>();
                }
                else
                {
                    listaRezultate = new List<Rezultat>();
                }

                Rezultat rezultatNou = new Rezultat(username, scor, managerTest.ListaIntrebari.Count);
                listaRezultate.Add(rezultatNou);

                string jsonNou = JsonConvert.SerializeObject(listaRezultate, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(caleFisier, jsonNou);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare la salvarea rezultatului: " + ex.Message);
            }
        }
    }
}
