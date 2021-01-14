using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageServiziNew : ContentPage {
        public PageServiziNew() {
            InitializeComponent();
            LblUtente.Text = "Ciao, " + App.DataRowUtente["Nome"].ToString() + "!";
            MenuTop.MenuLaterale = MenuLaterale;
        }

        private void BtnPrenotaRitiro_Tapped(object sender, EventArgs e) {
            //Navigation.PushAsync(new PagePrenotaRitiro());
            Application.Current.MainPage = new PagePrenotaRitiro();
        }


    }

}