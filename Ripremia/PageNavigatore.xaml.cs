using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageNavigatore : Xamarin.Forms.TabbedPage {
        public PageNavigatore() {
            InitializeComponent();
            this.On<Xamarin.Forms.PlatformConfiguration.Android>().SetIsSwipePagingEnabled(false);
            if ((Boolean)App.DataRowComune["ServizioRipremia"]==false) {
                TabBorsellino.IsEnabled=false;
            }
            if ((Boolean)App.DataRowComune["ServizioRitiro"]==false && (Boolean)App.DataRowComune["ServizioCentroRiuso"]==false && (Boolean)App.DataRowComune["ServizioAbbandono"]==false && (Boolean)App.DataRowComune["ServizioCalendario"]==false) {
                TabServizi.IsEnabled=false;
            }
        }

        protected override bool OnBackButtonPressed() {
            return true;
        }

    }
}