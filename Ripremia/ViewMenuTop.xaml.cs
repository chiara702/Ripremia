using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewMenuTop : Grid {
        public ViewMenuTop() {
            InitializeComponent();
            BtnBell.IsVisible = _MostraBell;
            BtnGoBack.IsVisible = _MostraGoBack;

        }

        //public event EventHandler<object> ThresholdReached;
        public ViewMenuLaterale MenuLaterale = null;


        private async void BtnMenu_Clicked(object sender, EventArgs e) {

            if (MenuLaterale!=null) await MenuLaterale.Mostra();
        }
        //private async void ImgMenu_Tapped(object sender, EventArgs e) {
        //}

        

        public Boolean _MostraBell=false;
        public Boolean MostraBell {
            get {
                return _MostraBell;
            }
            set {
                _MostraBell = value;
                BtnBell.IsVisible = _MostraBell;
                Task.Run(() => GetNotifiche());
            }
        }

        public Boolean _MostraGoBack=false;
        public Boolean MostraGoBack{
            get
            {
                return _MostraGoBack;
            }
            set
            {
                _MostraGoBack = value;
                BtnGoBack.IsVisible = _MostraGoBack;
               
            }
        }


        public Boolean NascondiMenu {
            set {
                BtnMenu.IsVisible = !value;
            }
            
        }
         

        public void GetNotifiche(){
            //return; //Da togliere

            var Table = PageNotifiche.TabellaNotifiche();

            if (Table.Select("DataInizio>'" + Funzioni.ToQueryData(App.DataLetturaNotifiche) + "'").Length > 0) {
                Device.BeginInvokeOnMainThread(() => {
                    BtnBell.Source = "BellRed";
                });
                Task.Run(MuoviCampanella);
            }
        }

        private void MuoviCampanella() {
            Device.BeginInvokeOnMainThread(async () => {
                for (int z=0; z<=100; z++) { 
                    for (int x = 0; x <= 5; x++) {
                        await BtnBell.RotateTo(-10, 300);
                        await BtnBell.RotateTo(10, 300);
                    }
                    await Task.Delay(1000);
                }
            });
        }

        private void BtnBell_Clicked(object sender, EventArgs e) {
            Application.Current.MainPage = new PageNotifiche();
            App.DataLetturaNotifiche = DateTime.Now;
        }

        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new PageNavigatore();
        }
    }
}