using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageConfermaEmail : ContentPage {
        public PageConfermaEmail() {
            InitializeComponent();
            BindingContext = this;

            LblUtente.Text = "Ciao, " + App.DataRowUtente["Nome"].ToString() + "!";
        }

        private string Antinull(object input) {
            if (input == null) return ""; else return input.ToString();
        }

        private void BtnAccedi_Clicked(object sender, EventArgs e) {


            

            if (Antinull(TxtConfermaEmail.Text) == "") {
                DisplayAlert("Codice non valido", "Il codice inserito non è corretto! Immettilo nuovamente.", "ok");
                return;
            }



            if (TxtConfermaEmail.Text == App.DataRowUtente["CodConferma"].ToString()) {
                Application.Current.MainPage = new PageNavigatore();
                var FunzDb = new ClassApiParco();
                var Par = FunzDb.GetParam();
                Par.AddParameterString("ConfermaEmail", "1");
                FunzDb.EseguiUpdate("Utente", long.Parse(App.DataRowUtente["Id"].ToString()), Par);
                if (FunzDb.LastError == true) {
                    DisplayAlert("Errore", "Errore: " + FunzDb.LastErrorDescrizione, "OK");
                    return;
                }
                Xamarin.Essentials.Preferences.Set("Loggato", true);
            } else {
                DisplayAlert("Codice non valido", "Inserisci un codice di conferma valido", "ok");
            }


        }
        protected override bool OnBackButtonPressed() {
            Application.Current.MainPage = new PageLogin();
            return true;
        }

        private void BtnInviaConferma_Clicked(object sender, EventArgs e) {
            Funzioni.SendEmail(App.DataRowUtente["Email"].ToString(), "", "RIPREMIA - Conferma la tua e-mail per accedere ai servizi", "Benvenuto, come ultimo passo non ti resta che inserire il seguente numero all'interno dell'app RIPREMIA per confermare la tua email.\n " + App.DataRowUtente["CodConferma"].ToString());
            DisplayAlert("E-mail inviata!", "Controlla nella tua casella di posta " + App.DataRowUtente["Email"].ToString() + "\nSe non trovi nessuna e-mail verifica che non sia finita tra gli SPAM", "OK");

        }
    }
}