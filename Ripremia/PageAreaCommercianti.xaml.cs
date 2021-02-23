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
            if (TotDaScontare < StepMinimo) {
                _ = DisplayAlert("Attenzione", "Inserire un valore valido da scontare", "OK");
                return;

            }


            var options = new MobileBarcodeScanningOptions {
                AutoRotate = false,
                UseFrontCameraIfAvailable = false,
                TryHarder = true
            };
            var overlay = new ZXingDefaultOverlay {
                TopText = "SCANSIONA IL QR-CODE DEL CLIENTE",
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
            Pagescanner.OnScanResult += async (x) => {
                Pagescanner.IsScanning = false;
                Device.BeginInvokeOnMainThread(async () => await Navigation.PopAsync());
                var codice = x.Text;
                //Device.BeginInvokeOnMainThread(() => DisplayAlert("", "Codice Ok: " + x.Text, "OK"));

                ClassApiParco Parchetto = new ClassApiParco();
                var rowCliente = Parchetto.EseguiQueryRow("Utente", "CodiceMonetaVirtuale='" + codice + "'");
                
                if ((int)rowCliente["IdComune"] != (int)rowAttivita["ComuneId"]) {
                    Device.BeginInvokeOnMainThread(() => DisplayAlert("ATTENZIONE", "Il cliente appartiene ad un comune diverso dall'attività!", "OK"));
                    return;
                }
                //Verifiche
                int MoltiplicatoreCoupon = (int)App.DataRowComune["MoltiplicatoreCoupon"];
                int ComuneEcopuntiCoupon = (int)App.DataRowComune["EcopuntiCoupon"];
                int CouponUtente = (int)rowCliente["Coupon"];
                int CouponUtilizzabili = Convert.ToInt32(EntryCouponN.Text);
                

                if (CouponUtente < 1) {
                    Device.BeginInvokeOnMainThread(() => DisplayAlert("Attenzione", "Il cliente non possiede coupon!", "OK"));
                    return;
                }
                if (CouponUtente < CouponUtilizzabili) {
                    CouponUtilizzabili = CouponUtente;
                }
              

                Device.BeginInvokeOnMainThread(() => {
                    StkImpostazioneValori.IsVisible = false;
                    BtnVisualizzaDati.IsVisible = false;
                    LblTotaleScontato.Text = "TOTALE SCONTATO EFFETTIVO";
                    TotScontato = TotDaScontare - (CouponUtilizzabili * ValoreCoupon);
                    TxtTotaleScontato.Text = TotScontato.ToString("0.00 €").Replace(".",",");
                    TxtCouponUsati.Text = CouponUtilizzabili.ToString();
                });

                //BtnVisualizzaDati.IsEnabled = false;

                var Par = Parchetto.GetParam();
                CouponUtente = CouponUtente - CouponUtilizzabili;
                Par.AddParameterInteger("Coupon", CouponUtente);            
                Parchetto.EseguiUpdate("Utente", (int)rowCliente["Id"], Par);
                int AggiornaCouponAttivita = (int)rowAttivita["CouponRaccolti"] + CouponUtilizzabili;
                Parchetto.EseguiUpdate("CouponRaccolti", (int)rowAttivita["CouponRaccolti"], Par);

                if (Parchetto.LastError == true) {
                    await DisplayAlert("ATTENZIONE", "Errore nello scaricare i coupon!", "OK");
                    return;
                }
                Parchetto.EseguiCommand("Update AttivitaCommerciali SET CouponRaccolti=CouponRaccolti-(-" + CouponUtilizzabili + ") Where Id=" + rowAttivita["Id"]);
                if (Parchetto.LastError == true) {
                    await DisplayAlert("ATTENZIONE", "Errore nel caricare i coupon sull'attività commerciale!", "OK");
                    return;
                }
                Device.BeginInvokeOnMainThread(() => {
                    FrmNomeCliente.IsVisible = true;
                    TxtNomeCliente.Text = rowCliente["Nome"].ToString() + " " + rowCliente["Cognome"].ToString();
                    FrmCoupon.IsVisible = true;
                    TxtCouponResidui.Text = ((int)rowCliente["Coupon"] - (int)CouponUtilizzabili).ToString();
                    BtnNuovaOperazione.IsVisible = true;
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
            try {
                DisplayAlert("Coupon raccolti", "Fin ora hai raccolto " + Parchetto.EseguiCommand("Select CouponRaccolti From AttivitaCommerciali Where Id=" + rowAttivita["Id"]).ToString() + " coupon!", "OK");
            } catch (Exception) { }
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
            EntryCouponN_TextChanged(null, null);

        }

        private void EntryCouponN_TextChanged(object sender, TextChangedEventArgs e) {
            QntCouponUtilizzabili = (int)(TotDaScontare / StepMinimo);
            if (QntCouponUtilizzabiliValore >= QntCouponUtilizzabili) {
                if (QntCouponUtilizzabili <= MaxCoupon) {
                    TotScontato = TotDaScontare - (QntCouponUtilizzabili * ValoreCoupon);
                    TxtTotaleScontato.Text = TotScontato.ToString("0.00 €").Replace(".",",");
                } else {
                    TotScontato = TotDaScontare - (MaxCoupon * ValoreCoupon);
                    TxtTotaleScontato.Text = TotScontato.ToString("0.00 €").Replace(".", ",");
                }
            } else {
                TotScontato = TotDaScontare - (QntCouponUtilizzabiliValore * ValoreCoupon);
                TxtTotaleScontato.Text = TotScontato.ToString("0.00 €").Replace(".", ",");
            }
        }

        private void BtnNuovaOperazione_Clicked(object sender, EventArgs e) {
            StkImpostazioneValori.IsVisible = true;
            EntryTotaleDaScontare.Text = "0,00";
            FrmCoupon.IsVisible = false;
            FrmNomeCliente.IsVisible = false;
            BtnVisualizzaDati.IsVisible = true;
            LblTotaleScontato.Text = "TOTALE SCONTATO PROVVISORIO";

        }
    }
}