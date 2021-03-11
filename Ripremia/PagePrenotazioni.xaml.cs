using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PagePrenotazioni : ContentPage {
        public PagePrenotazioni() {
            InitializeComponent();
            Prenotazioni = new List<ClassePrenotazione>();

            Task.Run(() => RiempiPrenotazioni());


        }

        protected override bool OnBackButtonPressed() {
            BtnIndietro_Clicked(null,null);
            //Application.Current.MainPage = new PageNavigatore();
            return true;
        }



        public void RiempiPrenotazioni() {
            var Parchetto = new ClassApiParco();
            var Where = "(DataRitiro >= '" + DateTime.Now.AddDays(-7).ToString("yy-MM-dd") + "' Or DataRitiro is NULL)";
            var Query = "Select * From PrenotaRitiri Where UtenteId=" + App.DataRowUtente["Id"].ToString() + " And " + Where;
            var Table = Parchetto.EseguiQuery(Query + " Order by DataCreazione desc");
            if (Parchetto.LastError == true) {
                DisplayAlert("Errore", Parchetto.LastErrorDescrizione, "OK");
            }
            foreach (System.Data.DataRow x in Table.Rows) {
                var n = new ClassePrenotazione();
                n.DataCreazione = DateTime.Parse(x["DataCreazione"].ToString()).ToString("dd/MM/yy");
                n.Telefono = Funzioni.Antinull(x["Telefono"]);
                n.Note = Funzioni.Antinull(x["Note"]);
                //n.ConfermaRitiro = bool.Parse(x["ConfermaRitiro"].ToString());
                if ((Boolean)x["ConfermaRitiro"] == true) n.ConfermaRitiro = "CONFERMATO"; else n.ConfermaRitiro = "IN ELABORAZIONE";

                if (x["DataRitiro"] != DBNull.Value) {
                    n.DataRitiro = DateTime.Parse(x["DataRitiro"].ToString()).ToString("dd/MM/yy");
                    n.DataRitiroVisible = true;
                } else {
                    n.DataRitiroVisible = false;
                }

                

                switch (x["TipoRifiuto"].ToString()){
                    case "Ingombranti":
                        n.SourceIcona = "Ingombranti";
                        break;
                    case "Raee":
                        n.SourceIcona = "Raee";
                        break;
                    case "Verde":
                        n.SourceIcona = "Verde";
                        break;
                }

               
                Prenotazioni.Add(n);
               
                
                
                //n.RispondiVisibile = Boolean.Parse(x["Rispondi"].ToString());


            }
            Device.BeginInvokeOnMainThread(() => {
                BindingContext = this;
                if (Table.Rows.Count == 0) {
                    txtNotice.IsVisible = true;
                }
            });

        }

        public List<ClassePrenotazione> Prenotazioni { get; set; }
        public class ClassePrenotazione {
            public String DataCreazione { get; set; }
            public String Telefono { get; set; }
            public String SourceIcona { get; set; }

            public String Note { get; set; }
            public String ConfermaRitiro { get; set; }
            public String DataRitiro { get; set; }
            public Boolean DataRitiroVisible { get; set; }

        }
        private void BtnIndietro_Clicked(object sender, EventArgs e) {
            var Page = new PageNavigatore();
            Page.CurrentPage = Page.Children[4];
            Application.Current.MainPage = Page;
        }
    }
}