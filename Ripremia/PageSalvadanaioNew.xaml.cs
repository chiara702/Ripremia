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
            if (((Boolean)App.DataRowComune["RaccoltaCoupon"]) == false) {
                FrmCoupon.IsVisible = false;
            }
        }

        protected override void OnAppearing() {
            base.OnAppearing();
            Task.Run(() => RiempiDati());
            //Task.Run(() => RiempiFrameCoupon());
            TxtPetConferito.Text = UtenteDatiMemoria.UtentePetRaccolto.ToString();
            TxtOilConferito.Text = UtenteDatiMemoria.UtenteOilRaccolto.ToString();
            TxtVetroConferito.Text = UtenteDatiMemoria.UtenteVetroRaccolto.ToString();
            Device.StartTimer(TimeSpan.FromSeconds(20), () => {
                RiempiDati();
                return true;
            });


        }
        

        public void RiempiDati() {
            try {
                var apiEcoControl = new ClassApiEcoControl();
                int Qta = Convert.ToInt32(apiEcoControl.EseguiCommand("Select Qta From MonetaVirtuale Where QrCode='" + App.DataRowUtente["CodiceMonetaVirtuale"].ToString() + "'"));
                Device.BeginInvokeOnMainThread(() => {
                    LblPunteggioFamiglia.Text = Qta.ToString();
                    LblAcquaDisp.Text = (Qta / 5).ToString();
                    LblSaponeDisp.Text = (Qta / 80 * 100).ToString();
                    LblSaccOrgDisp.Text = (Qta / 120).ToString();
                    LblSaccSeccoDisp.Text = (Qta / 100).ToString();
                    RiempiFrameCoupon();
                });
            } catch (Exception) {
                DisplayAlert("Errore", "Errore recupero statistiche", "OK");
            }

        }

        public void RiempiFrameCoupon() {
            if ((Boolean)App.DataRowComune["RaccoltaCoupon"] == false) return;
            int Coupon = (int)App.DataRowUtente["Coupon"];
            int FattoreConversione = (int)App.DataRowComune["EcopuntiCoupon"];
            int MoltiplicatoreCoupon = (int)App.DataRowComune["MoltiplicatoreCoupon"];
            int GeneratoreCoupon = (int)App.DataRowUtente["GeneratoreCoupon"];
            if (FattoreConversione == 0) return;
            var Parchetto = new ClassApiParco();
            var Par = Parchetto.GetParam();

            if (GeneratoreCoupon >= FattoreConversione) {
                Coupon = Coupon + (1 * MoltiplicatoreCoupon);
                GeneratoreCoupon = GeneratoreCoupon - FattoreConversione;
                Par.AddParameterInteger("Coupon", Coupon);
                Par.AddParameterInteger("GeneratoreCoupon", GeneratoreCoupon);
                Parchetto.EseguiUpdate("Utente", (int)App.DataRowUtente["Id"], Par);

            }

            Device.BeginInvokeOnMainThread(() => {              
                double ProgressCalc = (double)GeneratoreCoupon / (double)FattoreConversione;
                BarCoupon.Progress = ProgressCalc;
                LblCouponQnt.Text = Coupon.ToString();
                LblEcopuntiParz.Text =  "Sei al " + GeneratoreCoupon.ToString() + "%";

                if (MoltiplicatoreCoupon > 1) {
                    LblSpiegaGeneratore.Text = "Ogni " + App.DataRowComune["EcopuntiCoupon"].ToString() + " ecopunti accumulati verranno generati " + (int)App.DataRowComune["MoltiplicatoreCoupon"] + " COUPON di SCONTO che potrai utilizzare presso le attività commerciali affiliate*";
                } else {
                    LblSpiegaGeneratore.Text = "Ogni " + App.DataRowComune["EcopuntiCoupon"].ToString() + " ecopunti accumulati verrà generato 1 COUPON di SCONTO che potrai utilizzare presso le attività commerciali affiliate*";
                }

            });
        }
        private void BtnInfoUser_Clicked(object sender, EventArgs e) {
            DisplayAlert("", "I cittadini virtuosi meritano di essere premiati! E noi vogliamo premiarvi con la raccolta di Ecopunti ed eventuali Coupon di sconto! Non dimenticare che il fine non è guadagnare, ma fare delle buone azioni per la tutela dell'ambiente!", "OK");
        }
        private void BtnShowAttivita_Clicked(object sender, EventArgs e) {
            Application.Current.MainPage = new NavigationPage(new PageShowAttivitaCommerciali());

        }
    }
}