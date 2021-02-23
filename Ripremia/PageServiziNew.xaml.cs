﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageServiziNew : ContentPage {
        public PageServiziNew() {
            InitializeComponent();
            LblUtente.Text = "Ciao, " + App.DataRowUtente["Nome"].ToString() + "!";
            MenuTop.MenuLaterale = MenuLaterale;
        }

        private void BtnPrenotaRitiro_Tapped(object sender, EventArgs e) {
            if (((Boolean)App.DataRowComune["ServizioRitiro"]) == false) {
                DisplayAlert("Servizio non disponibile", "Ci scusiamo, ma per il suo comune questo servizio non è attivo.", "OK");
                return;
            }
            Application.Current.MainPage = new PagePrenotaRitiro();
        }

        private void BtnDenunciaAbbandono_Tapped(object sender, EventArgs e) {
            if (((Boolean)App.DataRowComune["ServizioAbbandono"]) == false){
                DisplayAlert("Servizio non disponibile", "Ci scusiamo, ma per il suo comune questo servizio non è attivo.", "OK");
                return;
            }
            Application.Current.MainPage = new PageDenunciaAbbandono();
        }

        private void BtnCentroRiuso_Tapped(object sender, EventArgs e) {
            if (((Boolean)App.DataRowComune["ServizioCentroRiuso"]) == false){
                DisplayAlert("Servizio non disponibile", "Ci scusiamo, ma per il suo comune questo servizio non è attivo.", "OK");
                return;
            }
        }
        private void BtnInfoUser_Clicked(object sender, EventArgs e) {
            DisplayAlert("", "Questa è la pagina dedicata ai servizi, qui potrai prenotare un ritiro, segnalare un abbandono di rifiuti e visualizzare il calendario del ritiro rifiuti del tuo comune! Sempre che il tuo comune abbia attivato i servizi!", "OK");
        }

        private void BtnCalendario_Tapped(object sender, EventArgs e) {
            if (((Boolean)App.DataRowComune["ServizioCalendario"]) == false) {
                DisplayAlert("Servizio non disponibile", "Ci scusiamo, ma per il suo comune questo servizio non è attivo.", "OK");
                return;
            }
            Application.Current.MainPage = new PageCalendarioVisualizza();
        }
    }

}