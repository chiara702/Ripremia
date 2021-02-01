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
    public partial class ViewCalendarioEccezzione : Grid {
        ClassApiParco Parchetto = new ClassApiParco();
        public ViewCalendarioEccezzione() {
            InitializeComponent();
            //this.IdComune = IdComune;
            //RiempiTipiRifiuti();
        }
        public int Id{ get; set; }
        public int IdComune{ get; set; }
        public DateTime Data{ 
            get {
                return DataEccezioni.Date;
            }
            set {
                DataEccezioni.Date = value;
            } 
        }
        

        public List<String> ListaRifiuti {
            set {
                RifiutoEcc1.Items.Clear();
                RifiutoEcc2.Items.Clear();
                RifiutoEcc1.Items.Add("Nessuno");
                RifiutoEcc2.Items.Add("Nessuno");
                foreach (String x in value) {
                    RifiutoEcc1.Items.Add(x);
                    RifiutoEcc2.Items.Add(x);
                }
            }
        }

        public String Rifiuto1 {
            set {
                RifiutoEcc1.SelectedItem = value;
            }
            get {
                if (RifiutoEcc1.SelectedItem == null) return "";
                if (RifiutoEcc1.SelectedItem.ToString() == "Nessuno") return "";
                return RifiutoEcc1.SelectedItem.ToString();
            }
        }
        public String Rifiuto2 {
            set {
                RifiutoEcc2.SelectedItem = value;
            }
            get {
                if (RifiutoEcc2.SelectedItem == null) return "";
                if (RifiutoEcc2.SelectedItem.ToString() == "Nessuno") return "";
                return RifiutoEcc2.SelectedItem.ToString();
            }
        }

        public Boolean IsNew { get; set; } = false;
        public Boolean IsDeleted { get; set; } = false;



        private void Delete_Clicked(object sender, EventArgs e) {
            this.IsVisible = false;
            this.IsDeleted = true;

        }
    }
}