// ============================================================================
// Fisier: LoginManager.cs
// Autor: David
// Descriere: Gestionarea autentificarii si inregistrarii utilizatorilor
// Functionalitate: Incarca, salveaza, autentifica si gestioneaza conturi din fisier JSON
// Data: 2025-05-25
// ============================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using ChestionarAuto.Core;

namespace ChestionarAuto.Login
{
    public class LoginManager
    {
        // Calea catre fisierul cu utilizatori
        private readonly string caleFisier;

        // Lista cu toti utilizatorii incarcati
        private List<User> utilizatori;

        // Constructor: seteaza calea si incarca lista
        public LoginManager()
        {
            caleFisier = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppData", "utilizatori.json");
            IncarcaUtilizatori();
        }

        // Proprietate pentru accesul la lista utilizatorilor
        public List<User> ListaUtilizatori => utilizatori;

        // Incarca utilizatorii din fisierul JSON
        private void IncarcaUtilizatori()
        {
            try
            {
                if (File.Exists(caleFisier))
                {
                    string json = File.ReadAllText(caleFisier);
                    utilizatori = JsonConvert.DeserializeObject<List<User>>(json) ?? new List<User>();
                }
                else
                {
                    utilizatori = new List<User>();
                }
            }
            catch
            {
                // Daca exista vreo eroare (ex: fisier corupt), pornim cu lista goala
                utilizatori = new List<User>();
            }
        }

        // Salveaza lista curenta de utilizatori in fisierul JSON
        public void SalveazaUtilizatori()
        {
            try
            {
                string json = JsonConvert.SerializeObject(utilizatori, Formatting.Indented);
                File.WriteAllText(caleFisier, json);
            }
            catch
            {
                // In productie, aici se poate loga exceptia intr-un fisier de log
            }
        }

        // Cauta un utilizator dupa username, returneaza un UserNull daca nu exista
        public User GasesteUtilizator(string username)
        {
            return utilizatori.FirstOrDefault(u => u.Username == username) ?? new UserNull();
        }

        // Verifica daca datele introduse corespund unui utilizator valid
        public bool Autentificare(string username, string parola)
        {
            var utilizator = GasesteUtilizator(username);
            return utilizator.IsValid() && utilizator.Parola == parola;
        }

        // Inregistreaza un utilizator nou, daca numele nu exista deja
        public bool Inregistreaza(string username, string parola)
        {
            if (utilizatori.Any(u => u.Username == username))
                return false; // deja exista

            utilizatori.Add(new User(username, parola));
            SalveazaUtilizatori();
            return true;
        }

        // Returneaza parola unui utilizator valid sau null daca nu exista
        public string RecuperareParola(string username)
        {
            var utilizator = GasesteUtilizator(username);
            return utilizator.IsValid() ? utilizator.Parola : null;
        }
    }
}
