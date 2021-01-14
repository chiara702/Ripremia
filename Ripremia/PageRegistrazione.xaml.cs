using System;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;





namespace EcoServiceApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageRegistrazione : ContentPage {
        public PageRegistrazione() {
            InitializeComponent();
        }


        protected override bool OnBackButtonPressed() {
            Application.Current.MainPage = new PageLogin();
            return true;
        }

        private void BtnRegistrati_Clicked(object sender, EventArgs e) {
            TxtNome.Text = Funzioni.Antinull(TxtNome.Text).Trim();
            TxtCognome.Text = Funzioni.Antinull(TxtCognome.Text).Trim();
            txtCodFamiglia.Text = Funzioni.Antinull(txtCodFamiglia.Text).Trim();
            TxtCodFiscale.Text = Funzioni.Antinull(TxtCodFiscale.Text).Trim();
            TxtEmail.Text = Funzioni.Antinull(TxtEmail.Text).Trim();
            TxtPassword.Text = Funzioni.Antinull(TxtPassword.Text).Trim();
           
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
            if (Funzioni.VerificaCodiceFiscale(TxtCodFiscale.Text) == false) {
                TxtCodFiscale.TextColor = Color.Red;
                AlertCodFisc.IsVisible = true;
                Err1 = true;
            }
            if (Funzioni.Antinull(TxtNome.Text).Length < 2) {
                AlertNome.IsVisible = true;
                Err1 = true;
            }
            if (Funzioni.Antinull(TxtCognome.Text).Length < 2) {
                AlertCognome.IsVisible = true;
                Err1 = true;
            }
            if (Funzioni.IsValidEmail(Funzioni.Antinull(TxtEmail.Text)) == false) {
                AlertEmail.Text = "Inserire una e-mail valida!";
                AlertEmail.IsVisible = true;
                Err1 = true;
            }
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

            String QrcodeMonetaVirtuale="";

            if (Funzioni.Antinull(txtCodFamiglia.Text) == "") {
                //Generazione Codice Famiglia
                var CodiceFamiglia = RandomString(8);
                for (var x = 0; x <= 10; x++) {
                    if (Parchetto.EseguiQueryRow("Utente", "CodiceFamiglia='" + CodiceFamiglia + "'") == null) break;
                    CodiceFamiglia = RandomString(8);
                }
                txtCodFamiglia.Text=CodiceFamiglia;
                var server = new ClassApiEcoControl();
                var NomeComune = FunzDb.EseguiCommand("Select Nome From Comune Where Id=" + IdComune).ToString();
                var RitMV=server.CreaQRCodeMonetaVirtuale(NomeComune, "", 0, TxtCodFiscale.Text);
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
            Par.AddParameterString("Nome", TxtNome.Text.ToUpper());
            Par.AddParameterString("Cognome", TxtCognome.Text.ToUpper());
            Par.AddParameterString("CodiceFiscale", TxtCodFiscale.Text.ToUpper());
            Par.AddParameterString("Email", TxtEmail.Text.ToLower());
            Par.AddParameterString("Password", TxtPassword.Text.Trim());
            Par.AddParameterString("CodiceFamiglia", txtCodFamiglia.Text.ToUpper());
            Par.AddParameterInteger("LetturaPrivacy", CheckPrivacy.IsChecked==true ? 1 : 0);
            Par.AddParameterInteger("ConsensoTerzeParti", CheckTrattamento.IsChecked == true ? 1 : 0);
            Par.AddParameterInteger("IdComune", IdComune);
            Par.AddParameterString("CodConferma", CodConferma.ToString()); //random
            Par.AddParameterString("CodiceMonetaVirtuale", QrcodeMonetaVirtuale);
            Par.AddParameterObject("DataRegistrazione", System.DateTime.Now);
            FunzDb.EseguiInsert("Utente", Par);
            if (FunzDb.LastError == true) {
                DisplayAlert("Errore", "Errore durante la registrazione!", "OK");
                return;
            }
            DisplayAlert("Registrazione effettuata con successo!","Ultimo passo!\nOra non ti resta che effettuare il primo accesso ed inserire il numero di conferma che ti abbiamo mandato all'indirizzo e-mail!", "Ok");
            
            Funzioni.SendEmail(TxtEmail.Text, "ripremianoreply@gmail.com", "Conferma la tua e-mail per accedere ai servizi", "Benvenuto, come ultimo passo non ti resta che inserire il seguente numero all'interno dell'EcoService APP per confermare la tua email.\n " + CodConferma);

            var Page = new PageLogin();
            Page.SetEmailAndPassword(TxtEmail.Text, TxtPassword.Text);
            Application.Current.MainPage = Page;
            


        }
        private static Random random = new Random();
        public static string RandomString(int length) {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void BtnTrattDati_Clicked(object sender, EventArgs e) {

        }




        private void BtnPrivacy_Clicked(object sender, EventArgs e) {

        }

        private void TxtCodFiscale_TextChanged(object sender, TextChangedEventArgs e) {
            TxtCodFiscale.TextColor = Color.Gray;
        }

        private void CheckPrivacy_CheckedChanged(object sender, CheckedChangedEventArgs e) {
            if (CheckPrivacy.IsChecked == true) {
                BtnRegistrati.BackgroundColor = Color.FromHex("#7ca137");
                BtnRegistrati.IsEnabled = true;
            } else {
                BtnRegistrati.IsEnabled = false;
                BtnRegistrati.BackgroundColor = Color.DarkGray;

            }

        }

        private void BtnShowPass_Clicked(object sender, EventArgs e) {
            TxtPassword.IsPassword = !TxtPassword.IsPassword;
        }

        private void BtnInfoCodice_Clicked(object sender, EventArgs e) {
            DisplayAlert("Cos'è il codice famiglia?", "Ogni nucleo familiare è identificato da un codice famiglia composto da 6 caratteri. Se sei in possesso di un codice famiglia inseriscilo al momento della registrazione, altrimenti te ne verrà assegnato uno di default che gli altri componenti potranno inserire al momento della registrazione.Potrai cambiare in un secondo momento il codice famiglia accedendo alla tua pagina personale.", "Capito!");

        }

        private void BtnReturnLog_Clicked(object sender, EventArgs e) {
            Application.Current.MainPage = new PageLogin();
        }

    }
}