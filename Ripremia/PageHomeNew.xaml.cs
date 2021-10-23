using Plugin.StoreReview;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.StoreReview;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using Plugin.BluetoothLE;
using System.Reactive.Linq;
using System.Threading;

namespace EcoServiceApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    
    public partial class PageHomeNew : ContentPage {
        const int RSSIShow = -70;
        const int RSSIHide = -80;
        private class _DeviceBle : IEquatable<_DeviceBle> {
            public Boolean Show = false;
            public String NomeBle = "";
            public int RSSI = 0;
            public IDevice scanResult;
            public bool Equals(_DeviceBle other) {
                return NomeBle == other.NomeBle;
            }
        }
        private List<_DeviceBle> ListaDeviceBle = new List<_DeviceBle>();

        private List<String> ElencoBLE = new List<String>();
        private List<Guid> ElencoUUID = new List<Guid>();
        private IDisposable ScanBle;
        //IDevice currentDevice = null;
        //IGattCharacteristic currentChar = null;
        public IList<Statistica> Statistiche { get; private set; }

        public class Statistica {
            public string Image { get; set; }
            public string Dati { get; set; }
            public string Dettagli { get; set; }
        }
        protected override void OnAppearing() {
            base.OnAppearing();
            Task.Run(() => {
                Task.Delay(5000); //5000
                Device.BeginInvokeOnMainThread(() => BtnShowQR_Tapped(null, null));
            });

            try {
                if (CrossStoreReview.IsSupported == true) {
                    BtnVoteAppFrame.IsVisible = true;
                } else BtnVoteAppFrame.IsVisible = false;
            } catch (Exception) { }

        }

        public void StartBleScan() {
            //Animazione
            var aniParent = new Animation();
            var ani1 = new Animation(v => ImgBle.Opacity = v, 0.5, 1, Easing.SpringIn);
            var ani2 = new Animation(v => ImgBle.Opacity = v, 1, 0.5);
            aniParent.Add(0, 0.5, ani1); aniParent.Add(0.5, 1, ani2);
            ImgBle.Animate("anible", aniParent, 16, 800, null, null, () => true);
            //
            CrossBleAdapter.Current.StopScan();
            var config = new ScanConfig();
            config.ScanType = BleScanType.Balanced;
            ListaDeviceBle.Clear();
            //return; //DA TOGLIERE
            ScanBle = CrossBleAdapter.Current.Scan(config).Subscribe(scanresult => {
                if (scanresult.Device.Name == null) return;
                if (scanresult.Device.Name.StartsWith("EC")==false) return;
                lock (ListaDeviceBle) {
                    var dev = ListaDeviceBle.FirstOrDefault<_DeviceBle>(x => { return x.NomeBle == scanresult.Device.Name; });
                    if (dev == null) { dev = new _DeviceBle(); ListaDeviceBle.Add(dev); }
                    dev.NomeBle = scanresult.Device.Name;
                    dev.RSSI = scanresult.Rssi;
                    dev.scanResult = scanresult.Device;
                }
            });
            while (true) {
                System.Threading.Thread.Sleep(500);
                
                lock (ListaDeviceBle) {
                    int count = 0;
                    foreach (var x in ListaDeviceBle) {
                        if (x.RSSI > RSSIShow) x.Show = true;
                        if (x.RSSI < RSSIHide) x.Show = false;
                        if (x.Show == true) count++;
                    }
                    if (count > 0) Device.BeginInvokeOnMainThread(() => {
                        FrameShowBle.IsVisible = true;
                    });
                    if (count == 0) Device.BeginInvokeOnMainThread(() => {
                        FrameShowBle.IsVisible = false;
                    });
                }
            }

        }




        public PageHomeNew() {
            InitializeComponent();
            Task.Run(() => {
                Task.Delay(1500); //1500
                CreateStatisticheCollection();
                Device.BeginInvokeOnMainThread(() => BindingContext = this);
                ControllaSegnalazioni();
            });
            //BLE
            Device.BeginInvokeOnMainThread(async () => {
                PermissionStatus r = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (r != PermissionStatus.Granted) r = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (r != PermissionStatus.Granted) {
                    await DisplayAlert("", "Periferiche BLE non scansionabili senza questo permesso!", "OK");
                    return;
                }
                var ThBleScan = new System.Threading.Thread(StartBleScan);
                ThBleScan.Start();
            });



            LblUtente.Text = "Ciao, " + App.DataRowUtente["Nome"].ToString() + "!";
            MenuTop.MenuLaterale = MenuLaterale;
        }
        public void ControllaSegnalazioni() {
            if (App.DataRowComune["AvvisoAbilitaAutomatico"].ToString() == "1") {
                var parchetto = new ClassApiParco();
                var NumSegn = Convert.ToInt32(parchetto.EseguiCommand($"Select Count(*) From Segnalazioni Where Data>'{DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd")}' And Data<'{DateTime.Now.ToString("yyyy-MM-dd")}' And IdComune={App.DataRowUtente["IdComune"]} And Problema!='Altro'"));
                if (NumSegn >= 2) {
                    FrmAvviso.IsVisible = true;
                    var TableSegnalazioni = parchetto.EseguiQuery($"Select *,(Select Via From Point Where Id=Segnalazioni.IdPoint) as Via,(Select Comune From Point Where Id=Segnalazioni.IdPoint) as Comune From Segnalazioni Where IdComune={App.DataRowUtente["IdComune"]} And Problema!='Altro' Order By Data desc  limit 3");
                    if (TableSegnalazioni.Rows.Count > 0) {
                        var sito = "";
                        sito = Funzioni.Antinull(TableSegnalazioni.Rows[0]["Via"]);
                        if (sito.Length < 4) sito = Funzioni.Antinull(TableSegnalazioni.Rows[0]["Comune"]);
                        LblWarning.Text = $"Sono state segnalate anomalie nel funzionamento del point sito in {sito} che riguardano: {TableSegnalazioni.Rows[0]["Problema"]}";
                    }
                }
            }
            if (App.DataRowComune["AvvisoMalfunzionamenti"].ToString().Length > 1) {
                FrmAvviso.IsVisible = true;
                LblWarning.Text = App.DataRowComune["AvvisoMalfunzionamenti"].ToString();
            }
        }

        void CreateStatisticheCollection() {
            Statistiche = new List<Statistica>();


            if (((Boolean)App.DataRowComune["ConferimentoPet"]) == true) {
                Statistiche.Add(new Statistica {
                    Image = "plastica",
                    Dati = UtenteDatiMemoria.UtentePetRaccolto.ToString(),
                    Dettagli = "Bottiglie di plastica raccolte",
                });
            }
            if (((Boolean)App.DataRowComune["ConferimentoVetro"]) == true) {
                Statistiche.Add(new Statistica {
                    Image = "vetro",
                    Dati = UtenteDatiMemoria.UtenteVetroRaccolto.ToString(),
                    Dettagli = "Bottiglie di vetro raccolte",
                });
            }
            if (((Boolean)App.DataRowComune["ConferimentoOlio"]) == true) {
                Statistiche.Add(new Statistica {
                    Image = "oil",
                    Dati = UtenteDatiMemoria.UtenteOilRaccolto.ToString("0.00").Replace(".", ","),
                    Dettagli = "Litri di olio raccolto",
                });
            }
            Statistiche.Add(new Statistica {
                Image = "co2",
                Dati = UtenteDatiMemoria.UtenteKgCO2Risparmiato.ToString("0.000").Replace(".", ","),
                Dettagli = "Kg di CO2 risparmiati",
            });

            Statistiche.Add(new Statistica {
                Image = "petrolio",
                Dati = UtenteDatiMemoria.UtenteBariliPetrolioRisparmiato.ToString("0.000").Replace(".", ","),
                Dettagli = "Barili di petrolio risparmiati",
            });



        }





        private async void ImgMenu_Tapped(object sender, EventArgs e) {
            MenuLaterale.IsVisible = true;
            await MenuLaterale.Mostra();
        }


        int Stato = 0;
        private async void BtnShowQR_Tapped(object sender, EventArgs e) {

            //CrossStoreReview.Current.OpenStoreReviewPage("com.companyname.ecolanappandroid");
            //


            //return;
            try {
                BarCodeId.BarcodeValue = Xamarin.Essentials.Preferences.Get("QrCodeNew", "1");
                if (Stato == 0) {
                    await BtnShowQRFrame.TranslateTo(1, -2, 100);
                    await BtnShowQRFrame.TranslateTo(0, 0, 100);
                    ImgArrow.IsVisible = false;
                    _ = StackBarCodeId.FadeTo(1, 250);
                    await ImgUp.FadeTo(0, 500);
                    _ = ZoomQRcode.ScaleTo(0.2, 1);
                    //_ = ZoomQRcode.TranslateTo(-40, -40, 600);
                    _ = ZoomQRcode.RotateTo(360, 350);
                    await ZoomQRcode.ScaleTo(1, 600);
                    _ = ZoomQRcode.RotateTo(-360, 1);
                    Stato = 1;
                    _ = BtnShowQRtxt.FadeTo(1, 1);
                    _ = BtnShowQRtxt.FadeTo(0, 250);
                    await BtnUnshowQRtxt.FadeTo(0, 250);
                    _ = BtnUnshowQRtxt.FadeTo(1, 250);
                    BtnShowQRtxt.IsVisible = false;
                    BtnUnshowQRtxt.IsVisible = true;

                } else {
                    await BtnShowQRFrame.TranslateTo(1, -2, 100);
                    await BtnShowQRFrame.TranslateTo(0, 0, 100);
                    ImgArrow.IsVisible = true;
                    _ = StackBarCodeId.FadeTo(0, 500);
                    await ImgUp.FadeTo(1, 500);
                    Stato = 0;
                    _ = BtnUnshowQRtxt.FadeTo(1, 1);
                    _ = BtnUnshowQRtxt.FadeTo(0, 250);
                    await BtnShowQRtxt.FadeTo(0, 250);
                    _ = BtnShowQRtxt.FadeTo(1, 250);
                    BtnShowQRtxt.IsVisible = true;
                    BtnUnshowQRtxt.IsVisible = false;

                }
            } catch (Exception) {
                var b = 0;
            }


        }

        private void BtnInfoUser_Clicked(object sender, EventArgs e) {
            DisplayAlert("", "Benvenuto in RIPREMIA, l'app che premia i cittadini virtuosi! Utilizza il QR-CODE per effettuare le tue operazioni! Controlla le tue statistiche e il tuo contributo per l'ambiente!", "ok");
        }

        private void TapGestureRecognizer_Tapped_1(object sender, EventArgs e) {
            DisplayAlert("prova", "prova", "ok");
        }

        private void TapNumeriUtili_Tapped(object sender, EventArgs e) {
            DisplayAlert("prova", "prova", "ok");
        }

        ClassApiParco Parchetto = new ClassApiParco();
        private void BtnHomeLight_Clicked(object sender, EventArgs e) {
            Application.Current.MainPage = new PageHomeLight();
            Task.Run(() => { Parchetto.EseguiCommand("Update Utente Set VersLight=1 Where Id=" + App.DataRowUtente["Id"]); });
        }

        private async void BtnVoteApp_Tapped(object sender, EventArgs e) {
            try {
                if (CrossStoreReview.IsSupported == true) {
                    if (Preferences.ContainsKey("votato")) {
                        CrossStoreReview.Current.OpenStoreReviewPage("com.companyname.ecolanappandroid");
                    } else {
                        await CrossStoreReview.Current.RequestReview(false);
                        Preferences.Set("votato", true);
                    }
                }
            } catch (Exception) {
                BtnVoteAppFrame.IsVisible = false;
            }

        }

        private void BtnSegnala_Tapped(object sender, EventArgs e) {
            if (Preferences.ContainsKey("SegnalazioneInviata") == true) {
                if (Preferences.Get("SegnalazioneInviata", DateTime.MinValue).Day == DateTime.Now.Day) {
                    DisplayAlert("", "Segnalazione già inviata!", "OK");
                    return;
                }
            }
            Application.Current.MainPage = new PageSegnalazioni();

        }

        private void BtnConferisciRifiuti_Clicked(object sender, EventArgs e) {

        }

        private void BtnPrelevaH2o_Clicked(object sender, EventArgs e) {

        }

        private void BtnPrelevaProdotti_Clicked(object sender, EventArgs e) {

        }

        private void ThreadBleConnessione() {

        }

        private async void BtnCollegaBle_Tapped(object sender, EventArgs e) {
            if (ListaDeviceBle.Count == 0) return;
            var ListaDeviceOrder = ListaDeviceBle.OrderByDescending((x) => x.RSSI);
            var DeviceSelezionato = ListaDeviceOrder.First();
            
            //if (DeviceSelezionato.RSSI < RSSIShow) {
            //    await DisplayAlert("", "Sei troppo distante da dispositivo! Avvicinati! Power:" + DeviceSelezionato.RSSI.ToString(), "OK");
            //    return;
            //}
            
            var CurrentBle = DeviceSelezionato.scanResult;
            var Page = new PageBle(CurrentBle);
            await Navigation.PushAsync(Page);

            


            //var tfinish=await Task.WhenAny(t1, Task.Delay(15000));
            //if (tfinish == t1) {
            //    var z=0;
            //} else {
            //    CurrentBle.CancelConnection();
            //    await DisplayAlert("", "Errore Timeout connessione", "OK");
            //}

            //var read=await currentChar.Read();
            //await DisplayAlert("", Convert.ToString(read.Data),"OK");
            //_ = Task.Run(() => {
            //    var api = new ClassApiEcoControl();
            //    var row=api.EseguiQueryRow("IsoleBidoni",$"NomeBidone='{DeviceSelezionato.NomeBle.Remove(0, 3)}'");
            //    if (row != null) {
            //        Device.BeginInvokeOnMainThread(() => {
            //            DisplayAlert("","Perc0:" + row["Perc0"].ToString(), "OK");
            //        });
            //    }

            //});

        }

        
    }
}
