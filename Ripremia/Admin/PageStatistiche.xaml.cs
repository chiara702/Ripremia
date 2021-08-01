using Microcharts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp.Admin {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageStatistiche : ContentPage {
        ClassApiParco Parchetto = new ClassApiParco();
        Dictionary<int, String> Comuni;
        public PageStatistiche() {
            InitializeComponent();
            Task.Run(() => CaricaComuni());
        }


        private void CaricaComuni() {
            if (Funzioni.Antinull(App.DataRowUtente["AdminSuperUserCode"]) != "") {
                Comuni = ElencoComuni(App.DataRowUtente["AdminSuperUserCode"].ToString());
                foreach (var x in Comuni) {
                    Device.BeginInvokeOnMainThread(() => PickerComune.Items.Add(x.Value));
                }
            }
            if ((int)App.DataRowUtente["AdminComuneId"] > 0) {
                Device.BeginInvokeOnMainThread(() => PickerComune.Items.Add(Parchetto.EseguiCommand("Select Nome From Comune Where Id=" + App.DataRowUtente["AdminComuneId"]).ToString()));
            }
        }

        private Dictionary<int, String> ElencoComuni(String SuperUser) {
            var rit = new Dictionary<int, String>();
            var Table = Parchetto.EseguiQuery("Select Id,Nome From Comune Where CodiceSuperUser='" + SuperUser + "'");
            if (Parchetto.LastError == true) {
                DisplayAlert("Error", "Si è verificato un errore. Verifica connettività!", "OK");
                return null;
            }
            foreach (DataRow x in Table.Rows) {
                rit.Add(int.Parse(x["Id"].ToString()), x["Nome"].ToString());
            }
            return rit;
        }

        private void PickerComune_SelectedIndexChanged(object sender, EventArgs e) {
            StkShowAll.IsVisible = true;
            //Grafici

            var entries = new[]{
                new Microcharts.ChartEntry(1.0f){
                    Label = "Data1",
                    ValueLabel = "200",
                    Color = SkiaSharp.SKColor.Parse("#ff0000")
                },
                new Microcharts.ChartEntry(2.0f){
                    Label = "Data2",
                    ValueLabel = "400",
                    Color = SkiaSharp.SKColor.Parse("#fff600")

                },
                new Microcharts.ChartEntry(4.0f){
                    Label = "Data2",
                    ValueLabel = "400",
                    Color = SkiaSharp.SKColor.Parse("#fef600")

                },
                new Microcharts.ChartEntry(1.0f){
                    Label = "Data2",
                    ValueLabel = "400",
                    Color = SkiaSharp.SKColor.Parse("#eee600")

                },
                new Microcharts.ChartEntry(6.0f){
                    Label = "Data2",
                    ValueLabel = "400",
                    Color = SkiaSharp.SKColor.Parse("#fff600")

                }
            };
            var chart = new LineChart() { Entries = entries, LabelTextSize = 25, IsAnimated = true, AnimationProgress = 20 };
            this.chartView.Chart = chart;

        }

        protected async override void OnAppearing() {
            base.OnAppearing();
            try {
               
            } catch (Exception e) {
                await DisplayAlert("", e.Message, "ok");
            }

            

        }//Grafici

        //public virtual void ViewDidLoad() {
        //    base.ViewDidLoad();

        //    var entries = // ... see 1°) above
        //    var chart = // ... see 2°) above

        //    var chartView = new ChartView {
        //        Frame = new CGRect(0, 32, this.View.Bounds.Width, 140),
        //        AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
        //        Chart = chart,
        //    };

        //    this.View.Add(chartView);
        //}


        private void BtnIndietro_Clicked(object sender, EventArgs e) {
            Application.Current.MainPage = new PageAreaRiservata();
        }

        protected override bool OnBackButtonPressed() {
            BtnIndietro_Clicked(null, null);
            return true;
        }


        //Grafici


        //var chart = new LineChart() { Entries = entries };



    }
}