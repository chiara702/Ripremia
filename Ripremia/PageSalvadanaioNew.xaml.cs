using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageSalvadanaioNew : ContentPage {
        public PageSalvadanaioNew() {
            InitializeComponent();
            LblUtente.Text = "Ciao, " + App.DataRowUtente["Nome"].ToString() + "!";
            MenuTop.MenuLaterale = MenuLaterale;
                
        }

        protected override void OnAppearing() {
            base.OnAppearing();
            Task.Run(() => RiempiDati());
            TxtPetConferito.Text = UtenteDatiMemoria.UtentePetRaccolto.ToString();
            TxtOilConferito.Text = UtenteDatiMemoria.UtenteOilRaccolto.ToString();
        }

        public void RiempiDati() {
            var apiEcoControl = new ClassApiEcoControl();
            int Qta = Convert.ToInt32(apiEcoControl.EseguiCommand("Select Qta From MonetaVirtuale Where QrCode='" + Xamarin.Essentials.Preferences.Get("QrCodeNew", "") + "'"));
            Device.BeginInvokeOnMainThread(() => {
                LblPunteggioFamiglia.Text = Qta.ToString();
                LblAcquaDisp.Text = (Qta / 5).ToString();
                LblSaponeDisp.Text = (Qta / 80 * 100).ToString();
                LblSaccOrgDisp.Text = (Qta / 120).ToString();
                LblSaccSeccoDisp.Text = (Qta / 100).ToString();

            });

        }




    }
}