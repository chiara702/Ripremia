using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class PageHomeNew : ContentPage {
        public IList<Statistica> Statistiche { get; private set; }

        public class Statistica {
            public string Image { get; set; }
            public string Dati { get; set; }
            public string Dettagli { get; set; }
        }
        protected override void OnAppearing() {
            base.OnAppearing();
            Task.Run(() => {
                for (var x = 0; x <= 100; x++) {
                    System.Threading.Thread.Sleep(3000);
                    Carosel1.Position = x % 4;
                }
            });
        }
        public PageHomeNew() {
            InitializeComponent();
            Task.Run(() => {
                Task.Delay(1500);
                CreateStatisticheCollection();
                Device.BeginInvokeOnMainThread(() => BindingContext = this);
            });
            
            

            LblUtente.Text = "Ciao, " + App.DataRowUtente["Nome"].ToString() + "!";
            MenuTop.MenuLaterale = MenuLaterale;
        }
        
        void CreateStatisticheCollection() {
            Statistiche = new List<Statistica>();

            Statistiche.Add(new Statistica {
                Image = "oil",
                Dati = UtenteDatiMemoria.UtenteOilRaccolto.ToString("0.00").Replace(".",","),
            Dettagli = "Litri di olio raccolto",
            });

            Statistiche.Add(new Statistica {
                Image = "plastica",
                Dati = UtenteDatiMemoria.UtentePetRaccolto.ToString(),
                Dettagli = "Bottiglie di plastica raccolte",
            });

            Statistiche.Add(new Statistica {
                Image = "co2",
                Dati = UtenteDatiMemoria.UtenteKgCO2Risparmiato.ToString("0.000").Replace(".",","),
                Dettagli = "Kg di CO2 risparmiati",
            });

            Statistiche.Add(new Statistica {
                Image = "petrolio",
                Dati = UtenteDatiMemoria.UtenteBariliPetrolioRisparmiato.ToString("0.000").Replace(".",","),
                Dettagli = "Barili di petrolio risparmiati",
            });
            
        }





        private async void ImgMenu_Tapped(object sender, EventArgs e) {
            MenuLaterale.IsVisible = true;
            await MenuLaterale.Mostra();
        }


        int Stato = 0;
        private async void BtnShowQR_Tapped(object sender, EventArgs e) {
            
            BarCodeId.BarcodeValue = Xamarin.Essentials.Preferences.Get("QrCodeNew", "1");
            if (Stato == 0) {
                await BtnShowQRFrame.TranslateTo(1, -2, 100);
                await BtnShowQRFrame.TranslateTo(0, 0, 100);
                ImgArrow.IsVisible = false;
                _ = StackBarCodeId.FadeTo(1, 250);
                await ImgUp.FadeTo(0, 500);
                _ = ZoomQRcode.ScaleTo(0.2, 1);
                //_ = ZoomQRcode.TranslateTo(-40, -40, 600);
                _ = ZoomQRcode.RotateTo(360, 350);
                await ZoomQRcode.ScaleTo(1, 600);
                _ = ZoomQRcode.RotateTo(-360, 1);
                Stato = 1;
                _ = BtnShowQRtxt.FadeTo(1, 1);
                _ = BtnShowQRtxt.FadeTo(0, 250);
                await BtnUnshowQRtxt.FadeTo(0, 250);
                _ = BtnUnshowQRtxt.FadeTo(1, 250);
                //await ZoomQRcode.ScaleTo(1, 500);
                //_ = ZoomQRcode.TranslateTo(0, 0, 300);
                BtnShowQRtxt.IsVisible = false;
                BtnUnshowQRtxt.IsVisible = true;

            } else {
                await BtnShowQRFrame.TranslateTo(1, -2, 100);
                await BtnShowQRFrame.TranslateTo(0, 0, 100);
                ImgArrow.IsVisible = true;
                _ = StackBarCodeId.FadeTo(0, 500);
                await ImgUp.FadeTo(1, 500);
                Stato = 0;
                _ = BtnUnshowQRtxt.FadeTo(1, 1);
                _ = BtnUnshowQRtxt.FadeTo(0, 250);
                await BtnShowQRtxt.FadeTo(0, 250);
                _ = BtnShowQRtxt.FadeTo(1, 250);
                BtnShowQRtxt.IsVisible = true;
                BtnUnshowQRtxt.IsVisible = false;

            }
            
        }

        

        private void TapGestureRecognizer_Tapped_1(object sender, EventArgs e) {
            DisplayAlert("prova", "prova", "ok");
        }

        private void TapNumeriUtili_Tapped(object sender, EventArgs e) {
            DisplayAlert("prova", "prova", "ok");
        }

        
    }

}
