using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageSuperuser : ContentPage {

        protected override bool OnBackButtonPressed() {
            Application.Current.MainPage = new PageNavigatore();
            return true;
        }


       
        
       
        public PageSuperuser() {
            InitializeComponent();
            var PathLogo = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "Logo.png";
            if (System.IO.File.Exists(PathLogo) == true) {
                ImgLogo.Source = ImageSource.FromFile(PathLogo);
            }
            var row = App.DataRowSuperUser;
            if (row != null) {
                TxtTelefono1.Text = row["Telefono"].ToString();
                TxtEmail.Text = row["Email"].ToString();
                TxtOrari1.Text = row["OrarioApertura1"].ToString();
                TxtOrari2.Text = row["OrarioApertura2"].ToString();
                TxtWww.Text = row["SitoWeb"].ToString();
                TxtInfo.Text = row["Via"].ToString() + " " + row["Comune"].ToString() + " - " + "(" + row["Provincia"].ToString() + ")";
            }
        }

        private async void ImgMenu_Tapped(object sender, EventArgs e) {
            MenuLaterale.IsVisible = true;
            await MenuLaterale.Mostra();

        }

        private void TapTelefono1_Tapped(object sender, EventArgs e) {
            Xamarin.Essentials.PhoneDialer.Open(TxtTelefono1.Text);
        }
        private void BtnIndietro_Clicked(object sender, EventArgs e) {
            Application.Current.MainPage = new PageNavigatore();
        }
    }
}