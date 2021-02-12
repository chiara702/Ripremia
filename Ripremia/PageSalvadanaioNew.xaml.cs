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
            
            if ((((Boolean)App.DataRowComune["BorsellinoAcqua"]) == false) && (((Boolean)App.DataRowComune["BorsellinoSapone"]) == false) && ((Boolean)App.DataRowComune["BorsellinoSacchi"]) == false){
                LblCosapossofare.IsVisible = false;
                ImgPig.IsVisible = false;
            }

                if (((Boolean)App.DataRowComune["BorsellinoAcqua"]) == false) {
                LblBorsellinoAcqua0.IsVisible = false;
                LblBorsellinoAcqua1.IsVisible = false;
                LblBorsellinoAcqua2.IsVisible = false;
                LblAcquaDisp.IsVisible = false;
            }
            if (((Boolean)App.DataRowComune["BorsellinoSapone"]) == false) {
                LblBorsellinoSapone0.IsVisible = false;
                LblBorsellinoSapone1.IsVisible = false;
                LblBorsellinoSapone2.IsVisible = false;
                LblSaponeDisp.IsVisible = false;
            }
            if (((Boolean)App.DataRowComune["BorsellinoSacchi"]) == false) {
                LblBorsellinoSaccBio0.IsVisible = false;
                LblBorsellinoSaccBio1.IsVisible = false;
                LblBorsellinoSaccBio2.IsVisible = false;
                LblSaccOrgDisp.IsVisible = false;
                LblBorsellinoSacc0.IsVisible = false;
                LblBorsellinoSacc1.IsVisible = false;
                LblBorsellinoSacc2.IsVisible = false;
                LblSaccSeccoDisp.IsVisible = false;
            }

        }

        protected override void OnAppearing() {
            base.OnAppearing();
            Task.Run(() => RiempiDati());
            TxtPetConferito.Text = UtenteDatiMemoria.UtentePetRaccolto.ToString();
            TxtOilConferito.Text = UtenteDatiMemoria.UtenteOilRaccolto.ToString();
            Device.StartTimer(TimeSpan.FromSeconds(20), () => {
                RiempiDati();
                return true;
            });
        }

        public void RiempiDati() {
            var apiEcoControl = new ClassApiEcoControl();
            int Qta = Convert.ToInt32(apiEcoControl.EseguiCommand("Select Qta From MonetaVirtuale Where QrCode='" + App.DataRowUtente["CodiceMonetaVirtuale"].ToString() + "'"));
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