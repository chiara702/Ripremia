using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp.Livelli {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageLivelloIsola : ContentPage {
        private int bidoneId;
        public PageLivelloIsola(int BidoneId) {
            InitializeComponent();
            bidoneId = BidoneId;
            Device.BeginInvokeOnMainThread(() => CaricaIsola());
            //ViewBidoni b1 = new ViewBidoni("Carta", "Ok", "Ok2", 30);
            //ViewBidoni b2 = new ViewBidoni("Plastica", "Ok", "Ok2", 30);
            //ViewBidoni b3 = new ViewBidoni("Secco", "Ok", "Ok2", 30);
            //ViewBidoni b4 = new ViewBidoni("Umido", "Ok", "Ok2", 30);
            //StackBidoni.Children.Add(b1);
            //StackBidoni.Children.Add(b2);
            //StackBidoni.Children.Add(b3);
            //StackBidoni.Children.Add(b4);
        }

        public async void CaricaIsola() {
            var api = new ClassApiEcoControl();
            var rowIsola = await Task.Run(() => {
                return api.EseguiQueryRow("IsoleBidoni",bidoneId);
            });
            System.IO.MemoryStream IniMemSt = new System.IO.MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(rowIsola["Impostazioni"].ToString()));
            IniFile Ini = new IniFile();
            Ini.Load(IniMemSt);
            int NumeroBidoni = 0;
            for (NumeroBidoni = 0; NumeroBidoni<=20; NumeroBidoni++) {
                if (Ini.GetSection("Bidoni" + NumeroBidoni.ToString())==null) return;
            }
            var c = 0;
        }
    }
}