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
            MenuTop.MenuLaterale = MenuLaterale;
            BindingContext = this;
            LblUtente.Text = "Ciao, " + App.DataRowUtente["Nome"].ToString() + "!";
            LblCodFamiglia.Text = App.DataRowUtente["CodiceFamiglia"].ToString();
            var apiEcoControl = new ClassApiEcoControl();
            int Qta = Convert.ToInt32(apiEcoControl.EseguiCommand("Select Qta From MonetaVirtuale Where QrCode='" + Xamarin.Essentials.Preferences.Get("QrCodeNew", "") + "'"));
            LblPunteggioFamiglia.Text = Qta.ToString();
            LblNomeUtente.Text = App.DataRowUtente["Nome"].ToString() + " " + App.DataRowUtente["Cognome"].ToString();
            LblEmailUtente.Text = App.DataRowUtente["Email"].ToString();
            Task.Run(() => RiempiDati());
        }
        public void RiempiDati() {
            var parchetto = new ClassApiParco();
            var rowComune=parchetto.EseguiQueryRow("Comune", (int) App.DataRowUtente["IdComune"]);
            if (rowComune == null) return;
            Device.BeginInvokeOnMainThread(() => LblComuneTari.Text = rowComune["Nome"].ToString());

        }

        private void BtnInfoUser_Clicked(object sender, EventArgs e) {
            DisplayAlert("", "Questa è la tua pagina personale, qui potrai visionare i tuoi dati e il tuo punteggio, modificare il codice famiglia o generarne uno nuovo accedendo alla sezione modifica i tuoi dati personali.", "ok");
        }

        private async void ImgMenu_Tapped(object sender, EventArgs e) {
            MenuLaterale.IsVisible = true;
            await MenuLaterale.Mostra();

        }
        private void BtnModificaDati_Clicked(object sender, EventArgs e) {
            Application.Current.MainPage = new PageModificaDati();
            Navigation.PushAsync (new PageModificaDati());
        }


        private void BtnVisualizzaPrenotazioni_Tapped(object sender, EventArgs e)
        {
            Application.Current.MainPage = new PagePrenotazioni();
        }
    }
}