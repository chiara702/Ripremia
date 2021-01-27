﻿using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace EcoServiceApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageDenunciaAbbandono : ContentPage {
        public PageDenunciaAbbandono() {
            InitializeComponent();
        }

        private MediaFile file = null;
        private async void BtnScattaFoto_Clicked(object sender, EventArgs e) {
            try {

                await CrossMedia.Current.Initialize();
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported) {
                    await DisplayAlert("No Camera", ":( No camera available.", "OK");
                    return;
                }
                file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions {
                    Name = "Ritiro1.jpg",
                    CompressionQuality = 30,
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                    SaveToAlbum = true,

                });
                if (file == null) return;
                BtnScattaFoto.Source = ImageSource.FromStream(() => {
                    var stream = file.GetStream();
                    return stream;
                });
            } catch (Exception) { await DisplayAlert("Attenzione", "E' necessario fornire le autorizzazioni di acquisizione della fotocamera", "OK"); }
        }

        private TipoRifiutoSelezionato tipoRifiutoSelezionato = TipoRifiutoSelezionato.Nullo;
        private enum TipoRifiutoSelezionato {
            Nullo = 0,
            Verde,
            Ingombranti,
            Raee,
            NonIdentificato
        }

        private void BtnRifiuti_Clicked(object sender, EventArgs e) {
            BtnVerde.BorderColor = Color.Transparent;
            BtnIngombranti.BorderColor = Color.Transparent;
            BtnRaee.BorderColor = Color.Transparent;
            BtnNonIdentificato.BorderColor = Color.Transparent;
            ImageButton sender1 = (ImageButton)sender;
            sender1.BorderColor = Color.DarkRed;
            if (sender1 == BtnVerde) tipoRifiutoSelezionato = TipoRifiutoSelezionato.Verde;
            if (sender1 == BtnIngombranti) tipoRifiutoSelezionato = TipoRifiutoSelezionato.Ingombranti;
            if (sender1 == BtnRaee) tipoRifiutoSelezionato = TipoRifiutoSelezionato.Raee;
            if (sender1 == BtnNonIdentificato) tipoRifiutoSelezionato = TipoRifiutoSelezionato.NonIdentificato;
        }

        protected override bool OnBackButtonPressed() {
            var Page = new PageNavigatore();
            Page.CurrentPage = Page.Children[3];
            Application.Current.MainPage = Page;
            return true;
        }

        private Boolean positionOk = false;
        private String Localizzazione = "";

        private async void BtnInviaPosizione_Tapped(object sender, EventArgs e) {
            imgGeolocalizzazione.IsVisible = false;
            ActWaitLocalizzation.IsVisible = true;
            try {
                var request = new GeolocationRequest(GeolocationAccuracy.High);
                request.Timeout = new TimeSpan(0, 0, 5);
                var location = await Geolocation.GetLocationAsync(request);
                if (location != null) {
                    if (location.Accuracy > 40) { //localizzazione insufficiente
                        ActWaitLocalizzation.IsVisible = true;
                        await ActWaitLocalizzation.FadeTo(1, 250);
                        imgGeolocalizzazione.IsVisible = false;
                        FrameInviaPosizione.BackgroundColor = Color.FromRgb(237, 28, 36);
                        FrameIndirizzoManuale.IsVisible = true;
                    } else { //localizzazione sufficiente
                        ActWaitLocalizzation.IsVisible = false;

                        imgGeolocalizzazione.Source = "checkanimation.gif";
                        FrameInviaPosizione.BackgroundColor = Color.FromRgb(251, 208, 8);
                        _ = txtGeolocalizzazione.FadeTo(0, 250);
                        imgGeolocalizzazione.IsVisible = true;
                        _ = txtGeolocalizzazione.FadeTo(1, 250);
                        positionOk = true;
                        Localizzazione = location.Latitude.ToString().Replace(",", ".") + "," + location.Longitude.ToString().Replace(",", ".");
                    }
                }
            } catch (Exception ex) {
                await DisplayAlert("Errore", ex.Message, "OK");
                ActWaitLocalizzation.IsVisible = false;
                FrameInviaPosizione.BackgroundColor = Color.FromRgb(237, 28, 36);
                FrameInviaPosizione.IsVisible = false;
                FrameIndirizzoManuale.IsVisible = true;
            }
        }

        private void CheckPrivacy_CheckedChanged(object sender, CheckedChangedEventArgs e) {
            if (CheckPrivacy.IsChecked == true) {

                BtnPrenota.BackgroundColor = Color.FromHex("#7ca137");
                BtnPrenota.IsEnabled = true;
            } else {
                BtnPrenota.IsEnabled = false;
                BtnPrenota.BackgroundColor = Color.DarkGray;

            }

        }

       


        private void BtnIndietro_Clicked(object sender, EventArgs e) {
            var Page = new PageNavigatore();
            Page.CurrentPage = Page.Children[3];
            Application.Current.MainPage = Page;
        }


        private void BtnDenuncia_Tapped(object sender, EventArgs e) {
            if (CheckPrivacy.IsChecked == false) { DisplayAlert("Attenzione", "E' necessario accettare l'informativa sulla privacy", "OK"); return; }
            if (Localizzazione == "" & String.IsNullOrEmpty(TxtIndirizzoManuale.Text) == true) { DisplayAlert("Attenzione", "E' necessario inserire un indirizzo per il ritiro!", "OK"); return; }
            if (file == null) { DisplayAlert("Attenzione", "Inserisci una foto di ciò che dovremo ritirare !", "OK"); return; }
            if (tipoRifiutoSelezionato == 0) { DisplayAlert("Attenzione", "Non è stato selezionato il tipo rifiuto", "OK"); return; }
            var Parchetto = new ClassApiParco();
            var Par = Parchetto.GetParam();
            Par.AddParameterObject("DataCreazione", DateTime.Now);
            Par.AddParameterInteger("UtenteId", int.Parse(App.DataRowUtente["Id"].ToString()));
            Par.AddParameterString("TipoRifiuto", tipoRifiutoSelezionato.ToString());
            Par.AddParameterString("Note", "" + TxtInfoRitiro.Text);
            Par.AddParameterString("Geolocalizzazione", "" + Localizzazione);
            Par.AddParameterString("Indirizzo", "" + TxtIndirizzoManuale.Text);
            var StRead = new System.IO.BinaryReader(file.GetStream());
            var ByteFoto = StRead.ReadBytes(int.MaxValue);//int.MaxValue);
            Par.AddParameterObject("Foto", ByteFoto); //ToHex(ByteFoto)
            var IdRit = Parchetto.EseguiInsert("DenunciaAbbandono", Par);
            if (IdRit == null || IdRit is String) {
                DisplayAlert("Errore", "Errore invio richiesta!", "OK");
            } else {
                var Page = new PageNavigatore();
                Page.CurrentPage = Page.Children[3];
                DisplayAlert("Richiesta inviata correttamente", "Ti ringraziamo per la segnalazione provvederemo alla bonificazione dell'area!", "OK");
                Application.Current.MainPage = Page;
            }

        }

       
    }
}