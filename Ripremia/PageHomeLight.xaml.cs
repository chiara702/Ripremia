using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageHomeLight : ContentPage {
        public PageHomeLight() {
            InitializeComponent();
            BarCodeId.BarcodeValue = Xamarin.Essentials.Preferences.Get("QrCodeNew", "1");
        }

        private void BtnHomeFull_Clicked(object sender, EventArgs e) {
            Application.Current.MainPage = new PageLogin();
        }
    }
}