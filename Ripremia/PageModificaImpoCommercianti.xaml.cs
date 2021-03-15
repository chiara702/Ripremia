using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageModificaImpoCommercianti : ContentPage {
        public PageModificaImpoCommercianti() {
            InitializeComponent();
        }
        async void BtnIndietro_Clicked(object sender, EventArgs e) {
            await Navigation.PopAsync(true);
        }

        protected override bool OnBackButtonPressed() {
            BtnIndietro_Clicked(null, null);
            return true;

        }

        private void BtnSalva_Clicked(object sender, EventArgs e) {

        }
    }
}