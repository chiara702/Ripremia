using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageLogin : ContentPage {
        public PageLogin() {
            InitializeComponent();
            var PathLogo = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "Logo.png";
            if (System.IO.File.Exists(PathLogo) == true) {
                ImgLogo.Source = ImageSource.FromFile(PathLogo);
            }
            Task.Run(() => Riempi());
            
            

        }

        public void SetEmailAndPassword(string email, string password) {
            TxtEmailLog.Text = email;
            TxtPassLog.Text = password;
        }

        private void Riempi() {
            var Parchetto = new ClassApiParco();
            var codiceSuperUser = Xamarin.Essentials.Preferences.Get("CodiceSuperUser","");
            Device.BeginInvokeOnMainThread(() => {
                var tmp = Parchetto.EseguiCommand("Select Denominazione From SuperUser Where Codice='" + Funzioni.AntiAp(codiceSuperUser) + "'");
                if (tmp == null) tmp = "";
                LblBenvenutiServizi.Text = "Benvenuti nei Servizi " + tmp.ToString()  + " accedi con le tue credenziali";
            });
        }


        private string Antinull(object input) {
            if (input == null) return ""; else return input.ToString();
        }



        private void BtnAccedi_Clicked(object sender, EventArgs e) {
            if (TxtEmailLog.Text == null) TxtEmailLog.Text = "";
            if (TxtPassLog.Text == null) TxtPassLog.Text = "";
            TxtEmailLog.Text = TxtEmailLog.Text.ToLower();

            var Parchetto = new ClassApiParco();

            var rowUtente = Parchetto.EseguiQueryRow("Utente", "Email='" + TxtEmailLog.Text + "' And Password='" + TxtPassLog.Text + "'");
            if (Parchetto.LastError == true) {
                Application.Current.MainPage = new PageOffLine();
                return;
            }            
            

            if (Antinull(TxtEmailLog.Text) == "" || (Antinull(TxtPassLog.Text) == "")) {
                DisplayAlert("Dati non validi", "Registrati per accedere ai servizi o inserisci i dati corretti", "ok");
                return;
            }
            if (rowUtente == null) {
                DisplayAlert("Utente non valido", "Registrati per accedere ai servizi oppure verifica i dati che siano corretti.", "ok");
                return;
            }
            App.DataRowUtente = rowUtente;
            Xamarin.Essentials.Preferences.Set("QrCodeNew", rowUtente["CodiceMonetaVirtuale"].ToString());
            Xamarin.Essentials.Preferences.Set("Email", TxtEmailLog.Text);
            Xamarin.Essentials.Preferences.Set("Password", TxtPassLog.Text);
            Task.Run(()=>App.InizializzaDatiApp()).Wait();
            if (App.DataRowUtente==null || App.DataRowSuperUser==null || App.DataRowComune == null) {
                DisplayAlert("Errore", "Errore connessione e recupero dati", "OK");
                return;
            }
            if ((bool)rowUtente["ConfermaEmail"] == true) {
                if ((bool)CheckRipremiaLight.IsChecked == true) {
                    Xamarin.Essentials.Preferences.Set("LoggatoLight", true);
                    Application.Current.MainPage = new PageHomeLight();
                } else { 
                Xamarin.Essentials.Preferences.Set("Loggato", true);
                Application.Current.MainPage=new PageNavigatore();
                }
            } else {
                Application.Current.MainPage = new PageConfermaEmail();
            }


        }

        private void BtnRecuperaPass_Clicked(object sender, EventArgs e) {
            Application.Current.MainPage = new PageRecuperaPass();
        }

        private void BtnRegistrazione2_Clicked(object sender, EventArgs e) {
            Application.Current.MainPage = new PageRegistrazione();
        }

        private void LinkRipremiaLight_Tapped(object sender, EventArgs e) {
            Application.Current.MainPage = new PageHomeLight();

        }

        private void CheckRipremiaLight_CheckedChanged(object sender, CheckedChangedEventArgs e) {

        }
    }
}