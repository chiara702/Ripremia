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

        protected override void OnAppearing() {
            base.OnAppearing();
            PrendiDatiCommerciante();
        }

        private ClassApiParco Parchetto = new ClassApiParco();
        //private DataRow rowAttivita;
        private double ValoreCoupon;
        private double StepMinimo;
        private int MaxCoupon;

        private async void PrendiDatiCommerciante() {
            App.DataRowCommerciante = await Task.Run(()=> Parchetto.EseguiQueryRow("AttivitaCommerciali", (int)App.DataRowUtente["AttivitaCommercialiId"]));
            ValoreCoupon = (double)App.DataRowCommerciante["ValoreCoupon"];
            StepMinimo = (double)App.DataRowCommerciante["SpesaMin"];
            MaxCoupon = (int)App.DataRowCommerciante["MaxCouponSpesaMin"];
            LblDenominazione.Text = "Nome: " + App.DataRowCommerciante["Nome"].ToString();          
            LblValCoupon.Text = "Valore di 1 coupon: " + App.DataRowCommerciante["ValoreCoupon"].ToString().Replace(".", ",") + "€";
            LblSpesaMin.Text = "Spesa minima: " + App.DataRowCommerciante["SpesaMin"].ToString().Replace(".", ",") + "€";
            LblMaxCouponXstep.Text = "Max Coupon: " + App.DataRowCommerciante["MaxCouponSpesaMin"].ToString();
            if((bool)App.DataRowCommerciante["Attivo"] == true) {
                LblStato.Text ="Stato: ATTIVO";
            } else {
                LblStato.Text = "Stato: NON ATTIVO";
            }


        }



        double TotDaScontare;
       
        double TotScontato;


        private async void BtnScansiona_Tapped(object sender, EventArgs e) {

            if (TotDaScontare < StepMinimo) {
                _ = DisplayAlert("Attenzione", "Inserire un valore valido da scontare", "OK");
                return;
            }

            if ((bool)App.DataRowCommerciante["Attivo"] == false) {
                _ = DisplayAlert("Attenzione", "E' necessario attivarsi come attività affiliata nella sezione MODIFICA IMPOSTAZIONI -> SCONTI COUPON ATTIVI -> SI", "OK");
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
                //var rowCliente = Parchetto.EseguiQueryRow("Utente", "CodiceMonetaVirtuale='" + codice + "'");
                var TableCliente = Parchetto.EseguiQuery("Select * From Utente Where CodiceMonetaVirtuale='" + Funzioni.AntiAp(codice) + "'");
                if (TableCliente.Rows.Count == 0) { await DisplayAlert("", "Nessun utente con codice moneta virtuale", "OK"); return; }
                var rowCliente = TableCliente.Rows[0];
                
                if ((int)rowCliente["IdComune"] != (int)App.DataRowCommerciante["ComuneId"]) {
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
                int AggiornaCouponAttivita = (int)App.DataRowCommerciante["CouponRaccolti"] + CouponUtilizzabili;
                Parchetto.EseguiUpdate("CouponRaccolti", (int)App.DataRowCommerciante["CouponRaccolti"], Par);

                if (Parchetto.LastError == true) {
                    await DisplayAlert("ATTENZIONE", "Errore nello scaricare i coupon!", "OK");
                    return;
                }
                Parchetto.EseguiCommand("Update AttivitaCommerciali SET CouponRaccolti=CouponRaccolti-(-" + CouponUtilizzabili + ") Where Id=" + App.DataRowCommerciante["Id"]);
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
                    DisplayAlert("", "OPERAZIONE CONCLUSA CON SUCCESSO", "OK");
                });
     
            };

        }


        private void BtnUp_Clicked(object sender, EventArgs e) {
            QntCouponUtilizzabiliValore = (int)(TotDaScontare / StepMinimo)*MaxCoupon;
            int ValoreEntryCoupon = Convert.ToInt32(EntryCouponN.Text);
            if (ValoreEntryCoupon < (int)QntCouponUtilizzabiliValore) {
                QntCouponUtilizzabiliValore = ValoreEntryCoupon + 1;
                EntryCouponN.Text = QntCouponUtilizzabiliValore.ToString();
            } else return;

        }

        private void BtnDown_Clicked(object sender, EventArgs e) {
            //QntCouponUtilizzabiliValore = (int)(TotDaScontare / StepMinimo) * MaxCoupon;
            if (QntCouponUtilizzabiliValore > 1) {
                QntCouponUtilizzabiliValore = QntCouponUtilizzabiliValore - 1;
                EntryCouponN.Text = QntCouponUtilizzabiliValore.ToString();
            } else QntCouponUtilizzabiliValore = 1;
        }


        private void BtnVisualizzaDati_Clicked(object sender, EventArgs e) {
            try {
                DisplayAlert("Coupon raccolti", "Fin ora hai raccolto " + Parchetto.EseguiCommand("Select CouponRaccolti From AttivitaCommerciali Where Id=" + App.DataRowCommerciante["Id"]).ToString() + " coupon!", "OK");
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
            TotDaScontare = Convert.ToDouble(EntryTotaleDaScontare.Text);
            QntCouponUtilizzabiliValore = MaxCoupon * ((int)(TotDaScontare / StepMinimo));
            EntryCouponN.Text = QntCouponUtilizzabiliValore.ToString();
            EntryCouponN_TextChanged(null, null);
        }


        private void EntryCouponN_TextChanged(object sender, TextChangedEventArgs e) {
            //QntCouponUtilizzabili = MaxCoupon * ((int)(TotDaScontare / StepMinimo));
            TotScontato = TotDaScontare - (QntCouponUtilizzabiliValore * ValoreCoupon);
            TxtTotaleScontato.Text = TotScontato.ToString("0.00 €").Replace(".",",");
        }

        private void BtnNuovaOperazione_Clicked(object sender, EventArgs e) {
            StkImpostazioneValori.IsVisible = true;
            EntryTotaleDaScontare.Text = "0,00";
            FrmCoupon.IsVisible = false;
            FrmNomeCliente.IsVisible = false;
            BtnVisualizzaDati.IsVisible = true;
            LblTotaleScontato.Text = "TOTALE SCONTATO PROVVISORIO";

        }

        async void BtnSettings_Clicked(object sender, EventArgs e) {
            await Navigation.PushAsync(new PageModificaImpoCommercianti());
        }

        
    }
}