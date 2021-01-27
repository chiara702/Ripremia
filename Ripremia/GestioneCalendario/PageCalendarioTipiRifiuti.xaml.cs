using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp.GestioneCalendario {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageCalendarioTipiRifiuti : ContentPage {


        ClassApiParco Parchetto = new ClassApiParco();
        private int IdComune;

       
        public List<ClasseTipiRifiuti> TipiRifiuti { get; set; } = new List<ClasseTipiRifiuti>();

        

        
        public class ClasseTipiRifiuti {
            public int Rnd { get; set; } = new Random().Next();
            public int Id { get; set; }
            public String Denominazione { get; set; } = "";
            public Boolean Azzurro { get; set; } = false;
            public Boolean Blu { get; set; } = false;
            public Boolean Marrone { get; set; } = false;
            public Boolean Giallo { get; set; } = false;
            public Boolean Grigio { get; set; } = false;
            public Boolean Verde { get; set; } = false;
            public Boolean Rosso { get; set; } = false;
            public Boolean Bianco { get; set; } = false;
            public Boolean Arancio { get; set; } = false;
            public Boolean LightGrigio { get; set; } = false;
            public String CosaConferire { get; set; } = "";
            public String CosaNonConferire { get; set; } = "";
            public String ComeConferire { get; set; } = "";
            public Boolean Eliminato { get; set; } = false;
            public Command ButtonElimina { get; } = new Command<ClasseTipiRifiuti>((obj) => {
                
                Device.BeginInvokeOnMainThread(async () => {
                    
                    if (await Application.Current.MainPage.DisplayAlert("Eliminazione", "Sei sicuro di voler eliminare la notifica?", "OK", "ANNULLA") == true) {
                        
                    }
                });
            });

        }


        public PageCalendarioTipiRifiuti(int IdComune) {
            InitializeComponent();
            this.IdComune = IdComune;
            Task.Run(() => RiempiTipiRifiuti());
        }

        public void RiempiTipiRifiuti() {
            var Table = Parchetto.EseguiQuery("Select * From CalendarioRifiutiTipo Where ComuneId=" + IdComune);
            if (Parchetto.LastError == true) {
                DisplayAlert("Errore", Parchetto.LastErrorDescrizione, "OK");
            }
            foreach (System.Data.DataRow x in Table.Rows) {
                var n = new ClasseTipiRifiuti();
                n.Id = (int)x["Id"];
                n.Denominazione = x["Denominazione"].ToString();
                n.CosaConferire = x["CosaConferire"].ToString();
                n.CosaNonConferire = x["CosaNonConferire"].ToString();
                n.ComeConferire = x["ComeConferire"].ToString();
                var Colore = x["Colore"].ToString();
                switch (Colore) {
                    case "Azzurro":
                        n.Azzurro = true;
                        break;
                    case "Blu":
                        n.Blu = true;
                        break;
                    case "Marrone":
                        n.Marrone = true;
                        break;
                    case "Giallo":
                        n.Giallo = true;
                        break;
                    case "Grigio":
                        n.Grigio = true;
                        break;
                    case "Verde":
                        n.Verde = true;
                        break;
                    case "Rosso":
                        n.Rosso = true;
                        break;
                    case "Bianco":
                        n.Bianco = true;
                        break;
                    case "Arancio":
                        n.Arancio = true;
                        break;
                    case "LightGrigio":
                        n.LightGrigio = true;
                        break;

                }
                TipiRifiuti.Add(n);

            }
            Device.BeginInvokeOnMainThread(() => {
                BindingContext = this;
            });

        }

        public void SalvaSuDb() {
            foreach (var x in TipiRifiuti) {
                if (x.Eliminato == true) {
                    Parchetto.EseguiCommand("Delete From CalendarioRifiutiTipo Where Id=" + x.Id.ToString());
                }
                var Param = Parchetto.GetParam();
                Param.AddParameterInteger("ComuneId", IdComune);
                Param.AddParameterString("Denominazione", x.Denominazione);
                Param.AddParameterString("CosaConferire", x.CosaConferire);
                Param.AddParameterString("CosaNonConferire", x.CosaNonConferire);
                Param.AddParameterString("ComeConferire", x.ComeConferire);
                var Colore = "";
                if (x.Blu == true) Colore = "Blu";
                if (x.Azzurro == true) Colore = "Azzurro";
                if (x.Marrone == true) Colore = "Marrone";
                if (x.Giallo == true) Colore = "Giallo";
                if (x.Grigio == true) Colore = "Grigio";
                if (x.Verde == true) Colore = "Verde";
                if (x.Rosso == true) Colore = "Rosso";
                if (x.Bianco == true) Colore = "Bianco";
                if (x.Arancio == true) Colore = "Arancio";
                if (x.LightGrigio == true) Colore = "LightGrigio";
                Param.AddParameterString("Colore", Colore);
                if (x.Id > 0) {
                    Parchetto.EseguiUpdate("CalendarioRifiutiTipo", x.Id, Param);
                } else {
                    Parchetto.EseguiInsert("CalendarioRifiutiTipo", Param);
                }
            }
            
        }

        protected override bool OnBackButtonPressed() {
            BtnIndietro_Clicked(null, null);
            return true;
        }
        private async void BtnIndietro_Clicked(object sender, EventArgs e) {
            if (await DisplayAlert("", "Vuoi salvare?", "Salva", "Non salvare") == true) SalvaSuDb();
            await Navigation.PopAsync();
            //Application.Current.MainPage = new PageAreaRiservata();
        }

        private void Button_Clicked(object sender, EventArgs e) {
            SalvaSuDb();
            Navigation.PopAsync();
        }

        private void Button_Clicked_1(object sender, EventArgs e) {
            var n = new ClasseTipiRifiuti();
            n.Id = 0;
            n.Denominazione = "Nuovo Rifiuto";
            TipiRifiuti.Add(n);
            
            BindingContext = null;
            BindingContext = this;
            Device.BeginInvokeOnMainThread(() => {
                BindingContext = null;
                BindingContext = this;
            });
        }
    }
}