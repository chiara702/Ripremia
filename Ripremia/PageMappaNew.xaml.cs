using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.Maps;
using System.Data;

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

                CaricaPin();
                /*var a = awai
                 * t Xamarin.Essentials.Geolocation.GetLastKnownLocationAsync();
                if (a != null) { 
                    var p = new Position(a.Latitude, a.Longitude);
                    var span = new MapSpan(p, 1, 1);
                    map1.MoveToRegion(span);
                }*/
                if (map1.Pins.Count > 0) {
                    var p = new Position(map1.Pins[0].Position.Latitude, map1.Pins[0].Position.Longitude);
                    var span = new MapSpan(p, 0.05, 0.05);
                    map1.MoveToRegion(span);
                }
            } catch (Exception e) {
                await DisplayAlert("", e.Message, "ok");
            }

        }

        public DataTable TablePoint=null;
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
                pintmp.Position = new Position(double.Parse(x["Latitudine"].ToString()), double.Parse( x["Longitudine"].ToString()));
                pintmp.Label = x["Etichetta"].ToString();
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
                    if (rigoPoint["FlagH2O"].ToString() == "1") ImgH2O.IsVisible = true; else ImgH2O.IsVisible = false;
                    if (rigoPoint["FlagSoap"].ToString() == "1") ImgSoap.IsVisible = true; else ImgSoap.IsVisible = false;
                    if (rigoPoint["FlagSacchetti"].ToString() == "1") ImgSacchetti.IsVisible = true; else ImgSacchetti.IsVisible = false;
                    if (rigoPoint["FlagOil"].ToString() == "1") ImgOil.IsVisible = true; else ImgOil.IsVisible = false;
                    if (rigoPoint["FlagPet"].ToString() == "1") ImgPet.IsVisible = true; else ImgPet.IsVisible = false;
                    if (rigoPoint["FlagBorracce"].ToString() == "1") ImgBorracce.IsVisible = true; else ImgBorracce.IsVisible = false;
                    if (rigoPoint["FlagEbike"].ToString() == "1") ImgEbike.IsVisible = true; else ImgEbike.IsVisible = false;
                    if (rigoPoint["FlagRicaricaCell"].ToString() == "1") ImgRicaricaCell.IsVisible = true; else ImgRicaricaCell.IsVisible = false;
                    if (rigoPoint["FlagWifiFree"].ToString() == "1") ImgWifiFree.IsVisible = true; else ImgWifiFree.IsVisible = false;
                 };
                map1.Pins.Add(pintmp);

            }
        }



        int ShowUnshow = 0;
        private void BtnShowAllPoint_Clicked(object sender, EventArgs e) {
            FrmDettagli.IsVisible = false;
            BtnShowAllPoint.IsVisible = false;
            if (ShowUnshow == 0) {
                CaricaPin(true);
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
                CaricaPin(false);
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