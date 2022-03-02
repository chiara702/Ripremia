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
            foreach (DataRow x in TableIs.Rows) {
                var v = new ViewListaIsole();
                v.LblNomeBidone.Text = x["NomeBidone"].ToString();
                v.LblIndirizzo.Text = Funzioni.Antinull(x["Indirizzo"]);
                StackIsole.Children.Add(v);
                v.Clicked+=(s, e) => {
                    var page = new PageLivelloIsola((int)x["Id"]);
                    Navigation.PushAsync(page);
                    //App.Current.MainPage = page;
                };
            }
      
        }
    }
}