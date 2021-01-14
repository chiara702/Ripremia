using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageLoading : ContentPage {
        public PageLoading() {
            InitializeComponent();
            Task.Run(() => Funzione());
        }

        public void Funzione() {
            var parchetto = new ClassApiParco();
            var count = parchetto.EseguiCommand("Select Count(*) from Utente");
            if (parchetto.LastError==true) {
                Device.BeginInvokeOnMainThread(() => {
                    DisplayAlert("Attenzione", "Internet assente...", "OK");
                    Application.Current.MainPage = new PageOffLine();
                });

                //goto ripeti;
                //da fare se non è presente internet
                return;
            }
            App.InizializzaDatiApp();
            if (App.DataRowUtente == null || App.DataRowSuperUser == null || App.DataRowComune == null) {
                DisplayAlert("Errore", "Errore connessione e recupero dati", "OK");
                App.Current.MainPage=new PageOffLine();
                return;
            }

            //var rowUtente = parchetto.EseguiQueryRow("Utente", "Email='" + Funzioni.AntiAp(Xamarin.Essentials.Preferences.Get("Email", "")) + "'");
            //if (parchetto.LastError == true) {
            //    Device.BeginInvokeOnMainThread(() => DisplayAlert("Errore", "Errore connessione", "Ok"));
            //    //Da fare se non c'è internet
            //    return;
            //}
            //if (rowUtente == null) {
            //    //email errata
            //    Device.BeginInvokeOnMainThread(() => App.Current.MainPage = new PagePresentazione());
            //    return;
            //}
            //if (rowUtente["Password"].ToString() != Xamarin.Essentials.Preferences.Get("Password", "")) {
            //    //password errata
            //    Device.BeginInvokeOnMainThread(() => App.Current.MainPage = new PagePresentazione());
            //    return;
            //}
            //App.RowUtente = rowUtente;
            Device.BeginInvokeOnMainThread(() => App.Current.MainPage = new PageNavigatore());
            


        }
    }
}