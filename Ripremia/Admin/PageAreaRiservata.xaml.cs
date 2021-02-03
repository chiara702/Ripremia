using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageAreaRiservata : ContentPage {
        public PageAreaRiservata() {
            InitializeComponent();
            //Inizializza button da rowutente
            
            
            
            if (((Boolean)App.DataRowUtente["PermessoNotifiche"]) == false) {
                BtnAddNotifiche.IsEnabled = false;
                BtnDeleteNotifiche.IsEnabled = false;
            }

            if (((Boolean)App.DataRowUtente["PermessoRitiri"]) == false) BtnRitiri.IsEnabled = false;
       
            if (((Boolean)App.DataRowUtente["PermessoAbbandoni"]) == false) BtnAbbandoni.IsEnabled = false;
            
            if (((Boolean)App.DataRowUtente["PermessoCalendario"]) == false) BtnCalendario.IsEnabled = false;

            if (((Boolean)App.DataRowUtente["PermessoCentroRiuso"]) == false) { BtnCentroRiuso.IsEnabled = false;}

        }

        //private async void ImgMenu_Tapped(object sender, EventArgs e) {
        //    MenuLaterale.IsVisible = true;
        //    await MenuLaterale.Mostra();
        //}

        private void BtnBack_Clicked(object sender, EventArgs e) {
            Application.Current.MainPage = new PageNavigatore();
        }

        private void BtnIndietro_Clicked(object sender, EventArgs e) {
            BtnBack_Clicked(null, null);
        }

        private void BtnAddNotifiche_Clicked(object sender, EventArgs e) {
            Application.Current.MainPage = new PageAddNotifiche();
        }

        

        private void BtnDeleteNotifiche_Clicked_1(object sender, EventArgs e) {
            Application.Current.MainPage = new PageDeleteNotifiche();
        }

        private void BtnCalendario_Clicked(object sender, EventArgs e) {
            //Application.Current.MainPage = new PageCalendarioGestione();
            Application.Current.MainPage = new NavigationPage(new PageCalendarioGestione());
        }

        protected override bool OnBackButtonPressed() {
            BtnIndietro_Clicked(null, null);
            return true;
        }

        private void BtnRitiri_Clicked(object sender, EventArgs e) {
            Application.Current.MainPage = new PageGestioneRitiri();
        }

        private void BtnAbbandoni_Clicked(object sender, EventArgs e) {
            Application.Current.MainPage = new Admin.PageGestioneAbbandoni();
        }
    }
}