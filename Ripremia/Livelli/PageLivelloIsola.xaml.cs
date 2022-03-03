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
            var TableProblemi = await Task.Run(() => {
                return api.EseguiQuery($"Select Top(20) * From EcoControlV2Bis.dbo.RegistroProblemi Where BidoneId={rowIsola["Id"].ToString()} Order By Data desc");
            });
            LblNomeIsola.Text = rowIsola["NomeBidone"].ToString();
            LblIndirizzo.Text=rowIsola["Indirizzo"].ToString();
            try {
                if (DateTime.Parse(rowIsola["TimeLastMess"].ToString())>DateTime.Now.AddMinutes(-120)) {
                    LblStato.Text = "Funzionante";
                } else {
                    LblStato.Text = "Riscontrati problemi";
                    LblStato.TextColor = Color.Red;
                }
            } catch (Exception) { LblStato.Text = "..."; }
            System.IO.MemoryStream IniMemSt = new System.IO.MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(rowIsola["Impostazioni"].ToString()));
            IniFile Ini = new IniFile();
            Ini.Load(IniMemSt);
            int NumeroBidoni = 0;
            for (NumeroBidoni = 0; NumeroBidoni<=20; NumeroBidoni++) {
                if (Ini.GetSection("Bidone" + NumeroBidoni.ToString())==null) break;
            }
            for (int x=0; x<=NumeroBidoni-1; x++) {
                var Tipo = Ini.GetKeyValue("Bidone" + x.ToString(), "Tipo");
                var SottoTipo = Ini.GetKeyValue("Bidone" + x.ToString(), "SottoTipo");
                if (SottoTipo=="") SottoTipo=Tipo;
                ViewBidoni bidone = new ViewBidoni(SottoTipo, SottoTipo, rowIsola["Perc" + x.ToString()].ToString() + "%", (int)rowIsola["Perc" + x.ToString()]);
                StackBidoni.Children.Add(bidone);
            }
            var SelectProbVol = TableProblemi.Select("Problema='Volumetrico'");
            if (SelectProbVol.Length>0) {
                if (DateTime.Now.Subtract( DateTime.Parse(SelectProbVol[0]["Data"].ToString())).TotalMinutes < 15) {
                    LblProblema.Text = "Problema volumetrico riscontrato negli ultimi minuti";
                }
            }
            var SelectProbCh = TableProblemi.Select("Problema='Problema chiusura'");
            if (SelectProbCh.Length>0) {
                if (DateTime.Now.Subtract(DateTime.Parse(SelectProbCh[0]["Data"].ToString())).TotalMinutes < 15) {
                    LblProblema2.Text = "Problema chiusura negli ultimi minuti";
                }
            }
            var SelectProbBatt = TableProblemi.Select("Problema='Alarme Batterie'");
            if (SelectProbBatt.Length>0) {
                if (DateTime.Now.Subtract(DateTime.Parse(SelectProbBatt[0]["Data"].ToString())).TotalHours < 24) {
                    LblProblema3.Text = "Problemi batteria nelle ultime ore";
                }
            }
        }
    }
}