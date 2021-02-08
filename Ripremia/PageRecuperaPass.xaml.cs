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
                Funzioni.SendEmail(row["Email"].ToString(), "ripremianoreply@gmail.com", "RECUPERO PASSWORD ECOSERVICE APP", "Abbiamo recuperato i tuoi dati!" + "\nPassword: " + row["Password"].ToString() + "\n" + "Ora puoi inserire i tuoi dati per accedere.");
                DisplayAlert("", "Ti abbiamo inviato la password per e-mail.\nSe non la trovi controlla tra gli spam.", "ok");
            } else {
                DisplayAlert("Utente non valido", "Registrati per accedere ai servizi oppure verifica che i dati siano corretti.", "ok");
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
            _ = DisplayAlert("", "Invia una e-mail all'indirizzo di posta ripremianoreply@gmail.com per ricevere supporto tecnico.", "ok");
            return;
        }
    }
}