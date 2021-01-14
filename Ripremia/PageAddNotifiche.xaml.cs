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
    public partial class PageAddNotifiche : ContentPage {
        private Dictionary<int, String> Comuni;

        private ClassApiParco Parchetto = new ClassApiParco();

        public PageAddNotifiche() {
            InitializeComponent();
            if (String.IsNullOrEmpty((String)App.DataRowUtente["AdminSuperUserCode"]) == false) { //se sei admin super user
                Comuni=ElencoComuni((String)App.DataRowUtente["AdminSuperUserCode"]);
                TxtMittente.Text = Funzioni.Antinull(Parchetto.EseguiCommand("Select Denominazione From SuperUser Where Codice='" + Funzioni.AntiAp(App.DataRowUtente["AdminSuperUserCode"].ToString()) + "'"));
                foreach (var x in Comuni) {
                    PickerComune.Items.Add(x.Value);
                }
            } else { //se sei admin comune 
                StackDestinatari.IsEnabled = false;
                TxtMittente.Text = Parchetto.EseguiCommand("Select Nome From Comune Where Id=" + Funzioni.AntiAp(App.DataRowUtente["AdminComuneId"].ToString())).ToString();
            }
            

        }
        private void BtnBack_Clicked(object sender, EventArgs e) {
            Application.Current.MainPage = new PageAreaRiservata();
        }

        private void BtnIndietro_Clicked(object sender, EventArgs e) {
            BtnBack_Clicked(null, null);
        }

        public class RadioButton : Xamarin.Forms.TemplatedView, Xamarin.Forms.IElementConfiguration<Xamarin.Forms.RadioButton> {
            public IPlatformElementConfiguration<T, Xamarin.Forms.RadioButton> On<T>() where T : IConfigPlatform {
                throw new NotImplementedException();
            }
        }

        private Dictionary<int,String> ElencoComuni(String SuperUser) {
            var rit = new Dictionary<int, String>();
            var Parchetto = new ClassApiParco();
            var Table=Parchetto.EseguiQuery("Select Id,Nome From Comune Where CodiceSuperUser='" + SuperUser + "'");
            if (Parchetto.LastError == true) {
                DisplayAlert("Error", "Si è verificato un errore. Verifica connettività!", "OK");
                return null;
            }
            foreach (DataRow x in Table.Rows) {
                rit.Add(int.Parse(x["Id"].ToString()), x["Nome"].ToString());
            }
            return rit;
        }

        private void BtnInviaNotifica_Clicked(object sender, EventArgs e) {
            var Par = Parchetto.GetParam();
            Par.AddParameterObject("DataCreazione", DateTime.Now);
            if (String.IsNullOrEmpty((String)App.DataRowUtente["AdminSuperUserCode"]) == false) { //se sei admin super user
                Par.AddParameterString("MittenteSuperUser", (String)App.DataRowUtente["AdminSuperUserCode"]);
                if (RadioTuttiComuni.IsChecked == true) {
                    Par.AddParameterString("DestinatarioSuperUser", (String)App.DataRowUtente["AdminSuperUserCode"]);
                }
                if (RadioComune.IsChecked == true) {
                    if (PickerComune.SelectedItem == null) {
                        DisplayAlert("Errore", "Seleziona il comune!", "OK");
                        return;
                    }
                    Par.AddParameterInteger("DestinatarioComune", Comuni.FirstOrDefault(x => x.Value == PickerComune.SelectedItem.ToString()).Key);
                }
            } else if ((int)App.DataRowUtente["AdminComuneId"] > 0) { //se sei admin comune
                Par.AddParameterInteger("MittenteComune", (int)App.DataRowUtente["AdminComuneId"]);
            } else {
                DisplayAlert("", "Non sei autorizzato!", "OK");
                return;
            }
            Par.AddParameterInteger("MittenteUtente", 0);
            Par.AddParameterInteger("DestinatarioUtente", 0);
            Par.AddParameterString("Topics", ""); //Da gestire
            if (RadioIcon0.IsChecked == true) Par.AddParameterInteger("IconIndex", 0);
            if (RadioIcon1.IsChecked == true) Par.AddParameterInteger("IconIndex", 1);
            if (RadioIcon2.IsChecked == true) Par.AddParameterInteger("IconIndex", 2);
            Par.AddParameterObject("DataInizio", DataPubblicazione.Date);
            Par.AddParameterObject("DataScadenza", DataScadenza.Date);
            Par.AddParameterString("DescrizioneBreve", ""+TxtOggetto.Text);
            Par.AddParameterString("DescrizioneEstesa", ""+TxtDescrizione.Text);
            if (RadioPushSi.IsChecked == true) Par.AddParameterInteger("Push", 1); else Par.AddParameterInteger("Push", 0);
            Par.AddParameterString("Link", ""+TxtLink.Text);

            Parchetto.EseguiInsert("Notifiche", Par);
            if (Parchetto.LastError == true) {
                DisplayAlert("Errore", Parchetto.LastErrorDescrizione, "OK");
                return;
            }
            DisplayAlert("Notifica Ok", "", "OK");


        }

        private void BtnIIndietro_Clicked(object sender, EventArgs e) {
            BtnBack_Clicked(null, null);
        }
    }
}