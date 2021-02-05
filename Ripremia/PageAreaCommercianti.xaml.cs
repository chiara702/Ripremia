using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageAreaCommercianti : ContentPage {
        public PageAreaCommercianti() {
            InitializeComponent();
        }

        Double QuantitaEcopunti;

        private ScontoSelezionato scontoSelezionato = ScontoSelezionato.Nullo;
        private enum ScontoSelezionato {
            Nullo = 0,
            Sconto5,
            Sconto10,
            Sconto15,
            Sconto20
        }


        private void BtnIndietro_Clicked(object sender, EventArgs e) {
            Application.Current.MainPage = new PageNavigatore();
        }

        protected override bool OnBackButtonPressed() {
            BtnIndietro_Clicked(null, null);
            return true;
        }

        private void BtnScansionaQR_Clicked(object sender, EventArgs e) {

        }

        private void BtnSconto_Clicked(object sender, EventArgs e) {
            BtnSconto1.BorderColor = Color.Transparent;
            BtnSconto2.BorderColor = Color.Transparent;
            BtnSconto3.BorderColor = Color.Transparent;
            BtnSconto4.BorderColor = Color.Transparent;
            ImageButton sender1 = (ImageButton)sender;
            sender1.BorderColor = Color.Black;
            if (sender1 == BtnSconto1) scontoSelezionato = ScontoSelezionato.Sconto5;
            if (sender1 == BtnSconto2) scontoSelezionato = ScontoSelezionato.Sconto10;
            if (sender1 == BtnSconto3) scontoSelezionato = ScontoSelezionato.Sconto15;
            if (sender1 == BtnSconto4) scontoSelezionato = ScontoSelezionato.Sconto20;

        }

        private void BtnConvalida_Tapped(object sender, EventArgs e) {

        }
    }
}