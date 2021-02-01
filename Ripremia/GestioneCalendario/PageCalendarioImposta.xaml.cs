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
            var pickers = new Picker[] { RifiutoLun1, RifiutoLun2, RifiutoMar1, RifiutoMar2, RifiutoMer1, RifiutoMer2, RifiutoGio1, RifiutoGio2, RifiutoVen1, RifiutoVen2, RifiutoSab1, RifiutoSab2, RifiutoDom1, RifiutoDom2 };
            foreach (var p in pickers) {
                p.Items.Add("Nessuno");
                foreach (DataRow x in TableTipiRifiuti.Rows) {
                    p.Items.Add(x["Denominazione"].ToString());
                }
            }
            ImpostaRigo(RifiutoLun1, "lun", 1);
            ImpostaRigo(RifiutoLun2, "lun", 2);
            ImpostaRigo(RifiutoMar1, "mar", 1);
            ImpostaRigo(RifiutoMar2, "mar", 2);
            ImpostaRigo(RifiutoMer1, "mer", 1);
            ImpostaRigo(RifiutoMer2, "mer", 2);
            ImpostaRigo(RifiutoGio1, "gio", 1);
            ImpostaRigo(RifiutoGio2, "gio", 2);
            ImpostaRigo(RifiutoVen1, "ven", 1);
            ImpostaRigo(RifiutoVen2, "ven", 2);
            ImpostaRigo(RifiutoSab1, "sab", 1);
            ImpostaRigo(RifiutoSab2, "sab", 2);
            ImpostaRigo(RifiutoDom1, "dom", 1);
            ImpostaRigo(RifiutoDom2, "dom", 2);

        }

        private void ImpostaRigo(Picker oggetto,String GiornoSettimana, int Rifiuto) {
            var Righi = TableCalendarioRifiuti.Select("GiornoSettimana='" + GiornoSettimana + "'");
            if (Righi.Length > 0) oggetto.SelectedItem = Funzioni.Antinull(Righi[0]["Rifiuto" + Rifiuto]);
        }

        private async void BtnIndietro_Clicked(object sender, EventArgs e) {
            if (await DisplayAlert("", "Vuoi salvare?", "Salva", "Non salvare") == true) SalvaSuDb();
            await Navigation.PopAsync();
        }
        protected override bool OnBackButtonPressed() {
            BtnIndietro_Clicked(null, null);
            return true;
        }

        public void SalvaRigo(String GiornoSettimana, String Rifiuto1, String Rifiuto2) {
            if (Rifiuto1 == "Nessuno") Rifiuto1 = "";
            if (Rifiuto2 == "Nessuno") Rifiuto2 = "";
            var Par = Parchetto.GetParam();
            Par.AddParameterInteger("ComuneId", IdComune);
            Par.AddParameterString("GiornoSettimana", GiornoSettimana);
            if (Rifiuto1 != "") {
                var tmp1 = TableTipiRifiuti.Select("Denominazione='" + Funzioni.AntiAp(Rifiuto1) + "'");
                var Id1 = 0;
                if (tmp1.Length > 0) Id1 = (int)tmp1[0]["Id"];
                Par.AddParameterInteger("Rifiuto1", Id1);
            }
            if (Rifiuto2 != "") {
                var tmp2 = TableTipiRifiuti.Select("Denominazione='" + Funzioni.AntiAp(Rifiuto2) + "'");
                var Id2 = 0;
                if (tmp2.Length > 0) Id2 = (int)tmp2[0]["Id"];
                Par.AddParameterInteger("Rifiuto2", Id2);
            }
            Parchetto.EseguiInsert("CalendarioRifiuti", Par);
        }



        public void SalvaSuDb() {
            Parchetto.EseguiCommand("Delete From CalendarioRifiuti Where ComuneId=" + IdComune);
            SalvaRigo("lun", Funzioni.Antinull(RifiutoLun1.SelectedItem), Funzioni.Antinull(RifiutoLun2.SelectedItem));
            SalvaRigo("mar", Funzioni.Antinull(RifiutoMar1.SelectedItem), Funzioni.Antinull(RifiutoMar2.SelectedItem));
            SalvaRigo("mer", Funzioni.Antinull(RifiutoMer1.SelectedItem), Funzioni.Antinull(RifiutoMer2.SelectedItem));
            SalvaRigo("gio", Funzioni.Antinull(RifiutoGio1.SelectedItem), Funzioni.Antinull(RifiutoGio2.SelectedItem));
            SalvaRigo("ven", Funzioni.Antinull(RifiutoVen1.SelectedItem), Funzioni.Antinull(RifiutoVen2.SelectedItem));
            SalvaRigo("sab", Funzioni.Antinull(RifiutoSab1.SelectedItem), Funzioni.Antinull(RifiutoSab2.SelectedItem));
            SalvaRigo("dom", Funzioni.Antinull(RifiutoDom1.SelectedItem), Funzioni.Antinull(RifiutoDom2.SelectedItem));


            

        }

        private void DelRifLun1_Clicked(object sender, EventArgs e) {

        }

        private void DelRifLun2_Clicked(object sender, EventArgs e) {

        }

        private void Button_Clicked(object sender, EventArgs e) {
            SalvaSuDb();
        }

        private void BtnCome_Clicked(object sender, EventArgs e) {
            DisplayAlert("Come funziona?", "Seleziona per ogni giorno della settimana il tipo di rifiuto che verrà ritirato.\nVerrà generato un calendario automatico con quelle tipologie di rifiuti.\nE' possibile lasciare vuoto il campo Rifiuti tipo 2, o entrambi i campi nel caso in cui non venga effettuato nessun ritiro.\nNella sezione Crea eccezzioni, sarà possibile gestire le eccezioni per data.", "OK");
        }
    }
}