﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageHomeLight : ContentPage {
        public PageHomeLight() {
            InitializeComponent();
            BarCodeId.BarcodeValue = Xamarin.Essentials.Preferences.Get("QrCodeNew", "1");

            LblUtente.Text = "Ciao, " + App.DataRowUtente["Nome"].ToString() + "!";
            //LblCoupon.Text = App.DataRowUtente["Coupon"].ToString();
            
        }
        ClassApiParco Parchetto = new ClassApiParco();
        private void BtnHomeFull_Clicked(object sender, EventArgs e) {
            Application.Current.MainPage = new PageLogin();
        }
        public void RiempiDati() {
            try {
                var apiEcoControl = new ClassApiEcoControl();
                int Qta = Convert.ToInt32(apiEcoControl.EseguiCommand("Select Qta From MonetaVirtuale Where QrCode='" + App.DataRowUtente["CodiceMonetaVirtuale"].ToString() + "'"));
                int Coupon = Convert.ToInt32(Parchetto.EseguiCommand("Select Coupon From Utente Where Id=" + App.DataRowUtente["Id"]).ToString());

                Device.BeginInvokeOnMainThread(() => {
                    LblPunteggioFamiglia.Text = Qta.ToString();
                    LblCoupon.Text = Coupon.ToString();
                });
            } catch (Exception) {
                DisplayAlert("Errore", "Errore recupero statistiche", "OK");
            }

        }
        protected override void OnAppearing() {
            base.OnAppearing();
            Task.Run(() => RiempiDati());
            Device.StartTimer(TimeSpan.FromSeconds(20), () => {
                RiempiDati();
                return true;
            });
        }
        private void BtnRefreshPage_Clicked(object sender, EventArgs e) {

        }
    }
}