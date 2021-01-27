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
    public partial class PageCalendarioImposta : ContentPage {
        ClassApiParco Parchetto = new ClassApiParco();
        private int IdComune;
        public PageCalendarioImposta(int IdComune) {
            InitializeComponent();
            this.IdComune = IdComune;
            RiempiTipiRifiuti();
        }
        private DataTable TableTipiRifiuti;
        private DataTable TableCalendarioRifiuti;

        public void RiempiTipiRifiuti() {
            TableTipiRifiuti = Parchetto.EseguiQuery("Select * From CalendarioRifiutiTipo Where ComuneId=" + IdComune);
            TableCalendarioRifiuti = Parchetto.EseguiQuery("SELECT GiornoSettimana,(Select Denominazione From CalendarioRifiutiTipo Where Id=C.Rifiuto1) as Rifiuto1, (Select Denominazione From CalendarioRifiutiTipo Where Id=C.Rifiuto2) as Rifiuto2   FROM `CalendarioRifiuti` as C Where ComuneId=" + IdComune);
            if (Parchetto.LastError == true) {
                DisplayAlert("Errore", Parchetto.LastErrorDescrizione, "OK");
            }
            var pickers = new Picker[] {RifiutoLun1,RifiutoLun2,RifiutoMar1,RifiutoMar2 };
            foreach (DataRow x in TableTipiRifiuti.Rows) {
                foreach (var p in pickers) {
                    p.Items.Add(x["Denominazione"].ToString());
                }
            }
            var Righi=TableCalendarioRifiuti.Select("GiornoSettimana='lun'");
            if (Righi.Length > 0) RifiutoLun1.SelectedItem = Funzioni.Antinull(Righi[0]["Rifiuto1"]);
            if (Righi.Length > 0) RifiutoLun2.SelectedItem = Funzioni.Antinull(Righi[0]["Rifiuto2"]);

        }


    }
}