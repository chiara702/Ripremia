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
    public partial class PageDettaglioAttivita : ContentPage {
        private ClassApiParco Parchetto = new ClassApiParco();
        private int idAttivita = 0;
        private DataRow rowAttivita;
        public PageDettaglioAttivita(int IdAttivita) {
            idAttivita = IdAttivita;
            InitializeComponent();
            CaricaAttivita();
        }
        
        public void CaricaAttivita() {
            rowAttivita = Parchetto.EseguiQueryRow("AttivitaCommerciali", idAttivita);
            Device.BeginInvokeOnMainThread(Riempi);
        }
        public void Riempi() {
            ImgLogo.Source = ImageSource.FromStream(() => { return new System.IO.MemoryStream((byte[])rowAttivita["Logo"]); });
            LblNome.Text = (string)rowAttivita["Nome"];
            LblCategoria.Text = "Categoria merceologica: " + (string)rowAttivita["CatMerceologica"];
            LblComune.Text = (string)rowAttivita["Comune"] + " (" + (string)rowAttivita["Provincia"] + ")";
            LblIndirizzo.Text = (string)rowAttivita["Via"] + ", " + (string)rowAttivita["Numero"];
            LblValoreCoupon.Text = ((double)rowAttivita["ValoreCoupon"]).ToString("0.00").Replace(".", ",") + " €";
            LblSpesaMin.Text = ((double)rowAttivita["SpesaMin"]).ToString("0.00").Replace(".", ",") + " €";
            LblCouponMax.Text = rowAttivita["MaxCouponSpesaMin"].ToString();
            LblSpiegazione.Text = "*Numero di coupon utilizzabili per ogni multiplo di spesa minima. Per esempio ogni " + ((double)rowAttivita["SpesaMin"]).ToString("0.00").Replace(".", ",") + " € di spesa avrai diritto ad utilizzare un coupon di sconto pari a " + ((double)rowAttivita["ValoreCoupon"]).ToString("0.00").Replace(".", ",") + " €";
            LblDettagli.Text = (string)rowAttivita["Dettagli"];
        }
    }
}