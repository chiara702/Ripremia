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

        
        }

        private void BtnCalcola_Clicked(object sender, EventArgs e) {
            MaxCoupon = (int)rowAttivita["MaxCouponSpesaMin"];
            TotDaScontare = Convert.ToDouble(EntryTotaleDaScontare.Text);     
          if ( (TotDaScontare / StepMinimo) <= MaxCoupon) { 
                 QntCouponUtilizzabiliValore = (int)(TotDaScontare / StepMinimo);
                 EntryCouponN.Text = QntCouponUtilizzabiliValore.ToString();
            } else {
                QntCouponUtilizzabiliValore = MaxCoupon;
                EntryCouponN.Text = QntCouponUtilizzabiliValore.ToString();
                    }
            if (QntCouponUtilizzabiliValore < MaxCoupon) {
                MaxCoupon = QntCouponUtilizzabiliValore;
            }
                {

            }

          }

        private void BtnAzzera_Clicked(object sender, EventArgs e) {
            EntryTotaleDaScontare.Text = "";
            EntryCouponN.Text = "1";
            
        }



        double TotDaScontare;
        int QntCouponUtilizzabili;
        double TotScontato;
        private void BtnScansiona_Tapped(object sender, EventArgs e) {
        QntCouponUtilizzabili = (int)(TotDaScontare / StepMinimo);
            if (QntCouponUtilizzabiliValore >= QntCouponUtilizzabili) {
                if (QntCouponUtilizzabili <= MaxCoupon) {
                    TotScontato = TotDaScontare - (QntCouponUtilizzabili * ValoreCoupon);
                    TxtTotaleScontato.Text = TotScontato.ToString();
                } else {
                    TotScontato = TotDaScontare - (MaxCoupon * ValoreCoupon);
                    TxtTotaleScontato.Text = TotScontato.ToString();
                }
            }else {
                TotScontato = TotDaScontare - (QntCouponUtilizzabiliValore * ValoreCoupon);
                TxtTotaleScontato.Text = TotScontato.ToString();
            }

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

        }

        private void BtnIndietro_Clicked(object sender, EventArgs e) {
            Application.Current.MainPage = new PageNavigatore();
        }

        protected override bool OnBackButtonPressed() {
            BtnIndietro_Clicked(null, null);
            return true;
        }

     
    }
}