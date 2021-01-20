﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewMenuTop : Grid {
        public ViewMenuTop() {
            InitializeComponent();
            BtnBell.IsVisible = _MostraBell;

        }

        //public event EventHandler<object> ThresholdReached;
        public ViewMenuLaterale MenuLaterale = null;

        private async void ImgMenu_Tapped(object sender, EventArgs e) {
            //EventHandler<object> handler = ThresholdReached;
            //if (handler != null) handler.Invoke(this, e);
            await MenuLaterale.Mostra();
        }

        

        public Boolean _MostraBell=false;
        public Boolean MostraBell {
            get {
                return _MostraBell;
            }
            set {
                _MostraBell = value;
                BtnBell.IsVisible = _MostraBell;
                Task.Run(() => GetNotifiche());
            }
        }
        public void GetNotifiche(){

            var Table = PageNotifiche.TabellaNotifiche();

            if (Table.Select("DataInizio>'" + Funzioni.ToQueryData(App.DataLetturaNotifiche) + "'").Length > 0) {
                Device.BeginInvokeOnMainThread(() => {
                    BtnBell.Source = "BellRed";
                });
                Task.Run(MuoviCampanella);
            }
        }

        private void MuoviCampanella() {
            Device.BeginInvokeOnMainThread(async () => {
                for (int z=0; z<=100; z++) { 
                    for (int x = 0; x <= 5; x++) {
                        await BtnBell.RotateTo(-10, 300);
                        await BtnBell.RotateTo(10, 300);
                    }
                    await Task.Delay(1000);
                }
            });
        }

        private void BtnBell_Clicked(object sender, EventArgs e) {
            Application.Current.MainPage = new PageNotifiche();
            App.DataLetturaNotifiche = DateTime.Now;
        }
    }
}