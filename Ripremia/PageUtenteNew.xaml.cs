using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace EcoServiceApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageUtenteNew : ContentPage {

        protected override bool OnBackButtonPressed()
        {
            return true;
            //return base.OnBackButtonPressed();
        }

        public PageUtenteNew() {
            InitializeComponent();
            LblUtente.Text = "Ciao, " + App.DataRowUtente["Nome"].ToString() + "!";
            LblCodFamiglia.Text = App.DataRowUtente["CodiceFamiglia"].ToString();
            LblNomeUtente.Text = App.DataRowUtente["Nome"].ToString() + " " + App.DataRowUtente["Cognome"].ToString();
            LblEmailUtente.Text = App.DataRowUtente["Email"].ToString();
            LblComuneTari.Text = App.DataRowComune["Nome"].ToString();
        }
        protected override void OnAppearing() {
            base.OnAppearing();
            Task.Run(() =>RiempiDati());
            Device.StartTimer(TimeSpan.FromSeconds(20), () => {
                RiempiDati();
                return true;
            });
            if ((Boolean)App.DataRowComune["ServizioRipremia"]==false) {
                FramePunteggioFamiglia.IsVisible=false;
            }
            if ((Boolean)App.DataRowComune["ServizioRitiro"]==false) {
                FrameVisualizzaPrenotazioni.IsVisible=false;
            }
        }

        
        public void RiempiDati() {
            try {
                var ecocontrol = new ClassApiEcoControl();
                int Qta = Convert.ToInt32(ecocontrol.EseguiCommand("Select Qta From MonetaVirtuale Where QrCode='" + App.DataRowUtente["CodiceMonetaVirtuale"].ToString() + "'"));
                Device.BeginInvokeOnMainThread(() => {
                    LblPunteggioFamiglia.Text = Qta.ToString();
                });

            } catch (Exception e) {
                DisplayAlert("Errore", "Errore recupero statistiche " + e.Message, "OK");
            }
        }

        private void BtnInfoUser_Clicked(object sender, EventArgs e) {
            DisplayAlert("", "Questa è la tua pagina personale, qui potrai visionare i tuoi dati e il tuo punteggio, modificare il codice famiglia o generarne uno nuovo accedendo alla sezione modifica i tuoi dati personali.", "ok");
        }

        private async void ImgMenu_Tapped(object sender, EventArgs e) {
            MenuLaterale.IsVisible = true;
            await MenuLaterale.Mostra();
        }

        private void BtnModificaDati_Clicked(object sender, EventArgs e) {
            //Application.Current.MainPage = new PageModificaDati();
            Application.Current.MainPage = new PageModificaDati();
            //Navigation.PushAsync (new PageModificaDati());

        }


        private void BtnVisualizzaPrenotazioni_Tapped(object sender, EventArgs e)
        {
            if (((Boolean)App.DataRowComune["ServizioRitiro"]) == false) {
                DisplayAlert("Servizio non disponibile", "Ci scusiamo, ma per il suo comune questo servizio non è attivo.", "OK");
                return;
            }

            //Application.Current.MainPage = new PagePrenotazioni();
            //Navigation.PushAsync(new PagePrenotazioni());
            Application.Current.MainPage = new PagePrenotazioni();
        }

        private void BtnShowPrivacy_Clicked(object sender, EventArgs e) {
            Xamarin.Essentials.Browser.OpenAsync("http://www.ripremia.com/PrivacyRipremia.pdf");
        }

        private void BtnLogout_Clicked(object sender, EventArgs e) {
            Xamarin.Essentials.Preferences.Set("Loggato", false);
            Xamarin.Essentials.Preferences.Set("Email", "");
            //UtenteDatiMemoria.AzzeraTimeUpdate();
            Application.Current.MainPage = new PagePresentazione();
        }
    }
}