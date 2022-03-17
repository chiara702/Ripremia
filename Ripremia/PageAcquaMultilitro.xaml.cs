using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageAcquaMultilitro : ContentPage {
        public PageAcquaMultilitro() {
            InitializeComponent();
            EntryLitri.Text="1";
            BarCodeId.BarcodeValue = Xamarin.Essentials.Preferences.Get("QrCodeNew", "1") + "," + EntryLitri.Text;
        }

        private void BtnDown_Clicked(object sender, EventArgs e) {
            int valore = Convert.ToInt16(EntryLitri.Text);
            if (valore>1) valore--;
            EntryLitri.Text=valore.ToString();
            BarCodeId.BarcodeValue = Xamarin.Essentials.Preferences.Get("QrCodeNew", "1") + "," + EntryLitri.Text;
        }

        private void BtnUp_Clicked(object sender, EventArgs e) {
            int valore = Convert.ToInt16(EntryLitri.Text);
            if (valore<30) valore++;
            EntryLitri.Text=valore.ToString();
            BarCodeId.BarcodeValue = Xamarin.Essentials.Preferences.Get("QrCodeNew", "1") + "," + EntryLitri.Text;
        }

        
    }
}