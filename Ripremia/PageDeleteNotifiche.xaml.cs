using EcoServiceApp;
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
    public partial class PageDeleteNotifiche : ContentPage {
        ClassApiParco Parchetto = new ClassApiParco();

        public List<ClasseNotifica> Notifiche { get; set; }
        public class ClasseNotifica {
            public int Id { get; set; }
            public String DataCreazione { get; set; }
            public String DataInizio { get; set; }
            public String DataScadenza { get; set; }
            public String DescrizioneBreve { get; set; }
            public String DescrizioneEstesa { get; set; }
            public String Link { get; set; }
            public String SourceIcona { get; set; }

            public Boolean RispondiVisibile { get; set; }

            public Command ButtonInizia { get; } = new Command<ClasseNotifica>((obj) => {
                Device.BeginInvokeOnMainThread(async () => { 
                    if (await Application.Current.MainPage.DisplayAlert("Eliminazione","Sei sicuro di voler eliminare la notifica?", "OK","ANNULLA")==true) {
                        var p = new ClassApiParco();
                        p.EseguiCommand("Delete From Notifiche Where Id=" + obj.Id.ToString());
                        Application.Current.MainPage = new PageDeleteNotifiche();
                    }
                });
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

        private Dictionary<int, String> Comuni;
        public PageDeleteNotifiche() {
            InitializeComponent();
            Notifiche = new List<ClasseNotifica>();
            //riempi Picker comuni
            if (String.IsNullOrEmpty((String)App.DataRowUtente["AdminSuperUserCode"]) == false) { //se sei admin super user
                PickerComune.Items.Clear();              
                PickerComune.Items.Add("VALIDE PER TUTTI I COMUNI");
                Comuni=ElencoComuni((String)App.DataRowUtente["AdminSuperUserCode"]);
                foreach (var x in Comuni) {
                    PickerComune.Items.Add(x.Value);
                }
                
            } else { //se sei comune super user
                PickerComune.IsVisible = false;
                String Where = "MittenteComune=" + App.DataRowUtente["AdminComuneId"];
                var Table = Parchetto.EseguiQuery("Select * From Notifiche Where " + Where + " Order By DataInizio desc");
                RiempiNotifiche(Table);
            }



        }

        protected override bool OnBackButtonPressed() {
            BtnIndietro_Clicked(null, null);
            return true;
        }

        public static DataTable TabellaNotifiche() {
            var Api = new ClassApiParco();
            var Topics = new List<String>();
            Topics.Add("DestinatarioComune=" + App.DataRowComune["Id"].ToString());
            Topics.Add("DestinatarioSuperUser='" + App.DataRowSuperUser["Codice"].ToString() + "'");
            Topics.Add("DestinatarioUtente=" + App.DataRowUtente["Id"].ToString());
            var WhereTopics = String.Join(" or ", Topics) + "";
           
            var Table = Api.EseguiQuery("Select * From Notifiche Where (" + WhereTopics + ")" + " Order By DataInizio desc");
            return Table;
        }

        public void RiempiNotifiche(DataTable TableNotifiche) {
            if (Parchetto.LastError == true) {
                DisplayAlert("Errore", Parchetto.LastErrorDescrizione, "OK");
            }
            Notifiche.Clear();
            foreach (System.Data.DataRow x in TableNotifiche.Rows) {
                var n = new ClasseNotifica();
                n.Id = int.Parse(x["Id"].ToString());
                n.DescrizioneBreve = Funzioni.Antinull(x["DescrizioneBreve"]);
                n.DataInizio = DateTime.Parse(x["DataInizio"].ToString()).ToString("dd/MM/yy");
                n.DataCreazione = DateTime.Parse(x["DataCreazione"].ToString()).ToString("dd/MM/yy");
                n.DataScadenza = DateTime.Parse(x["DataScadenza"].ToString()).ToString("dd/MM/yy");
                n.DescrizioneEstesa = Funzioni.Antinull(x["DescrizioneEstesa"]);
                n.Link = Funzioni.Antinull(x["Link"]);
                switch (int.Parse(x["IconIndex"].ToString())) {
                    case 0:
                        n.SourceIcona = "InfoNote";
                        break;
                    case 1:
                        n.SourceIcona = "AttenzioneNote";
                        break;
                    case 2:
                        n.SourceIcona = "AltroNote";
                        break;
                    case 3:
                        n.SourceIcona = "";
                        break;
                }


                n.RispondiVisibile = ((ulong)x["Rispondi"]) == 1;
                Notifiche.Add(n);

            }
            Device.BeginInvokeOnMainThread(() => {
                BindingContext = null;
                BindingContext = this;
            });

        }

        

        private void BtnIndietro_Clicked(object sender, EventArgs e) {
            Application.Current.MainPage = new PageAreaRiservata();
        }

        DataTable Table;
        private void PickerComune_SelectedIndexChanged(object sender, EventArgs e) {
            if (PickerComune.SelectedItem == null) return;
            if (PickerComune.SelectedItem.ToString() == "Notifiche Generali") {
                String Where = "MittenteSuperUser=" + App.DataRowUtente["AdminSuperUserCode"] + " And DestinatarioSuperUser=" + App.DataRowUtente["AdminSuperUserCode"];
                Table = Parchetto.EseguiQuery("Select * From Notifiche Where " + Where + " Order By DataInizio desc");
                RiempiNotifiche(Table);
                PickerComune.IsVisible = false;

            } else {
                String Where = "MittenteSuperUser=" + App.DataRowUtente["AdminSuperUserCode"] + " And DestinatarioComune=" + Comuni.FirstOrDefault(x => x.Value == PickerComune.SelectedItem.ToString()).Key;
                Table = Parchetto.EseguiQuery("Select * From Notifiche Where " + Where + " Order By DataInizio desc");
                RiempiNotifiche(Table);
            }
            BtnDeleteScadute.IsEnabled = true;
        }

        private void BtnDeleteScadute_Clicked(object sender, EventArgs e) {
            foreach (DataRow x in Table.Rows) {
                if ((DateTime)x["DataScadenza"]<DateTime.Now) Parchetto.EseguiCommand("Delete From Notifiche Where Id=" + x["Id"].ToString());
            }
            PickerComune_SelectedIndexChanged(null, null);
        }

        //private void BtnDeleteNotifica_Clicked(object sender, EventArgs e) {
        //    DisplayAlert("Pagina in costruzione!", "Presto disponibile", "ok");
        //}
    }

}

