using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewMenuLaterale : Grid {
        public ViewMenuLaterale() {
            InitializeComponent();
            var PathLogo = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "LogoComune.png";
            if (System.IO.File.Exists(PathLogo) == true) {
                ImgLogoComune.Source = ImageSource.FromFile(PathLogo);
            }
            PathLogo = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "Logo.png";
            if (System.IO.File.Exists(PathLogo) == true) {
                ImgLogo.Source = ImageSource.FromFile(PathLogo);
            }

            if ((int)App.DataRowUtente["AdminComuneId"] > 0 || Funzioni.Antinull(App.DataRowUtente["AdminSuperuserCode"]) != "") StkAreaRiservata.IsVisible = true;
            if ((Boolean)App.DataRowUtente["AdminCommerciante"] == true) StkAreaCommercianti.IsVisible = true;

        }



        public async Task Mostra() {
            await MenuLaterale.TranslateTo(-250, 0, 1, Easing.Linear);
            await MenuLaterale2.TranslateTo(-250, 0, 1, Easing.Linear);
            GridOverlay.IsVisible = true;
            _ = MenuLaterale.TranslateTo(0, 0, 400, Easing.Linear);
            await MenuLaterale2.TranslateTo(0, 0, 400, Easing.Linear);
        }

        
        
        private async void TapCloseMenu_Tapped(object sender, EventArgs e) {
            _ = MenuLaterale.TranslateTo(-250, 0, 400, Easing.Linear);
            await MenuLaterale2.TranslateTo(-250, 0, 400, Easing.Linear);
            GridOverlay.IsVisible = false;
        }

        

        private void TapLogout_Tapped(object sender, EventArgs e) {
            Xamarin.Essentials.Preferences.Set("Loggato", false);
            Xamarin.Essentials.Preferences.Set("Email", "");
            //UtenteDatiMemoria.AzzeraTimeUpdate();
            Application.Current.MainPage = new PagePresentazione();
        }

        private void TapNumeriUtili_Tapped(object sender, EventArgs e) {
            Application.Current.MainPage = new PageSuperuser();

        }

        private void TapPageUtente_Tapped(object sender, EventArgs e) {
            var Page = new PageNavigatore();
            Page.CurrentPage = Page.Children[4];
            Application.Current.MainPage = Page;
        }


        private void TapAreaRiservata_Tapped(object sender, EventArgs e) {
            Application.Current.MainPage = new PageAreaRiservata();

        }

        private void TapAreaCommercianti_Tapped(object sender, EventArgs e) {      
            if ((int)App.DataRowUtente["AttivitaCommercialiId"] == 0) {
              return;
            }
            var Page = new PageAreaCommercianti();
            App.Current.MainPage = new NavigationPage(Page);
            //Application.Current.MainPage = new PageAreaCommercianti();
        }
    }
}