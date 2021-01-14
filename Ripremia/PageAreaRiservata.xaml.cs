using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageAreaRiservata : ContentPage {
        public PageAreaRiservata() {
            InitializeComponent();
        }

        //private async void ImgMenu_Tapped(object sender, EventArgs e) {
        //    MenuLaterale.IsVisible = true;
        //    await MenuLaterale.Mostra();
        //}

        private void BtnBack_Clicked(object sender, EventArgs e) {
            Application.Current.MainPage = new PageNavigatore();
        }

        private void BtnIndietro_Clicked(object sender, EventArgs e) {
            BtnBack_Clicked(null, null);
        }

        private void BtnAddNotifiche_Clicked(object sender, EventArgs e) {
            Application.Current.MainPage = new PageAddNotifiche();
        }
    }
}