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
    public partial class ViewCalendarioEccezzione : ContentView {
        ClassApiParco Parchetto = new ClassApiParco();
        private int IdComune;
        public ViewCalendarioEccezzione(int IdComune) {
            InitializeComponent();
            this.IdComune = IdComune;
            RiempiTipiRifiuti();
        }
        private DataTable TableTipiRifiuti;
        private DataTable TableCalendarioRifiuti;


        public void RiempiTipiRifiuti() {
            TableTipiRifiuti = Parchetto.EseguiQuery("Select * From CalendarioRifiutiTipo Where ComuneId=" + IdComune);
            var pickers = new Picker[] { RifiutoEcc1, RifiutoEcc2 };
            foreach (var p in pickers) {
                p.Items.Add("Nessuno");
                foreach (DataRow x in TableTipiRifiuti.Rows) {
                    p.Items.Add(x["Denominazione"].ToString());
                }
            }
        }

        private void Delete_Clicked(object sender, EventArgs e) {

        }
    }
}