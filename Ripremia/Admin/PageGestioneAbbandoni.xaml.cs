using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp.Admin {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageGestioneAbbandoni : ContentPage {

        ClassApiParco Parchetto = new ClassApiParco();
        Dictionary<int, String> Comuni;
        public PageGestioneAbbandoni() {
            InitializeComponent();
            Task.Run(() => CaricaComuni());
        }
        private void CaricaComuni() {
            if (Funzioni.Antinull(App.DataRowUtente["AdminSuperUserCode"]) != "") {
                Comuni = ElencoComuni(App.DataRowUtente["AdminSuperUserCode"].ToString());
                foreach (var x in Comuni) {
                    Device.BeginInvokeOnMainThread(() => PickerComune.Items.Add(x.Value));
                }
            }
            if ((int)App.DataRowUtente["AdminComuneId"] > 0) {
                Device.BeginInvokeOnMainThread(()=>PickerComune.Items.Add(Parchetto.EseguiCommand("Select Nome From Comune Where Id=" + App.DataRowUtente["AdminComuneId"]).ToString()));
            }
        }

        private Dictionary<int, String> ElencoComuni(String SuperUser) {
            var rit = new Dictionary<int, String>();
            var Table = Parchetto.EseguiQuery("Select Id,Nome From Comune Where CodiceSuperUser='" + SuperUser + "'");
            if (Parchetto.LastError == true) {
                DisplayAlert("Error", "Si è verificato un errore. Verifica connettività!", "OK");
                return null;
            }
            foreach (DataRow x in Table.Rows) {
                rit.Add(int.Parse(x["Id"].ToString()), x["Nome"].ToString());
            }
            return rit;
        }


        protected async override void OnAppearing() {
            base.OnAppearing();
            try {
                //if (map1.Pins.Count > 0) {
                //    var p = new Position(map1.Pins[0].Position.Latitude, map1.Pins[0].Position.Longitude);
                //    var span = new MapSpan(p, 0.02, 0.02);
                //    map1.MoveToRegion(span);
                //}
            } catch (Exception e) {
                await DisplayAlert("", e.Message, "ok");
            }

        }

        public DataTable TablePoint = null;
        private const int DistanzaMtRipetiPin= 50;
        private MemoryStream CaricaImageRifiuto(int Id) {
            var TableRow = Parchetto.EseguiQueryRow("DenunciaAbbandono", Id);
            var ByteImage = (byte[])TableRow["Foto"];
            var mem = new MemoryStream(ByteImage);
            return mem;
        }
        public void CaricaPin(int ComuneId) {
            var parchetto = new ClassApiParco();
            String WhereTipoRifiuto="1=1";
            if (RadioIcon0.IsChecked == true) WhereTipoRifiuto = "1=1";
            if (RadioIcon1.IsChecked == true) WhereTipoRifiuto = "TipoRifiuto='Raee'";
            if (RadioIcon2.IsChecked == true) WhereTipoRifiuto = "TipoRifiuto='Verde'";
            if (RadioIcon3.IsChecked == true) WhereTipoRifiuto = "TipoRifiuto='Ingombranti'";
            if (RadioIcon4.IsChecked == true) WhereTipoRifiuto = "TipoRifiuto='Non identificati'";
            String Query = "Select Id,DataCreazione,TipoRifiuto,Note,Geolocalizzazione,(Select CONCAT (Cognome,' ', Nome) From Utente Where Id=UtenteId) as Nome From DenunciaAbbandono Where Ritirato=false And UtenteId in (Select Id From Utente Where Idcomune=" + ComuneId + ") And " + WhereTipoRifiuto;
            TablePoint = parchetto.EseguiQuery(Query);
            map1.Pins.Clear();
            if (TablePoint == null) return;
            var ListaPoint = new List<Location>();
            foreach (DataRow x in TablePoint.Rows) {
                Location tmppoint=null;
                if (Funzioni.Antinull(x["Geolocalizzazione"]).Split(",").Length==2) {
                    var Latitudine = Convert.ToDouble(x["Geolocalizzazione"].ToString().Split(",")[0], System.Globalization.CultureInfo.InvariantCulture);
                    var Longitudine = Convert.ToDouble(x["Geolocalizzazione"].ToString().Split(",")[1], System.Globalization.CultureInfo.InvariantCulture);
                    var Salta = false;
                    foreach (var z in ListaPoint) {
                        if (Location.CalculateDistance(z, tmppoint, DistanceUnits.Kilometers) < DistanzaMtRipetiPin / 1000) Salta = true;
                    }
                    if (Salta==true) continue;
                    tmppoint = new Location(Latitudine, Longitudine);
                }
                if (tmppoint == null) continue;
                var pintmp = new Pin();
                pintmp.Position = new Position(tmppoint.Latitude,tmppoint.Longitude);
                pintmp.Label = x["TipoRifiuto"].ToString();
                pintmp.Type = PinType.Generic;
                var RigoPoint = x;
                pintmp.MarkerClicked += (s, e) => {
                    var memfoto=CaricaImageRifiuto((int) RigoPoint["Id"]);
                    FrmDettagli.IsVisible = true;
                    LblUtente.Text = RigoPoint["Nome"].ToString();
                    LblNote.Text = RigoPoint["Note"].ToString();
                    ImgAbbandono.Source = ImageSource.FromStream(()=> { return memfoto; });
                    BtnRitirato.TabIndex = (int)RigoPoint["Id"];
                };
                map1.Pins.Add(pintmp);
            }
            if (map1.Pins.Count > 0) {
                var p = new Position(map1.Pins[0].Position.Latitude, map1.Pins[0].Position.Longitude);
                var span = new MapSpan(p, 0.02, 0.02);
                map1.MoveToRegion(span);
                
                map1.IsShowingUser = true;
            }
        }




        private void BtnIndietro_Clicked(object sender, EventArgs e) {
            Application.Current.MainPage = new PageAreaRiservata();
        }

        protected override bool OnBackButtonPressed() {
            BtnIndietro_Clicked(null, null);
            return true;
        }

        private void BtnMostraMappa_Tapped(object sender, EventArgs e) {
            StkSelectComune.IsVisible = false;
            map1.IsVisible = true;
            FrmDettagli.IsVisible = false;
            if (PickerComune.SelectedItem.ToString() != "") {
                try {
                    CaricaPin(int.Parse(Parchetto.EseguiCommand("Select Id From Comune Where Nome='" + Funzioni.AntiAp(PickerComune.SelectedItem.ToString()) + "'").ToString()));
                    map1.IsVisible = true;
                } catch (Exception r) {
                    DisplayAlert("Errore", r.Message, "OK");
                }

            }
        }


        private void PickerComune_SelectedIndexChanged(object sender, EventArgs e) {
            StkShowAll.IsVisible = true;
            

        }

        
        private async void BtnRitirato_Clicked(object sender, EventArgs e) {
            if (await DisplayAlert("Ritiro", "Vuoi confermare l'avvenuto ritiro?", "SI", "NO") == true) {
                Parchetto.EseguiCommand("Update DenunciaAbbandono Set Ritirato=True Where Id=" + BtnRitirato.TabIndex);
                BtnMostraMappa_Tapped(null, null);
            }
        }
    }
}