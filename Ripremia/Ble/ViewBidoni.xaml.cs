using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp.Ble {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewBidoni : ContentView {
        public int Indice = 0;
        public ViewBidoni(String Rifiuto, String Testo1, String Testo2, int LivelloPerc) {
            InitializeComponent();
            ImgSfondo.Source = "BinGrigio";
            if (Rifiuto == "Secco") ImgSfondo.Source = "BinGrigio";
            if (Rifiuto == "Plastica") ImgSfondo.Source = "BinGiallo";
            if (Rifiuto == "Umido") ImgSfondo.Source = "BinMarrone";
            if (Rifiuto == "Vetro") ImgSfondo.Source = "BinVerde";
            if (Rifiuto == "Carta") ImgSfondo.Source = "BinAzzurro";

            if (LivelloPerc < 5) ImgRiempimento.Source = "";
            else if (LivelloPerc < 25) ImgRiempimento.Source = "Perc25";
            else if (LivelloPerc < 50) ImgRiempimento.Source = "Perc50";
            else if (LivelloPerc < 75) ImgRiempimento.Source = "Perc75";
            else ImgRiempimento.Source = "Perc100";

            Txt1.Text = Testo1 + '\n' + Testo2;

        }
        public EventHandler Clicked;


        private void ImgSfondo_Clicked(object sender, EventArgs e) {
            Clicked.Invoke(sender, e);
        }
    }
}