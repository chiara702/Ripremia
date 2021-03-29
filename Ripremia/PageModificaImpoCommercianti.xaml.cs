using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageModificaImpoCommercianti : ContentPage {

        private DataRow rowAttivita;
        private ClassApiParco Parchetto = new ClassApiParco();
        public PageModificaImpoCommercianti() {
            InitializeComponent();
            PrendiDatiCommerciante(); //aggiungere task
        }
            double ValoreCoupon;
            double StepMinimo;
            int MaxCoupon;

        private void PrendiDatiCommerciante() {
            rowAttivita = Parchetto.EseguiQueryRow("AttivitaCommerciali", (int)App.DataRowUtente["AttivitaCommercialiId"]);
            ValoreCoupon = (double)rowAttivita["ValoreCoupon"];
            StepMinimo = (double)rowAttivita["SpesaMin"];
            MaxCoupon = (int)rowAttivita["MaxCouponSpesaMin"];
            RadioPushSi.IsChecked = ((bool)rowAttivita["Attivo"] == true);
            RadioPushNo.IsChecked = ((bool)rowAttivita["Attivo"] == false);
            TxtValoreCoupon.Text = ValoreCoupon.ToString("0.00 €").Replace(".", ",");
            EntryMaxCouponUtilizzabili.Text = MaxCoupon.ToString();
            EntrySpesaMin.Text = StepMinimo.ToString();

        }

        async void BtnIndietro_Clicked(object sender, EventArgs e) {
            await Navigation.PopAsync(true);
        }

        protected override bool OnBackButtonPressed() {
            BtnIndietro_Clicked(null, null);
            return true;

        }

        private async void BtnSalva_Clicked(object sender, EventArgs e) {
            EntryMaxCouponUtilizzabili.Text = Funzioni.Antinull(EntryMaxCouponUtilizzabili.Text);
            EntrySpesaMin.Text = Funzioni.Antinull(EntrySpesaMin.Text);
            if (EntryMaxCouponUtilizzabili.Text == "") EntryMaxCouponUtilizzabili.Text = "0";
            if (EntrySpesaMin.Text == "") EntrySpesaMin.Text = "0";
            if (RadioPushSi.IsChecked == true) {
                if (Convert.ToDouble(EntrySpesaMin.Text) <= 0) {await DisplayAlert("", "E' necessario inserire gli step minimi di spesa!", "OK"); return; }
                if (Convert.ToDouble(EntryMaxCouponUtilizzabili.Text) <= 0) {await DisplayAlert("", "E' necessario inserire almeno 1 coupon nei Coupon max utilizzabili", "OK"); return; }
            }
            var Par = Parchetto.GetParam();
            Par.AddParameterObject("SpesaMin",Convert.ToDouble(EntrySpesaMin.Text));
            Par.AddParameterObject("MaxCouponSpesaMin", Convert.ToDouble(EntryMaxCouponUtilizzabili.Text));
            Par.AddParameterObject("Attivo", RadioPushSi.IsChecked ? true : false);
            Parchetto.EseguiUpdate("AttivitaCommerciali",(int)rowAttivita["Id"],Par);
            await Navigation.PopAsync();
            DisplayAlert("","Modifiche effettuate correttamente","OK");
        }

        private void EntryMaxCouponUtilizzabili_TextChanged(object sender, TextChangedEventArgs e) {
            AggiornaDati();
            
        }

        private void TxtSpesaMin_TextChanged(object sender, TextChangedEventArgs e) {
            AggiornaDati();
        }

        private void AggiornaDati() {
            try {
                Double SpesaMin = Convert.ToDouble(EntrySpesaMin.Text);
                Double MaxCoupon = Convert.ToDouble(EntryMaxCouponUtilizzabili.Text);
                Double Sconto = ((ValoreCoupon * 100) / SpesaMin * MaxCoupon);
                LblMaxPercSconto.Text = "Percentuale di sconto applicata " + Sconto.ToString("0.00") + "%";
                LblMaxPercSconto.Text += "\n\nEsempi:\n Su una spesa di " + SpesaMin.ToString("0.00") + "€, il totale pagato dal cliente sarà " + (SpesaMin - (ValoreCoupon * MaxCoupon)).ToString("0.00") + "€ utilizzando " + MaxCoupon + " coupon.";
                LblMaxPercSconto.Text += "\n\nSu una spesa di " + (SpesaMin * 2).ToString("0.00") + "€, il totale pagato dal cliente sarà " + (SpesaMin * 2 - (ValoreCoupon * MaxCoupon * 2)).ToString("0.00") + "€ utilizzando " + MaxCoupon * 2 + " coupon.";
                LblMaxPercSconto.Text += "\n\nSu una spesa di " + (SpesaMin * 2 + 2).ToString("0.00") + "€, il totale pagato dal cliente sarà " + ((SpesaMin * 2 + 2) - (ValoreCoupon * MaxCoupon * 2)).ToString("0.00") + "€ utilizzando " + MaxCoupon * 2 + " coupon.";
            } catch (Exception e) {
                LblMaxPercSconto.Text = "Errore! Controlla i dati inseriti!";
            }
        }



    }
}