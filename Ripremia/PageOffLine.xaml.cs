using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageOffLine : ContentPage {
        public PageOffLine() {
            InitializeComponent();
            BarCodeId.BarcodeValue = Xamarin.Essentials.Preferences.Get("QrCodeNew", "1");
            if (BarCodeId.BarcodeValue == "1") BarCodeId.IsVisible = false; else BarCodeId.IsVisible = true;
            Task.Run(() => CheckOnLine());
            
        }
        public void CheckOnLine() {
            var parchetto = new ClassApiParco();
            for (int x=0; x<=1000; x++) {
                System.Threading.Thread.Sleep(3000);
                var c=parchetto.EseguiCommand("Select Count(*) From Utente");
                if (parchetto.LastError==false) {
                    Device.BeginInvokeOnMainThread(()=>App.Current.MainPage = new PageLoading());
                    return;
                }
            }
        }
    }
}