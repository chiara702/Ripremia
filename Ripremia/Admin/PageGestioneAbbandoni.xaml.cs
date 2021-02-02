using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
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
            Comuni = ElencoComuni(App.DataRowUtente["AdminSuperUserCode"].ToString());
            foreach (var x in Comuni) {
                PickerComune.Items.Add(x.Value);
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


        //private void PickerComune_SelectedIndexChanged(object sender, EventArgs e) {

        //}

        private void BtnIndietro_Clicked(object sender, EventArgs e) {
            Application.Current.MainPage = new PageAreaRiservata();
        }

        protected override bool OnBackButtonPressed() {
            BtnIndietro_Clicked(null, null);
            return true;
        }

        private void BtnMostraMappa_Tapped(object sender, EventArgs e) {

        }
    }
}