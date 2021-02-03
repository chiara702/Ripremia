using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageAreaCommercianti : ContentPage {
        public PageAreaCommercianti() {
            InitializeComponent();
        }


        private void BtnIndietro_Clicked(object sender, EventArgs e) {
            Application.Current.MainPage = new PageNavigatore();
        }

        protected override bool OnBackButtonPressed() {
            BtnIndietro_Clicked(null, null);
            return true;
        }

    }
}