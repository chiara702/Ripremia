using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PagePresentazione : CarouselPage {
        public PagePresentazione() {
            InitializeComponent();
        }

        private async void BtnConfermaComune_Clicked(object sender, EventArgs e) {
            var ComuneInserito = Funzioni.Antinull(TxtComuneTari.Text).ToUpper();
            if (ComuneInserito == "T") TxtComuneTari.Text = "TERMOLI";
            if (ComuneInserito == "L") TxtComuneTari.Text = "LANCIANO";
            var Parchetto = new ClassApiParco();
            var rigoComune = Parchetto.EseguiQueryRow("Comune", "Upper(Nome)='" + Funzioni.AntiAp(ComuneInserito) + "'");
            if (Parchetto.LastError == true) {
                await DisplayAlert("Attenzione", "Errore connessione!", "ok");
                return;
            }
            if (rigoComune == null) {
                await DisplayAlert("Attenzione", "Non abbiamo trovato il comune nei nostri sistemi. Verifica il corretto inserimento e/o disponibilità di connessione.", "ok");
                return;
            }
            var rigoSuperUser = Parchetto.EseguiQueryRow("SuperUser", "Codice='" + rigoComune["CodiceSuperUser"].ToString() + "'");
            if (rigoSuperUser == null) {
                await DisplayAlert("Attenzione", "Nessun superuser assegnato al comune!", "OK");
                return;
            }
            await DisplayAlert("Perfetto", "Ora non ti resta che registrarti per accedere ai servizi!", "ok");
            Xamarin.Essentials.Preferences.Set("IdComune", int.Parse(rigoComune["Id"].ToString()));
            Xamarin.Essentials.Preferences.Set("CodiceSuperUser", rigoComune["CodiceSuperUser"].ToString());
            //Memorizzazione Logo SuperUser
            var http = new System.Net.Http.HttpClient();
            if ("" + rigoSuperUser["Logo"] != "") {
                try {
                    var ByteImage = await http.GetByteArrayAsync(rigoSuperUser["Logo"].ToString());
                    System.IO.File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "Logo.png", ByteImage);
                } catch (Exception) { }
            }
            //Memorizzazione Logo Comune
            if ("" + rigoComune["Logo"] != "") {
                try {
                    var ByteImage = await http.GetByteArrayAsync(rigoComune["Logo"].ToString());
                    System.IO.File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "LogoComune.png", ByteImage);
                } catch (Exception) { }
            }

            Application.Current.MainPage = new PageLogin();



        }


        private void BtnSalta_Clicked(object sender, EventArgs e) {
            this.CurrentPage = FinalPageComune;
        }
    }
}