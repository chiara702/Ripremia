using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp.Livelli {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageListaIsole : ContentPage {
        public PageListaIsole() {
            InitializeComponent();
            Device.BeginInvokeOnMainThread(()=>CaricaIsole());

        }
        public List<ViewListaIsole> ListaViewIsole = new List<ViewListaIsole>();
        public async void CaricaIsole() {
            var Paese = "Bastia Umbra";
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
                //v.LblDistance.Text = ..;
                StackIsole.Children.Add(v);
                v.Clicked+=(s, e) => {
                    var page = new PageLivelloIsola((int)x["Id"]);
                    Navigation.PushAsync(page);
                };
            }
            //get localizzazione
            var loc = await Xamarin.Essentials.Geolocation.GetLocationAsync();
            var pos = new Xamarin.Essentials.Location(loc.Latitude, loc.Longitude);
            foreach (var z in listaView) {
                try {
                    var posIsola = new Xamarin.Essentials.Location(Double.Parse(z.Coordinate.Split(",")[0].Replace(".",",")), Double.Parse(z.Coordinate.Split(",")[1].Replace(".",",")));
                    var dist = Xamarin.Essentials.Location.CalculateDistance(pos, posIsola, Xamarin.Essentials.DistanceUnits.Kilometers);
                z.LblDistance.Text = dist.ToString();
                } catch (Exception) { }
            }
        }
    }
}