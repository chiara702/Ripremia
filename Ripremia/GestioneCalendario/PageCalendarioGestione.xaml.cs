using EcoServiceApp.GestioneCalendario;
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
    public partial class PageCalendarioGestione : ContentPage {
        ClassApiParco Parchetto = new ClassApiParco();
        Dictionary<int, String> Comuni;

        public PageCalendarioGestione() {
            InitializeComponent();
            Task.Run(() => CaricaComuni());
        }
        private void CaricaComuni() {
            Comuni = ElencoComuni(App.DataRowUtente["AdminSuperUserCode"].ToString());
            Device.BeginInvokeOnMainThread(() => { 
                foreach (var x in Comuni) {
                    PickerComune.Items.Add(x.Value);
                }
            });
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

        private void BtnIndietro_Clicked(object sender, EventArgs e) {
            Application.Current.MainPage = new PageAreaRiservata();
        }


        protected override bool OnBackButtonPressed() {
            BtnIndietro_Clicked(null, null);
            return true;
        }

        private void PickerComune_SelectedIndexChanged(object sender, EventArgs e) {
            BtnShowImpostazioniCalendario.IsVisible = true;
        }

        private void BtnTipiRifiuti_Tapped(object sender, EventArgs e) {
            if (PickerComune.SelectedItem == null) return;
            var IdComune = Comuni.FirstOrDefault(x => x.Value == PickerComune.SelectedItem.ToString()).Key;
            Navigation.PushAsync(new PageCalendarioTipiRifiuti(IdComune));
        }

        private void BtnImpostazioni_Tapped(object sender, EventArgs e) {
            if (PickerComune.SelectedItem == null) return;
            var IdComune = Comuni.FirstOrDefault(x => x.Value == PickerComune.SelectedItem.ToString()).Key;
            Navigation.PushAsync(new PageCalendarioImposta(IdComune));
        }

        private void BtnEccezzioni_Tapped(object sender, EventArgs e) {
            if (PickerComune.SelectedItem == null) return;
            var IdComune = Comuni.FirstOrDefault(x => x.Value == PickerComune.SelectedItem.ToString()).Key;
            Navigation.PushAsync(new PageCalendarioEccezzioni(IdComune));
        }
    }
}