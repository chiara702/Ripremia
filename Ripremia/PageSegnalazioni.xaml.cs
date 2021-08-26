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
    public partial class PageSegnalazioni : ContentPage {

        ClassApiParco Parchetto = new ClassApiParco();
        Dictionary<int, String> Point;


        public PageSegnalazioni() {
            InitializeComponent();
            Task.Run(() => CaricaPoint());
        }

        private void CaricaPoint() {
            Point = ElencoPoint(App.DataRowUtente["IdComune"].ToString());
            Device.BeginInvokeOnMainThread(() => {
                foreach (var x in Point) {
                    PickerPoint.Items.Add(x.Value);
                }
            });
        }

        private Dictionary<int, String> ElencoPoint(String ComunePoint) {
            var rit = new Dictionary<int, String>();
            var Table = Parchetto.EseguiQuery("Select Id, Nome From Point Where IdComune='" + ComunePoint + "'");
            if (Parchetto.LastError == true) {
                DisplayAlert("Error", "Si è verificato un errore. Verifica connettività!", "OK");
                return null;
            }
            foreach (DataRow x in Table.Rows) {
                rit.Add(int.Parse(x["Id"].ToString()), x["Nome"].ToString());
            }
            return rit;
        }




        private void BtnInviaSegnalazione_Clicked(object sender, EventArgs e) {
            if (PickerPoint.SelectedItem == null) {
                DisplayAlert("Attenzione", "Inserisci il punto che necessita di intervento", "OK");
                return;
            }
            
            var RiempiSegnalazioni = new ClassApiParco();
            var Par = RiempiSegnalazioni.GetParam();
            Par.AddParameterObject("Data", DateTime.Now);
            Par.AddParameterInteger("IdUtente", int.Parse(App.DataRowUtente["Id"].ToString()));
            Par.AddParameterInteger("IdComune", int.Parse(App.DataRowUtente["IdComune"].ToString()));              
            Par.AddParameterInteger("IdPoint", Point.FirstOrDefault(x => x.Value == PickerPoint.SelectedItem.ToString()).Key);
            Par.AddParameterString("Note", Funzioni.Antinull(TxtMessaggio.Text));
            if (RadioIcon0.IsChecked == true) Par.AddParameterString("Problema", RadioIcon0.Content.ToString());
            if (RadioIcon1.IsChecked == true) Par.AddParameterString("Problema", RadioIcon1.Content.ToString());
            if (RadioIcon2.IsChecked == true) Par.AddParameterString("Problema", RadioIcon2.Content.ToString());
            if (RadioIcon3.IsChecked == true) Par.AddParameterString("Problema", RadioIcon3.Content.ToString());
            if (RadioIcon4.IsChecked == true) Par.AddParameterString("Problema", RadioIcon4.Content.ToString());
            if (RadioIcon5.IsChecked == true) Par.AddParameterString("Problema", RadioIcon5.Content.ToString());


            RiempiSegnalazioni.EseguiInsert("Segnalazioni", Par);
            DisplayAlert("Richiesta inviata correttamente", "Ti ringraziamo per la segnalazione provvederemo il prima possibile alla risoluzione del problema!", "OK");
            Funzioni.SendEmail("ripremiasupport@ecocontrolgsm.it", "ripremiasupport@ecocontrolgsm.it", "Segnalazione nuova", $"Data: {DateTime.Now}, IdUtente: {App.DataRowUtente["Id"].ToString()}, IdComune: {App.DataRowUtente["IdComune"].ToString()}, IdPoint: {Point.FirstOrDefault(x => x.Value == PickerPoint.SelectedItem.ToString()).Key}, Note: {Funzioni.Antinull(TxtMessaggio.Text)}");

            Xamarin.Essentials.Preferences.Set("SegnalazioneInviata", DateTime.Now);
            var Page = new PageNavigatore();
            Page.CurrentPage = Page.Children[0];
            Application.Current.MainPage = Page;
        }

        private void BtnAnnulla_Clicked(object sender, EventArgs e) {
            var Page = new PageNavigatore();
            Page.CurrentPage = Page.Children[0];
            Application.Current.MainPage = Page;
        }


        protected override bool OnBackButtonPressed() {
            BtnAnnulla_Clicked(null, null);
            return true;
            //return base.OnBackButtonPressed();
        }

        private void PickerPoint_SelectedIndexChanged(object sender, EventArgs e) {

        }
    }
}