using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp.Livelli {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageListaIsole : ContentPage {
        private List<String> ListaIsolePreferite;
        public PageListaIsole() {
            InitializeComponent();
            RadioOrdineVicino.IsEnabled = false;
            Device.BeginInvokeOnMainThread(()=>CaricaIsole());
            ListaIsolePreferite=Preferences.Get("ListaIsolePreferite", "").Split(",",StringSplitOptions.RemoveEmptyEntries).ToList();
            

        }
        public List<ViewListaIsole> ListaViewIsole = new List<ViewListaIsole>();
        public async void CaricaIsole() {
            var Paese = "Bastia Umbra";
            if (Debugger.IsAttached==false) Paese=App.DataRowComune["Nome"].ToString();
            var api = new ClassApiEcoControl();
            var TableIs = await Task.Run(() => {
                return api.EseguiQuery($"Select * From IsoleBidoni Where Paese='{Funzioni.AntiAp(Paese)}'");
            });

            StackIsole.Children.Clear();
            var listaView=new List<ViewListaIsole>();
            foreach (DataRow x in TableIs.Rows) {
                var v = new ViewListaIsole();
                listaView.Add(v);
                v.LblNomeBidone.Text = x["NomeBidone"].ToString();
                v.LblIndirizzo.Text = Funzioni.Antinull(x["Indirizzo"]);
                v.Coordinate=Funzioni.Antinull(x["LatLong"]);
                if (ListaIsolePreferite.Contains(x["NomeBidone"])) v.SetPreferito();
                StackIsole.Children.Add(v);
                v.Clicked+=(s, e) => {
                    var page = new PageLivelloIsola((int)x["Id"]);
                    Navigation.PushAsync(page);
                };
                v.PreferedClicked+=(s, e) => {
                    if (v.Preferito==false) ListaIsolePreferite.Remove(v.LblNomeBidone.Text); else ListaIsolePreferite.Add(v.LblNomeBidone.Text);
                    Preferences.Set("ListaIsolePreferite", String.Join(",", ListaIsolePreferite));
                };
            }
            RiordinaIsole();
            //get localizzazione
            if (await CheckAndRequestLocationPermission()!=PermissionStatus.Granted) return;
            var loc = await Xamarin.Essentials.Geolocation.GetLocationAsync();
            
            var pos = new Xamarin.Essentials.Location(loc.Latitude, loc.Longitude);
            foreach (var z in listaView) {
                try {
                    var posIsola = new Xamarin.Essentials.Location(Double.Parse(z.Coordinate.Split(",")[0], new CultureInfo("en-US")), Double.Parse(z.Coordinate.Split(",")[1], new CultureInfo("en-US")));
                    var dist = Xamarin.Essentials.Location.CalculateDistance(pos, posIsola, Xamarin.Essentials.DistanceUnits.Kilometers);
                    z.DistanzaKm = dist;
                    if (dist > 1) {
                        z.LblDistance.Text = ((int)dist).ToString() + " Km";
                    } else {
                        z.LblDistance.Text = ((int)(dist*1000)).ToString() + " mt";
                    }
                } catch (Exception) { }
            }
            RadioOrdineVicino.IsEnabled = true;
            RiordinaIsole();
        }

        public void RiordinaIsole() {
            var listaControlli = new List<ViewListaIsole>();
            foreach (ViewListaIsole x in StackIsole.Children) {
                listaControlli.Add(x);
            }
            StackIsole.Children.Clear();
            //Add first preferiti
            foreach (ViewListaIsole x in listaControlli) {
                if (x.Preferito==true) StackIsole.Children.Add(x);
            }
            //Add Other
            if (RadioOrdineStd.IsChecked == true) {
                foreach (ViewListaIsole x in listaControlli.OrderBy<ViewListaIsole, int>(y => { return int.Parse(Regex.Match(y.LblNomeBidone.Text, @"\d+").Value); })) {
                    if (x.Preferito==false) StackIsole.Children.Add(x);
                }
            } else {
                foreach (ViewListaIsole x in listaControlli.OrderBy<ViewListaIsole, Double>(y => { return y.DistanzaKm; })) {
                    if (x.Preferito==false) StackIsole.Children.Add(x);
                }
            }

        }

        public async Task<PermissionStatus> CheckAndRequestLocationPermission() {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status == PermissionStatus.Granted)
                return status;

            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS) {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                // Prompt the user to turn on in settings
                // On iOS once a permission has been denied it may not be requested again from the application
                return status;
            }

            if (Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>()) {
                // Prompt the user with additional information as to why the permission is needed
            }

            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            return status;
        }

        private void RadioOrdineStd_CheckedChanged(object sender, CheckedChangedEventArgs e) {
            RiordinaIsole();
        }

        private void RadioOrdineVicino_CheckedChanged(object sender, CheckedChangedEventArgs e) {
            RiordinaIsole();
        }
    }
}