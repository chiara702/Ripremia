using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageModificaDati : ContentPage {
        private ClassApiParco Parchetto = new ClassApiParco();
        private DataRow RowUtente = null;

        protected override bool OnBackButtonPressed() {
            BtnAnnulla_Clicked(null, null);
            return true;
            //return base.OnBackButtonPressed();
        }
        public PageModificaDati() {
            InitializeComponent();
            MenuTop.MenuLaterale = MenuLaterale;
            BindingContext = this;

            
            RowUtente=Parchetto.EseguiQueryRow("Utente", int.Parse(App.DataRowUtente["Id"].ToString()));
            if (Parchetto.LastError == true) {
                DisplayAlert("Errore", "Connessione internet non disponibile!", "OK");
                Application.Current.MainPage = new PageLogin();
                return;
            }

            TxtNome.Text = RowUtente["Nome"].ToString() ;
            TxtCognome.Text = RowUtente["Cognome"].ToString();
            TxtEmail.Text = RowUtente["Email"].ToString();
            TxtCodFiscale.Text = RowUtente["CodiceFiscale"].ToString();
            TxtPassword.Text = RowUtente["Password"].ToString();
            txtCodFamiglia.Text = RowUtente["CodiceFamiglia"].ToString();
            //Task.Run(() => RiempiDati());

           

        }

        

        private void BtnShowPass_Clicked(object sender, EventArgs e) {
            TxtPassword.IsPassword = !TxtPassword.IsPassword;
        }

        private void CheckPrivacy_CheckedChanged(object sender, CheckedChangedEventArgs e) {
            if (CheckPrivacy.IsChecked == true) {
                BtnConfermaModifiche.BackgroundColor = Color.FromHex("#7ca137");
                BtnConfermaModifiche.IsEnabled = true;
            } else {
                BtnConfermaModifiche.IsEnabled = false;
                BtnConfermaModifiche.BackgroundColor = Color.DarkGray;

            }

        }
        private void BtnInfoCodice_Clicked(object sender, EventArgs e) {
            DisplayAlert("Cos'è il codice famiglia?", "Ogni nucleo familiare è identificato da un codice famiglia composto da 6 caratteri. Se sei in possesso di un codice famiglia inseriscilo al momento della registrazione, altrimenti te ne verrà assegnato uno di default che gli altri componenti potranno inserire al momento della registrazione.Potrai cambiare in un secondo momento il codice famiglia accedendo alla tua pagina personale.", "Capito!");

        }


        private async void ImgMenu_Tapped(object sender, EventArgs e) {
            MenuLaterale.IsVisible = true;
            await MenuLaterale.Mostra();

        }

        private void TapSicuro_Tapped(object sender, EventArgs e) {

        }

        private void BtnCancella_Clicked(object sender, EventArgs e) {
            StkAttenzione.IsVisible = false;
            FrmOpacity.IsVisible = false;
        }
        private void BtnOk_Clicked(object sender, EventArgs e) {
            //Generazione Codice Famiglia
            var CodiceFamiglia = RandomString(8);
            for (var x = 0; x <= 10; x++) {
                if (Parchetto.EseguiQueryRow("Utente", "CodiceFamiglia='" + CodiceFamiglia + "'") == null) break;
                CodiceFamiglia = RandomString(8);
            }
            txtCodFamiglia.Text = CodiceFamiglia;
            //Da spostare su Conferma definitiva
            var server = new ClassApiEcoControl();
            var RitMV = server.CreaQRCodeMonetaVirtuale(Parchetto.EseguiCommand("Select Nome From Comune Where Id=" + RowUtente["IdComune"].ToString()).ToString(), "", 0, RowUtente["CodiceFiscale"].ToString());
            if (RitMV.ErroreString != "") {
                DisplayAlert("Errore", "Errore generazione codice. Verificare connettività!", "OK");
                return;
            }
            RowUtente["CodiceMonetaVirtuale"] = RitMV.QrCode;
            StkAttenzione.IsVisible = false;
            FrmOpacity.IsVisible = false;
        }

        private void BtnGeneraNew_Clicked(object sender, EventArgs e) {
            StkAttenzione.IsVisible = true;
            FrmOpacity.IsVisible = true;
            
        }

        private void BtnAnnulla_Clicked(object sender, EventArgs e) {
            var Page = new PageNavigatore();
            Page.CurrentPage = Page.Children[4];
            Application.Current.MainPage = Page;
            
        }

        private void BtnConfermaModifiche_Clicked(object sender, EventArgs e) {
            var FunzDb = new ClassApiParco();
            var IdComune = Xamarin.Essentials.Preferences.Get("IdComune", 0);

            AlertCodFamiglia.IsVisible = false;
            AlertCodFisc.IsVisible = false;
            AlertCognome.IsVisible = false;
            AlertEmail.IsVisible = false;
            AlertNome.IsVisible = false;
            AlertPassword.IsVisible = false;
            AlertPrivacy.IsVisible = false;

            var Err1 = false;
            //if (Funzioni.VerificaCodiceFiscale(TxtCodFiscale.Text) == false) {
            //    TxtCodFiscale.TextColor = Color.Red;
            //    AlertCodFisc.IsVisible = true;
            //    Err1 = true;
            //}
            if (Funzioni.Antinull(TxtNome.Text).Length < 2) {
                AlertNome.IsVisible = true;
                Err1 = true;
            }
            if (Funzioni.Antinull(TxtCognome.Text).Length < 2) {
                AlertCognome.IsVisible = true;
                Err1 = true;
            }
            //if (Funzioni.IsValidEmail(Funzioni.Antinull(TxtEmail.Text)) == false) {
            //    AlertEmail.Text = "Inserire una e-mail valida!";
            //    AlertEmail.IsVisible = true;
            //    Err1 = true;
            //}
            if (Err1 == true) return;

            var Parchetto = new ClassApiParco();


            var row = Parchetto.EseguiQueryRow("Utente", "Email='" + Funzioni.Antinull(TxtEmail.Text) + "'");
            if (row != null) {
                AlertEmail.IsVisible = true;
                AlertEmail.Text = "E-mail già presente nei nostri sistemi.";
                Err1 = true;
            }

            if (Funzioni.Antinull(txtCodFamiglia.Text) != "") {
                var rowCod = Parchetto.EseguiQueryRow("Utente", "CodiceFamiglia='" + txtCodFamiglia.Text + "'");
                if (rowCod == null) {
                    AlertCodFamiglia.IsVisible = true;
                    Err1 = true;
                }
            }



            if (Funzioni.VerificaPassword(Funzioni.Antinull(TxtPassword.Text)) != 0) {
                AlertPassword.IsVisible = true;
                var ret = Funzioni.VerificaPassword(Funzioni.Antinull(TxtPassword.Text));
                if (ret == -1) AlertPassword.Text = "Inserisci una password valida di almeno 8 caratteri, un numero, una lettera maiuscola e un carattere speciale ($,#,%,..)!";
                if (ret == -2) AlertPassword.Text = "La password deve contenere un carattere maiuscolo";
                if (ret == -3) AlertPassword.Text = "La password deve contenere un carattere minuscolo";
                if (ret == -4) AlertPassword.Text = "La password deve contenere almeno un numero";
                if (ret == -5) AlertPassword.Text = "La password deve contenere almeno un carattere speciale tra: !,@,#,$,%,*,.,-,_";
                Err1 = true;
            }
            if (Err1 == true) return;

            String QrcodeMonetaVirtuale = "";

            if (Funzioni.Antinull(txtCodFamiglia.Text) == "") {
                //Generazione Codice Famiglia
                string CodiceFamiglia = RandomString(8);
                for (var x = 0; x <= 10; x++) {
                    if (Parchetto.EseguiQueryRow("Utente", "CodiceFamiglia='" + CodiceFamiglia + "'") == null) break;
                    CodiceFamiglia = RandomString(8);
                }
                txtCodFamiglia.Text = CodiceFamiglia;
                var server = new ClassApiEcoControl();
                var RitMV = server.CreaQRCodeMonetaVirtuale(FunzDb.EseguiCommand("Select Nome From Comune Where Id=" + IdComune).ToString(), "", 0, TxtCodFiscale.Text);
                if (RitMV.ErroreString != "") {
                    DisplayAlert("Errore", "Errore generazione codice. Verificare connettività!", "OK");
                    return;
                }
                QrcodeMonetaVirtuale = RitMV.QrCode;
            } else {
                if (Parchetto.EseguiQueryRow("Utente", "CodiceFamiglia='" + Funzioni.AntiAp(txtCodFamiglia.Text) + "'") == null) {
                    AlertCodFamiglia.IsVisible = true;
                    Err1 = true;
                    return;
                }
                QrcodeMonetaVirtuale = Funzioni.Antinull(FunzDb.EseguiCommand("Select CodiceMonetaVirtuale From Utente Where CodiceFamiglia='" + Funzioni.AntiAp(txtCodFamiglia.Text) + "'"));
                if (QrcodeMonetaVirtuale == "") {
                    DisplayAlert("Errore", "Errore nel trovare QrCode della famiglia. Verificare connettività!", "OK");
                    return;
                }

            }

            if (Err1 == true) return;




            var CodConferma = new Random().Next(100000, 999999);//random



            var Par = FunzDb.GetParam();
            Par.AddParameterString("Nome", TxtNome.Text);
            Par.AddParameterString("Cognome", TxtCognome.Text);
            Par.AddParameterString("CodiceFiscale", TxtCodFiscale.Text);
            Par.AddParameterString("Email", TxtEmail.Text.ToLower());
            Par.AddParameterString("Password", TxtPassword.Text);
            Par.AddParameterString("CodiceFamiglia", txtCodFamiglia.Text);
            Par.AddParameterInteger("LetturaPrivacy", CheckPrivacy.IsChecked == true ? 1 : 0);
            Par.AddParameterInteger("ConsensoTerzeParti", CheckTrattamento.IsChecked == true ? 1 : 0);
            Par.AddParameterInteger("IdComune", IdComune);
            Par.AddParameterString("CodConferma", CodConferma.ToString()); //random
            Par.AddParameterString("CodiceMonetaVirtuale", QrcodeMonetaVirtuale);
            Par.AddParameterObject("DataRegistrazione", System.DateTime.Now.ToString());
            var err = FunzDb.EseguiInsert("Utente", Par).ToString();
            if (err == null) {
                DisplayAlert("Errore", "Errore durante la registrazione!", "OK");
                return;
            }
            DisplayAlert("Registrazione effettuata con successo!", "Ultimo passo!\n Ora non ti resta che effettuare il primo accesso ed inserire il numero di conferma che ti abbiamo mandato all'indirizzo e-mail!", "Ok");

            Funzioni.SendEmail(TxtEmail.Text, "noreply@ecoserviceapp.it", "Conferma la tua e-mail per accedere ai servizi", "Benvenuto, come ultimo passo non ti resta che inserire il seguente numero all'interno dell'EcoService APP per confermare la tua email.\n " + CodConferma);
            Funzioni.SendEmail(TxtEmail.Text, "chiara702@gmail.com", "Conferma la tua e-mail per accedere ai servizi", "Benvenuto, come ultimo passo non ti resta che inserire il seguente numero all'interno dell'EcoService APP per confermare la tua email.\n " + CodConferma);

            Xamarin.Essentials.Preferences.Set("Loggato", false);
            Application.Current.MainPage = new PageLogin();



        }
       
        private static Random random = new Random();
        public static string RandomString(int length) {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void txtCodFamiglia_Focused(object sender, FocusEventArgs e) {

        }
    }
}