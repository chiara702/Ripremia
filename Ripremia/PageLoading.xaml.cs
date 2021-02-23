using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageLoading : ContentPage {
        public PageLoading() {
            InitializeComponent();
            //var t1=Task.Run(() => Funzione());
            //var t2 = Task.Run(Animazione);
            //Task.WaitAll(new Task[]{t1,t2});
            Task.Run(Animazione);
            //R3.RotateTo(360, 200);
          


        }

        private async void Animazione() {
            var t2 = Task.Run(Funzione);
            await R1.FadeTo(0, 1);
            await R11.FadeTo(0, 1);
            await R3.FadeTo(0, 1);
            await R4.FadeTo(0, 1);
            await R5.FadeTo(0, 1);
            await R6.FadeTo(0, 1);
            await R7.FadeTo(0, 1);
            await R8.FadeTo(0, 1);
            await R10.FadeTo(0, 1);
            await R9.FadeTo(0, 1);

            await R3.TranslateTo(-15, 0, 1);
            await R3.FadeTo(1, 100);
            await R3.TranslateTo(0, 0, 100);

            await R4.TranslateTo(-15, 0, 1);
            await R4.FadeTo(1, 100);
            await R4.TranslateTo(0, 0, 100);

            await R5.TranslateTo(-15, 0, 1);
            await R5.FadeTo(1, 100);
            await R5.TranslateTo(0, 0, 100);

            await R6.TranslateTo(-15, 0, 1);
            await R6.FadeTo(1, 100);
            await R6.TranslateTo(0, 0, 100);

            await R7.TranslateTo(-15, 0, 1);
            await R7.FadeTo(1, 100);
            await R7.TranslateTo(0, 0, 100);

            await R8.TranslateTo(-15, 0, 1);
            await R8.FadeTo(1, 100);
            await R8.TranslateTo(0, 0, 100);

            await R9.TranslateTo(-15, 0, 1);
            await R9.FadeTo(1, 100);
            await R9.TranslateTo(0, 0, 100);

            await R10.TranslateTo(-15, 0, 1);
            await R10.FadeTo(1, 100);
            await R10.TranslateTo(0, 0, 100);

            _ = R1.FadeTo(1, 100);
            await R1.RotateTo(15, 100);
            await R1.RotateTo(-15, 100);
            await R1.RotateTo(0, 100);
            _ = R11.FadeTo(1, 100);
            await R11.RotateTo(15, 100);
            await R11.RotateTo(-15, 100);
            await R11.RotateTo(0, 100);
 

            if (await t2 == true) {
                if (App.DataRowUtente != null && (Boolean)App.DataRowUtente["VersLight"] == true) {
                    Device.BeginInvokeOnMainThread(() => App.Current.MainPage = new PageHomeLight());
                } else {
                    Device.BeginInvokeOnMainThread(() => App.Current.MainPage = new PageNavigatore());
                }
            }

            //await StackLoading.FadeTo(0, 400);
        }

        protected async override void OnAppearing() {
            base.OnAppearing();
            var currentVersion = VersionTracking.CurrentVersion;
            LblVersion.Text = "vers." + currentVersion;



        }

        public Boolean Funzione() {
            var parchetto = new ClassApiParco();
            var count = parchetto.EseguiCommand("Select Count(*) from Utente");
            if (parchetto.LastError==true) {
                Device.BeginInvokeOnMainThread(() => {
                    DisplayAlert("Attenzione", "Internet assente...", "OK");
                    Application.Current.MainPage = new PageOffLine();
                });

                //goto ripeti;
                //da fare se non è presente internet
                return false;
            }
            App.InizializzaDatiApp();
            if (App.DataRowUtente == null || App.DataRowSuperUser == null || App.DataRowComune == null) {
                DisplayAlert("Errore", "Errore connessione e recupero dati", "OK");
                App.Current.MainPage=new PageOffLine();
                return false;
            }
            PushApi.ResetTopics();
            return true;




            //Task.Delay(2400).Wait();

            //Device.BeginInvokeOnMainThread(() => App.Current.MainPage = new PageNavigatore());



        }

       

    }
}