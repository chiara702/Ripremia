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


    public partial class PageNotifiche : ContentPage {
        ClassApiParco Parchetto = new ClassApiParco();
        public PageNotifiche() {
            InitializeComponent();
            Notifiche = new List<ClasseNotifica>();
            
            Task.Run(() => RiempiNotifiche());
           

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
            var WhereCondition = " And DataInizio >= '" + Funzioni.ToQueryData(DateTime.Now) + "' And DataScadenza <= '" + Funzioni.ToQueryData(DateTime.Now) + "'";
            var Table = Api.EseguiQuery("Select * From Notifiche Where (" + WhereTopics + ")" + WhereCondition + " Order By DataInizio desc");
            return Table;
        }

        public void RiempiNotifiche() {
            var Table = TabellaNotifiche();
            if (Parchetto.LastError == true) {
                DisplayAlert("Errore", Parchetto.LastErrorDescrizione, "OK");
            }
            foreach (System.Data.DataRow x in Table.Rows) {
                var n = new ClasseNotifica();
                n.Id = int.Parse(x["Id"].ToString());
                n.DescrizioneBreve = Funzioni.Antinull(x["DescrizioneBreve"]);
                //var nd = new ClasseNotifica.ClasseNotificaDettaglio();
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


                n.RispondiVisibile = (Boolean)x["Rispondi"];
                Notifiche.Add(n);

            }
            Device.BeginInvokeOnMainThread(() => {
                //InitializeComponent();
                BindingContext = this;
                if (Table.Rows.Count == 0){
                    txtNotice.IsVisible = true;
                }
            });
            
        }
        
        public List<ClasseNotifica> Notifiche { get; set; }
        public class ClasseNotifica {
            public int Id { get; set; }
            public String DescrizioneBreve { get; set; }

            public String DescrizioneEstesa { get; set; }
            public String Link { get; set; }
            public String SourceIcona { get; set; }

            public Boolean RispondiVisibile { get; set; }
            

        }

        private void BtnIndietro_Clicked(object sender, EventArgs e) {
            Application.Current.MainPage = new PageNavigatore();
        }

       
       
    }

}