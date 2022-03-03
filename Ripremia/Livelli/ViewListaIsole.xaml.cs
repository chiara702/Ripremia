using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp.Livelli {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewListaIsole : ContentView {
        public Boolean Preferito = false;
        public string Coordinate="";
        public Double DistanzaKm = 0;
        public ViewListaIsole() {
            InitializeComponent();
        }
        public void SetPreferito() {
            Preferito=true;
            BtnPreferito.Source="heartGr_yes.png";
        }
        public EventHandler Clicked;
        private void BtnClick_Tapped(object sender, EventArgs e) {
            Clicked.Invoke(sender, e);
        }
        public EventHandler PreferedClicked;
        

        private void BtnPreferito_Clicked(object sender, EventArgs e) {
            if (Preferito==false) {
                SetPreferito();
            } else {
                Preferito=false;
                BtnPreferito.Source="heart_no.png";
            }
            PreferedClicked.Invoke(sender, e);
        }
    }
}