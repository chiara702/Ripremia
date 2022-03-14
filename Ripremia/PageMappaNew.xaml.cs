using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.Maps;
using System.Data;
using System.Globalization;
using Xamarin.Essentials;

namespace EcoServiceApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageMappaNew : ContentPage {
        public PageMappaNew() {
            InitializeComponent();
            MenuTop.MenuLaterale = MenuLaterale;
        }
        protected async override void OnAppearing() {
            base.OnAppearing();
            try {
                await Task.Run(()=> {
                    MatricolePin.Clear();
                    CaricaPin();
                    CaricaPinEcocontrol();
                    Device.BeginInvokeOnMainThread(() => {
                        if (map1.Pins.Count > 0) {
                            var p = new Position(map1.Pins[0].Position.Latitude, map1.Pins[0].Position.Longitude);
                            var span = new MapSpan(p, 0.05, 0.05);
                            map1.MoveToRegion(span);
                        }
                    });
                });
                


                
            } catch (Exception e) {
                await DisplayAlert("", e.Message, "ok");
            }

        }

        public Location CoordinateFromString(String CoordinateString) {
            var posIsola = new Xamarin.Essentials.Location(Double.Parse(CoordinateString.Split(",")[0], new CultureInfo("en-US")), Double.Parse(CoordinateString.Split(",")[1], new CultureInfo("en-US")));
            //var dist = Xamarin.Essentials.Location.CalculateDistance(pos, posIsola, Xamarin.Essentials.DistanceUnits.Kilometers);
            return posIsola;
        }

        public void CaricaPinEcocontrol() {
            if ((Boolean)App.DataRowComune["ServizioStatoEcoisole"]==false) return;
            var ApiEco=new ClassApiEcoControl();
            var TableIsole=ApiEco.EseguiQuery($"Select Id,NomeBidone,Matricola,Indirizzo,LatLong From IsoleBidoni Where Paese='{Funzioni.AntiAp(App.DataRowComune["Nome"].ToString())}'");
            foreach (DataRow x in TableIsole.Rows) {
                if (MatricolePin.Contains(Funzioni.Antinull(x["Matricola"])) == true) continue;
                if (Funzioni.Antinull(x["LatLong"]).Trim() == "") continue;
                var pintmp = new Pin();
                var LocationTmp = CoordinateFromString(x["LatLong"].ToString());
                pintmp.Position = new Position(LocationTmp.Latitude, LocationTmp.Longitude);  //new Position(double.Parse(x["Latitudine"].ToString()), double.Parse(x["Longitudine"].ToString()));
                pintmp.Label = x["NomeBidone"].ToString();
                MatricolePin.Add(Funzioni.Antinull(x["Matricola"]));

                pintmp.Type = PinType.Place;
                var rigoPoint = x;
                pintmp.MarkerClicked += (s, e) => {
                    var tmpPage = new Livelli.PageLivelloIsola((int)x["Id"]);
                    Navigation.PushAsync(tmpPage);
                };
                map1.Pins.Add(pintmp);
            }
        }


