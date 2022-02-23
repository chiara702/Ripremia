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
        private Boolean Preferito = false;
        public ViewListaIsole() {
            InitializeComponent();
        }
        public void SetPreferito() {
            Preferito=true;
            BtnPreferito.Source="collapse.png";
        }
        public EventHandler Clicked;
        private void BtnClick_Tapped(object sender, EventArgs e) {
            Clicked.Invoke(sender, e);
        }

        private void BtnPreferito_Clicked(object sender, EventArgs e) {
            if (Preferito==false) {
                SetPreferito();
            } else {
                Preferito=false;
                BtnPreferito.Source="Come.png";
            }
        }
    }
}