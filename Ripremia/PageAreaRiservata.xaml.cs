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
            //Inizializza visualizzazione button
            if (((Boolean)App.DataRowComune["ServizioRitiro"]) == false) BtnRitiri.IsVisible = false;
            //if (((Boolean)App.DataRowComune["ServizioCentroRiuso"]) == false) BtnRitiri.IsVisible = false;
            if (((Boolean)App.DataRowComune["ServizioAbbandono"]) == false) BtnAbbandoni.IsVisible = false;


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
    }
}