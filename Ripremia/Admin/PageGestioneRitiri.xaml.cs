using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageGestioneRitiri : ContentPage {
        public PageGestioneRitiri() {
            InitializeComponent();
        }


        private void BtnMostraMappa_Tapped(object sender, EventArgs e) {

        }

        private void ImageButton_Clicked(object sender, EventArgs e) {

        }

        private void PickerComune_SelectedIndexChanged(object sender, EventArgs e) {

        }

        private void BtnRaee_Clicked(object sender, EventArgs e) {

        }

        private void BtnIngombranti_Clicked(object sender, EventArgs e) {

        }

        private void BtnVerde_Clicked(object sender, EventArgs e) {

        }

        private void BtnIndietro_Clicked(object sender, EventArgs e) {
            Application.Current.MainPage = new PageAreaRiservata();
        }


        protected override bool OnBackButtonPressed() {
            BtnIndietro_Clicked(null, null);
            return true;
        }
    }
}