using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]

    
    public partial class PageShowAttivitaCommerciali : ContentPage {
        private ClassApiParco Parchetto = new ClassApiParco();
        public PageShowAttivitaCommerciali() {
            InitializeComponent();
            Task.Run(CaricaLoghi);
            //CaricaLoghi();
        }

        public void CaricaLoghi() {
            var TableRow = Parchetto.EseguiQuery("Select * From AttivitaCommerciali where Attivo=1 and ComuneId=" + App.DataRowUtente["IdComune"]  );
            foreach (DataRow x in TableRow.Rows) {
                var SalvaRigo = x;
                var ImgTmp = new ImageButton();
                ImgTmp.Source=ImageSource.FromStream(() => { return new System.IO.MemoryStream((byte[])x["Logo"]); });
                ImgTmp.Aspect = Aspect.AspectFit;
                ImgTmp.HeightRequest = 150;
                ImgTmp.Margin = new Thickness(20);
                ImgTmp.BackgroundColor = Color.Transparent;
                Device.BeginInvokeOnMainThread(()=>StackLoghi.Children.Add(ImgTmp));
                ImgTmp.Clicked += (s,e) => {
                    var Page = new PageDettaglioAttivita((int)SalvaRigo["Id"]);
                    Navigation.PushAsync(Page);
                    //SalvaRigo

                };
            }
            
        }

        private void ImgTmp_Clicked(object sender, EventArgs e) {
            throw new NotImplementedException();
        }

        private void BtnIndietro_Clicked(object sender, EventArgs e) {
            var Page = new PageNavigatore();
            Page.CurrentPage = Page.Children[2];
            Application.Current.MainPage = Page;


            //Application.Current.MainPage = new PageNavigatore();
        }
        protected override bool OnBackButtonPressed() {
            BtnIndietro_Clicked(null, null);
            return true;
        }
    }
}