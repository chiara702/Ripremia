using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace EcoServiceApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageAreaCommercianti : ContentPage {

        int QntCouponUtilizzabiliValore;

        public PageAreaCommercianti() {
            InitializeComponent();
            PrendiDatiCommerciante(); //aggiungere task
        }

        private ClassApiParco Parchetto = new ClassApiParco();
        private DataRow rowAttivita;
        private double ValoreCoupon;
        private double StepMinimo;
        private int MaxCoupon;

        private void PrendiDatiCommerciante() {
            rowAttivita = Parchetto.EseguiQueryRow("AttivitaCommerciali", (int)App.DataRowUtente["AttivitaCommercialiId"]);
            ValoreCoupon = (double)rowAttivita["ValoreCoupon"];
            StepMinimo = (double)rowAttivita["SpesaMin"];
            MaxCoupon = (int)rowAttivita["MaxCouponSpesaMin"];
            EntryCouponN.Text = MaxCoupon.ToString();
        }



        double TotDaScontare;
        int QntCouponUtilizzabili;
        double TotScontato;


        private async void BtnScansiona_Tapped(object sender, EventArgs e) {

            var options = new MobileBarcodeScanningOptions {
                AutoRotate = false,
                UseFrontCameraIfAvailable = false,
                TryHarder = true
            };
            var overlay = new ZXingDefaultOverlay {
                TopText = "SCANSIONA BARCODE DEL PRODOTTO",
                BottomText = ""
            };

            var Pagescanner = new ZXingScannerPage(options, overlay);
            //var Pagescanner = new ZXingScannerView();

            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted) {
                status = await Permissions.RequestAsync<Permissions.Camera>();
            }
            if (status != PermissionStatus.Granted)
                return;

            await Navigation.PushAsync(Pagescanner);
            Pagescanner.OnScanResult += (x) => {
                Pagescanner.IsScanning = false;
                Device.BeginInvokeOnMainThread(async () => {
                    await Navigation.PopAsync();
                    var codice = x.Text;
                    //await DisplayAlert("", "Codice Ok: " + x.Text, "OK");
                    
                   
                });
            };

        }


        private void BtnUp_Clicked(object sender, EventArgs e) {
            QntCouponUtilizzabiliValore = (int)(TotDaScontare / StepMinimo);
            if (QntCouponUtilizzabiliValore < ((int)(TotDaScontare / StepMinimo))) {
                QntCouponUtilizzabiliValore = QntCouponUtilizzabiliValore + 1;
                EntryCouponN.Text = QntCouponUtilizzabiliValore.ToString();
            } else {
                QntCouponUtilizzabiliValore = MaxCoupon;
                EntryCouponN.Text = QntCouponUtilizzabiliValore.ToString();
            }
        }

        private void BtnDown_Clicked(object sender, EventArgs e) {
            if (QntCouponUtilizzabiliValore > 1) {
                QntCouponUtilizzabiliValore = QntCouponUtilizzabiliValore - 1;
                EntryCouponN.Text = QntCouponUtilizzabiliValore.ToString();
            } else QntCouponUtilizzabiliValore = 1;
        }


        private void BtnVisualizzaDati_Clicked(object sender, EventArgs e) {
            DisplayAlert("Coupon raccolti", "Fin ora hai raccolto " + rowAttivita["CouponRaccolti"] + " coupon!", "OK");
        }

        private void BtnIndietro_Clicked(object sender, EventArgs e) {
            Application.Current.MainPage = new PageNavigatore();
        }

        protected override bool OnBackButtonPressed() {
            BtnIndietro_Clicked(null, null);
            return true;
        }

        private void EntryTotaleDaScontare_TextChanged(object sender, TextChangedEventArgs e) {
            if (Funzioni.Antinull(EntryTotaleDaScontare.Text) == "") return;
            MaxCoupon = (int)rowAttivita["MaxCouponSpesaMin"];
            TotDaScontare = Convert.ToDouble(EntryTotaleDaScontare.Text);


            if ((TotDaScontare / StepMinimo) <= MaxCoupon) {
                QntCouponUtilizzabiliValore = (int)(TotDaScontare / StepMinimo);
                EntryCouponN.Text = QntCouponUtilizzabiliValore.ToString();
            } else {
                QntCouponUtilizzabiliValore = MaxCoupon;
                EntryCouponN.Text = QntCouponUtilizzabiliValore.ToString();
            }
            if (QntCouponUtilizzabiliValore < MaxCoupon) {
                MaxCoupon = QntCouponUtilizzabiliValore;
            }

        }

        private void EntryCouponN_TextChanged(object sender, TextChangedEventArgs e) {
            QntCouponUtilizzabili = (int)(TotDaScontare / StepMinimo);
            if (QntCouponUtilizzabiliValore >= QntCouponUtilizzabili) {
                if (QntCouponUtilizzabili <= MaxCoupon) {
                    TotScontato = TotDaScontare - (QntCouponUtilizzabili * ValoreCoupon);
                    TxtTotaleScontato.Text = TotScontato.ToString();
                } else {
                    TotScontato = TotDaScontare - (MaxCoupon * ValoreCoupon);
                    TxtTotaleScontato.Text = TotScontato.ToString();
                }
            } else {
                TotScontato = TotDaScontare - (QntCouponUtilizzabiliValore * ValoreCoupon);
                TxtTotaleScontato.Text = TotScontato.ToString("0.00").Replace(".", ",");
            }
        }
    }
}