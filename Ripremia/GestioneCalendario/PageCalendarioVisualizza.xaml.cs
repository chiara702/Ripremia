﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public class Calendario {
        public int Giorno { get; set; }
        public String GiornoSettimana { get; set; }
        public String Rifiuto1 { get; set; }
        public String Rifiuto2 { get; set; }
        public Color Colore1 { get; set; }
        public Color Colore2 { get; set; }
        public int ColSpan { get; set; }
        public double Opacity { get; set; }

    }
    public partial class PageCalendarioVisualizza : ContentPage {
        public List<Calendario> ListaCalendario { get; set; } = new List<Calendario>();
        public DataTable RifiutiTipo;
        public DataTable Rifiuti;
        public DataTable RifiutiEccezioni;
        public Button[] ListaButtonMesi;
        public PageCalendarioVisualizza() {
            InitializeComponent();
            ListaButtonMesi = new Button[12] { BtnGennaio, BtnFebbraio, BtnMarzo, BtnAprile, BtnMaggio, BtnGiugno, BtnLuglio, BtnAgosto, BtnSettembre, BtnOttobre, BtnNovembre, BtnDicembre };
            EseguiSoap();
            RiempiListaCalendario(DateTime.Now.Month);
            foreach (Button x in ListaButtonMesi) x.Opacity = 0.6;
            ListaButtonMesi[DateTime.Now.Month - 1].Opacity = 1;
            UpdateMese(DateTime.Now.Month);

        }

        private void EseguiSoap() {
            var Parchetto = new ClassApiParco();
            RifiutiTipo=Parchetto.EseguiQuery("Select * From CalendarioRifiutiTipo Where ComuneId=" + App.DataRowComune["Id"].ToString());
            Rifiuti = Parchetto.EseguiQuery("Select * From CalendarioRifiuti Where ComuneId=" + App.DataRowComune["Id"].ToString());
            RifiutiEccezioni = Parchetto.EseguiQuery("Select * From CalendarioRifiutiEccezzioni Where ComuneId=" + App.DataRowComune["Id"].ToString());

            //Carico Tipi rifiuti dettagli
            foreach (DataRow x in RifiutiTipo.Rows) {
                var tmp = new ViewTipiRifiuti();
                tmp.Denominazione = x["Denominazione"].ToString();
                var rigo = x;
                tmp.Click += (s, e)=> {
                    StackInfo.IsVisible = true;
                    LblComeConferire.Text = Funzioni.Antinull(rigo["ComeConferire"]);
                    LblCosaConferire.Text = Funzioni.Antinull(rigo["CosaConferire"]);
                    LblCosaNonConferire.Text = Funzioni.Antinull(rigo["CosaNonConferire"]);
                };
                StkRifiuti.Children.Add(tmp);
            }

            

        }

        private void BtnNascondiInfo_Tapped(object sender, EventArgs e) {
            StackInfo.IsVisible = false;
        }

        public static String ColoreToHex(String Testo) {
            switch (Testo) {
                case "Azzurro":
                    return "#3091C8";
                case "Blu":
                    return ""; 
                case "Marrone":
                    return "#66431C";
                case "Giallo":
                    return "#D0C82C";
                case "Grigio":
                    return "#3F3E40";
                case "Verde":
                    return "#02A44F";
                case "Rosso":
                    return "";
                case "Bianco":
                    return "#EAEAEA";
                case "Arancio":
                    return "";
                case "LightGrigio":
                    return "";
            }
            return "";
        }

        private void RiempiListaCalendario(int Mese) {
            if (Rifiuti == null || RifiutiTipo == null) return;
            ListaCalendario.Clear();
            var Anno = DateTime.Now.Year;
            var GiorniMese = DateTime.DaysInMonth(DateTime.Now.Year, Mese);
            for (int x = 1; x <= GiorniMese; x++) {
                var DataTmp = new DateTime(Anno, Mese, x);
                var n = new Calendario();
                ListaCalendario.Add(n);
                n.Colore1 = Color.White;
                n.Colore2 = Color.White;
                n.Giorno = x;
                n.ColSpan = 1;
                n.GiornoSettimana = DataTmp.ToString("ddd",new CultureInfo("it-IT"));
                var Rifiutoddd = Rifiuti.Select("GiornoSettimana='" + DataTmp.ToString("ddd", new CultureInfo("it-IT")) + "'");
                if (Rifiutoddd.Length==0) continue;
                if ((int)Rifiutoddd[0]["Rifiuto1"] > 0) {
                    var RifiutoTipo = RifiutiTipo.Select("Id=" + Rifiutoddd[0]["Rifiuto1"]);
                    if (RifiutoTipo.Length > 0) {
                        n.Colore1 = Color.FromHex(ColoreToHex(RifiutoTipo[0]["Colore"].ToString()));
                        n.Rifiuto1 = RifiutoTipo[0]["Denominazione"].ToString();
                    }
                }
                if ((int)Rifiutoddd[0]["Rifiuto2"] > 0) {
                    var RifiutoTipo = RifiutiTipo.Select("Id=" + Rifiutoddd[0]["Rifiuto2"]);
                    if (RifiutoTipo.Length > 0) {
                        n.Colore2 = Color.FromHex(ColoreToHex(RifiutoTipo[0]["Colore"].ToString()));
                        n.Rifiuto2 = RifiutoTipo[0]["Denominazione"].ToString();
                    }
                }
                if ((int)Rifiutoddd[0]["Rifiuto2"] == 0) {
                    var RifiutoTipo = RifiutiTipo.Select("Id=" + Rifiutoddd[0]["Rifiuto1"]);
                    if (RifiutoTipo.Length > 0) {
                        n.Colore2 = Color.FromHex(ColoreToHex(RifiutoTipo[0]["Colore"].ToString()));
                        //n.Rifiuto2 = RifiutoTipo[0]["Denominazione"].ToString(); 
                        n.ColSpan = 2;
                    }
                }
                DataRow RowEcc = null;
                foreach (DataRow z in RifiutiEccezioni.Rows) {
                    if (((DateTime)z["Data"]).ToShortDateString() == DataTmp.ToShortDateString()) RowEcc = z;
                }

                if (RowEcc != null) {
                    n.ColSpan = 1;
                    var RifiutoTipo = RifiutiTipo.Select("Id=" + RowEcc["Rifiuto1"]);
                    if (RifiutoTipo.Length > 0) {
                        n.Colore1 = Color.FromHex(ColoreToHex(RifiutoTipo[0]["Colore"].ToString()));
                        n.Rifiuto1 = RifiutoTipo[0]["Denominazione"].ToString();
                    }
                    RifiutoTipo = RifiutiTipo.Select("Id=" + RowEcc["Rifiuto2"]);
                    if (RifiutoTipo.Length > 0) {
                        n.Colore2 = Color.FromHex(ColoreToHex(RifiutoTipo[0]["Colore"].ToString()));
                        n.Rifiuto2 = RifiutoTipo[0]["Denominazione"].ToString();
                    }
                    if (n.Rifiuto2 == null) { n.ColSpan = 2; n.Colore2 = n.Colore1; }

                }
                if (DataTmp < DateTime.Now) {
                    n.Opacity = 0.6;
                } else {
                    n.Opacity = 1;
                }


            }
            Device.BeginInvokeOnMainThread(() => { 
                BindingContext = null;
                BindingContext = this;
            });
        }

        

        private void BtnIndietro_Clicked(object sender, EventArgs e) {
            var Page = new PageNavigatore();
            Page.CurrentPage = Page.Children[3];
            Application.Current.MainPage = Page;
        }
        protected override bool OnBackButtonPressed() {
            BtnIndietro_Clicked(null, null);
            return true;
        }

        private void BtnMese_Clicked(object sender, EventArgs e) {
            Button btn = (Button)sender;
            foreach (Button x in ListaButtonMesi) x.Opacity = 0.6;
            btn.Opacity = 1;
            RiempiListaCalendario(btn.TabIndex);
            UpdateMese(btn.TabIndex);
        }
        private void UpdateMese( int Mese) {
            var MesiStringa = new String[12] { "GENNAIO", "FEBBRAIO", "MARZO", "APRILE", "MAGGIO", "GIUGNO", "LUGLIO", "AGOSTO", "SETTEMBRE", "OTTOBRE", "NOVEMBRE", "DICEMBRE" };
            LblMese.Text = MesiStringa[Mese - 1];
        }

        private void BtnRifiuto1_Tapped(object sender, EventArgs e) {

        }

        private void BtnEccezzioni_Tapped(object sender, EventArgs e) {

        }

      
    }
}