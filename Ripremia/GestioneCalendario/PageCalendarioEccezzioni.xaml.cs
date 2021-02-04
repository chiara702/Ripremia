using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp.GestioneCalendario {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageCalendarioEccezzioni : ContentPage {
        ClassApiParco Parchetto = new ClassApiParco();
        private int IdComune;
        public PageCalendarioEccezzioni(int IdComune) {
            InitializeComponent();
            this.IdComune = IdComune;
            Task.Run(RiempiStack);
            //RiempiStack();
        }

        private DataTable TableTipiRifiuti;
        private List<String> ListaRifiuti = new List<String>();
        private List<ViewCalendarioEccezzione> ListaEccezioni = new List<ViewCalendarioEccezzione>();

        public void RiempiStack() {
            var TableEcc = Parchetto.EseguiQuery("Select Id,Data,(Select Denominazione From CalendarioRifiutiTipo where Id=c.Rifiuto1) as Rifiuto1, (Select Denominazione From CalendarioRifiutiTipo where Id=c.Rifiuto2) as Rifiuto2 From CalendarioRifiutiEccezzioni as c Where ComuneId=" + IdComune);
            
            TableTipiRifiuti = Parchetto.EseguiQuery("Select Id,Denominazione From CalendarioRifiutiTipo Where ComuneId=" + IdComune);
            foreach (DataRow x in TableTipiRifiuti.Rows) {
                ListaRifiuti.Add(x["Denominazione"].ToString());
            }
            foreach (DataRow x in TableEcc.Rows) {
                var view = new ViewCalendarioEccezzione();
                view.ListaRifiuti = ListaRifiuti;
                view.Id = (int)x["Id"];
                view.Rifiuto1 = x["Rifiuto1"].ToString();
                view.Rifiuto2 = x["Rifiuto2"].ToString();
                ListaEccezioni.Add(view);
                Device.BeginInvokeOnMainThread(() => StackEccezioni.Children.Add(view));
            }
        }


        private async void BtnIndietro_Clicked(object sender, EventArgs e) {
            //if (await DisplayAlert("", "Vuoi salvare?", "Salva", "Non salvare") == true) SalvaSuDb();
            await Navigation.PopAsync();
        }

        protected override bool OnBackButtonPressed() {
            BtnIndietro_Clicked(null, null);
            return true;
        }

        private void BtnAddNew_Clicked(object sender, EventArgs e) {
            var view = new ViewCalendarioEccezzione();
            view.ListaRifiuti = ListaRifiuti;
            view.IsNew = true;
            ListaEccezioni.Add(view);
            StackEccezioni.Children.Add(view);
        }

        private int IdFromTiporifiuto(String tiporifiuto) {
            if (tiporifiuto == "") return 0;
            var rowtmp = TableTipiRifiuti.Select("Denominazione='" + Funzioni.AntiAp(tiporifiuto) + "'");
            if (rowtmp.Length == 0) return 0;
            return (int) rowtmp[0]["Id"];
        }


        private void BtnSalva_Clicked(object sender, EventArgs e) {
            foreach (ViewCalendarioEccezzione x in ListaEccezioni) {
                if (x.IsDeleted == true) { //Delete
                    Parchetto.EseguiDelete("CalendarioRifiutiEccezzioni", x.Id);
                } else if (x.IsNew==true) {  //Addnew
                    var Par = Parchetto.GetParam();
                    Par.AddParameterInteger("ComuneId", IdComune);
                    Par.AddParameterInteger("Rifiuto1", IdFromTiporifiuto(x.Rifiuto1));
                    Par.AddParameterInteger("Rifiuto2", IdFromTiporifiuto(x.Rifiuto2));
                    Par.AddParameterObject("Data", x.Data);
                    Parchetto.EseguiInsert("CalendarioRifiutiEccezzioni", Par);
                } else { //Update
                    var Par = Parchetto.GetParam();
                    Par.AddParameterInteger("ComuneId", IdComune);
                    Par.AddParameterInteger("Rifiuto1", IdFromTiporifiuto(x.Rifiuto1));
                    Par.AddParameterInteger("Rifiuto2", IdFromTiporifiuto(x.Rifiuto2));
                    Par.AddParameterObject("Data", x.Data);
                    Parchetto.EseguiUpdate("CalendarioRifiutiEccezzioni", x.Id, Par);
                }
                

            }
            Navigation.PopAsync();
        }
    }
}