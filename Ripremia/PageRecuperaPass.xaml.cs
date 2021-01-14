﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageRecuperaPass : ContentPage {
        public PageRecuperaPass() {
            InitializeComponent();
        }

        private void BtnRecupera_Clicked(object sender, EventArgs e) {
            if (Funzioni.IsValidEmail(TxtEmail.Text) == false) {
                DisplayAlert("", "Inserisci un'e-mail valida", "ok");

                return;
            }

            var FunzDb = new ClassApiParco();
            var row=FunzDb.EseguiQueryRow("Utente", "email='" + TxtEmail.Text.Replace("'", "") + "'");


            if (row != null) {
                Funzioni.SendEmail(row["Email"].ToString(), "chiara702@gmail.com", "RECUPERO PASSWORD ECOSERVICE APP", "Abbiamo recuperato i tuoi dati!" + "\nPassword: " + row["Password"].ToString() + "\n" + "Ora puoi inserire i tuoi dati per accedere.");
                DisplayAlert("", "Ti abbiamo inviato la password per e-mail.\nSe non la trovi controlla tra gli spam.", "ok");
            } else {
                DisplayAlert("Utente non valido", "Registrati per accedere ai servizi oppure verifica che i dati siano corretti.", "ok");
                return;
            }



        }

        private void BtnReturnLog_Clicked(object sender, EventArgs e) {
            Application.Current.MainPage = new PageLogin();
        }

        private void BtnProblemiAccesso_Clicked(object sender, EventArgs e) {
            Application.Current.MainPage = new PagePresentazione();
        }
    }
}