        public DataTable TablePoint=null;
        public List<String> MatricolePin=new List<String>();
        public void CaricaPin(Boolean MostraGestore=false) {
            var parchetto = new ClassApiParco();
            String Query = "";
            if (MostraGestore == false) {
                Query="Select * From Point Where Comune=(Select Nome From Comune Where Id=" + App.DataRowUtente["IdComune"] + ")";
            }else {
                Query ="Select * From Point Where CodiceGestore=(Select CodiceSuperUser From Comune Where Id=" + App.DataRowUtente["IdComune"] + ")";
            }
            TablePoint = parchetto.EseguiQuery(Query);
            map1.Pins.Clear();
            if (TablePoint == null) return;
            foreach (DataRow x in TablePoint.Rows) {
                var pintmp = new Pin();
                pintmp.Position = new Position(double.Parse(x["Latitudine"].ToString()), double.Parse(x["Longitudine"].ToString()));
                pintmp.Label = x["Etichetta"].ToString();
                MatricolePin.Add(Funzioni.Antinull(x["Matricola"]));
                
                pintmp.Type = PinType.Place;
                var rigoPoint = x;
                pintmp.MarkerClicked += async (s, e) => {
                   _ = FrmDettagli.TranslateTo(-450, 0, 1, Easing.Linear);
                    FrmDettagli.IsVisible = true;
                    _ = StkIconservice.TranslateTo(-550, 0, 1, Easing.Linear);
                    _ = FrmDettagli.TranslateTo(0, 0, 600, Easing.Linear);                    
                    _ =  StkIconservice.TranslateTo(0, 0, 600, Easing.Linear);
                    LblDenominazione.Text = rigoPoint["Etichetta"].ToString();
                    LblIndirizzo.Text = rigoPoint["Via"].ToString() + ", " + rigoPoint["Comune"].ToString() + " (" + rigoPoint["Provincia"].ToString() + ")";
                   
                    if ((Boolean)rigoPoint["FlagH2O"]==true) ImgH2O.IsVisible = true; else ImgH2O.IsVisible = false;
                    if ((Boolean)rigoPoint["FlagSoap"] == true) ImgSoap.IsVisible = true; else ImgSoap.IsVisible = false;
                    if ((Boolean)rigoPoint["FlagSacchetti"] == true) ImgSacchetti.IsVisible = true; else ImgSacchetti.IsVisible = false;
                    if ((Boolean)rigoPoint["FlagPet"] == true) ImgPet.IsVisible = true; else ImgPet.IsVisible = false;
                    if ((Boolean)rigoPoint["FlagOil"] == true) ImgOil.IsVisible = true; else ImgOil.IsVisible = false;    
                    if ((Boolean)rigoPoint["FlagVetro"] == true) ImgVetro.IsVisible = true; else ImgVetro.IsVisible = false;
                    if ((Boolean)rigoPoint["FlagBorracce"] == true) ImgBorracce.IsVisible = true; else ImgBorracce.IsVisible = false;
                    if ((Boolean)rigoPoint["FlagEbike"] == true) ImgEbike.IsVisible = true; else ImgEbike.IsVisible = false;
                    if ((Boolean)rigoPoint["FlagRicaricaCell"] == true) ImgRicaricaCell.IsVisible = true; else ImgRicaricaCell.IsVisible = false;
                    if ((Boolean)rigoPoint["FlagWifiFree"] == true) ImgWifiFree.IsVisible = true; else ImgWifiFree.IsVisible = false;
                    if ((Boolean)rigoPoint["FlagDog"] == true) ImgDog.IsVisible = true; else ImgDog.IsVisible = false;

                };
                map1.Pins.Add(pintmp);

            }
        }



        int ShowUnshow = 0;
        private void BtnShowAllPoint_Clicked(object sender, EventArgs e) {
            FrmDettagli.IsVisible = false;
            BtnShowAllPoint.IsVisible = false;
            if (ShowUnshow == 0) {
                MatricolePin.Clear();
                CaricaPin(true);
                CaricaPinEcocontrol();
                ShowUnshow = 1;
                BtnShowAllPoint.Source = "ShowLess";
                BtnShowAllPoint.IsVisible = true;
                try{
                    var p = new Position(map1.Pins[0].Position.Latitude, map1.Pins[0].Position.Longitude);
                    var span = new MapSpan(p, 0.25, 0.25);
                    map1.MoveToRegion(span);
                }
                catch (Exception) { }
            } else {
                MatricolePin.Clear();
                CaricaPin(false);
                CaricaPinEcocontrol();
                ShowUnshow = 0;
                BtnShowAllPoint.Source = "ShowAll";
                BtnShowAllPoint.IsVisible = true;
                try
                {
                    var p = new Position(map1.Pins[0].Position.Latitude, map1.Pins[0].Position.Longitude);
                    var span = new MapSpan(p, 0.05, 0.05);
                    map1.MoveToRegion(span);
                }
                catch (Exception) { }
            }

        }

        private async void BtnInfoservice_Clicked(object sender, EventArgs e) {
            FrmDettagli.IsVisible = false;
            _ = await FrmInfoservice.FadeTo(0, 1);
            FrmInfoservice.IsVisible = true;
            _ = await FrmInfoservice.FadeTo(1, 800);
       
        }

        private async void TapInfoservice_Tapped(object sender, EventArgs e) {
            _ = await FrmInfoservice.FadeTo(0, 500);
            FrmInfoservice.IsVisible = false;
            _ = await FrmDettagli.FadeTo(0, 1);
            FrmDettagli.IsVisible = true;
            _ = FrmDettagli.FadeTo(1, 800);

        }

        
    }
}