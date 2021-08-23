using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageRecuperaPass : ContentPage {
        public PageRecuperaPass() {
            InitializeComponent();
        }

        private void BtnRecupera_Clicked(object sender, EventArgs e) {
            if (Funzioni.IsValidEmail(TxtEmail.Text) == false ) {
                DisplayAlert("E-mail non presente nei nostri sistemi", "Verifica il corretto inserimento", "ok");
                return;
            }

            var FunzDb = new ClassApiParco();
            var row=FunzDb.EseguiQueryRow("Utente", "email='" + TxtEmail.Text.Replace("'", "") + "'");


            if (row != null) {
                var Email = Funzioni.Antinull(TxtEmail.Text);
                Funzioni.SendEmail(Email, "ripremiasupport@ecocontrolgsm.it", "RECUPERO PASSWORD RIPREMIA", "Clicca il link per resettare la password <a href='https://ecocontrolgsm.cloud/Ripremia/ResetPassword.aspx?Email=" + System.Web.HttpUtility.UrlEncode(Email) + "&Securcode=" + Email.Length * 9999 +  "'>Reset Password</a>");
                DisplayAlert("", "Ti abbiamo inviato un'e-mail con le istruzioni per resettare la password.\nSe non la trovi, controlla tra gli spam.", "ok");
                Application.Current.MainPage = new PageLogin();
            } else {
                DisplayAlert("E-mail non presente nei nostri sistemi", "Registrati per accedere ai servizi oppure verifica che l'e-mail inserita sia scritta correttamente.", "ok");
                return;
            }



        }

        protected override bool OnBackButtonPressed() {
            Application.Current.MainPage = new PageLogin();
            return true;

        }

        private void BtnReturnLog_Clicked(object sender, EventArgs e) {
            Application.Current.MainPage = new PageLogin();
        }

        private void BtnProblemiAccesso_Clicked(object sender, EventArgs e) {
            _ = DisplayAlert("", "Invia una e-mail all'indirizzo di posta ripremiasupport@ecocontrolgsm.it per ricevere supporto tecnico.", "ok");
            return;
        }
    }
}