using Plugin.BluetoothLE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageBle : ContentPage {
        private IDevice CurrentBle;
        public PageBle(IDevice DeviceBle) {
            CurrentBle = DeviceBle;
            InitializeComponent();
            Task.Run(ConnessioneBle);
            Task.Run(() => {
                Task.Delay(5000).Wait();
                Device.BeginInvokeOnMainThread(() => {
                    BtnOk.IsVisible = true;
                });
            });
            Task.Run(() => {
                Device.BeginInvokeOnMainThread(() => Act1.IsVisible=true);
                var api = new ClassApiEcoControl();
                var TableIs = api.EseguiQuery($"Select Perc0,Perc1,Perc2,Perc3,Perc4,Perc5,Impostazioni From IsoleBidoni Where NomeBidone='{CurrentBle.Name.Remove(0, 3)}'");
                Console.WriteLine("---> Request Ok");
                if (TableIs != null && TableIs.Rows.Count > 0) {
                    var RowIs = TableIs.Rows[0];
                    Device.BeginInvokeOnMainThread(() => {
                        var ConfigIsola = new ClassConfigurazioneIsola();
                        ConfigIsola.LeggiConfigurazioneIsola(RowIs["Impostazioni"].ToString());
                        var NumBidoni = ConfigIsola.ListaBidoni.Count;
                        GridBidoni.RowDefinitions.Clear();
                        GridBidoni.ColumnDefinitions.Clear();
                        GridBidoni.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                        if (NumBidoni > 2) GridBidoni.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                        if (NumBidoni > 4) GridBidoni.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                        if (NumBidoni > 0) GridBidoni.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                        if (NumBidoni > 1) GridBidoni.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                        for (int x=0; x<=NumBidoni-1; x++) {
                            var viewBidone = new Ble.ViewBidoni(ConfigIsola.ListaBidoni[x].Tipo, RowIs["Perc" + x.ToString()].ToString() + " %", ConfigIsola.ListaBidoni[x].SottoTipo, Convert.ToInt32(RowIs["Perc" + x.ToString()].ToString()));
                            viewBidone.Indice = x;
                            viewBidone.Clicked += async (s,e) => {
                                var tmp = "PORTELLA" + Convert.ToChar(65+viewBidone.Indice);
                                if (currentChar != null) await currentChar.Write(UTF8Encoding.UTF8.GetBytes("PORTELLA" + Convert.ToChar(65 + viewBidone.Indice)));
                                viewBidone.TxtInApertura.IsVisible = true;
                            };
                            GridBidoni.Children.Add(viewBidone, x % 2, x / 2);
                        }
                        Act1.IsVisible = false;
                    });
                }
            });
            
        }
        IGattCharacteristic currentChar = null;
        public async void ConnessioneBle() {
            try {
                CurrentBle.CancelConnection();
                await CurrentBle.ConnectWait();
                Device.BeginInvokeOnMainThread(() => { TxtIdBidone.Text = CurrentBle.Name; });
                Console.WriteLine("---> Connesso");
                var Services = await CurrentBle.DiscoverServices();
                Console.WriteLine("---> DiscoveryServices");
                var Characteristics = await Services.DiscoverCharacteristics();
                Console.WriteLine("---> DiscoveryChar");
                currentChar = await Services.GetKnownCharacteristics(new Guid("a8261b36-07ea-f5b7-8846-e1363f48b5be"));
                Console.WriteLine("---> ConnectChar");
                await currentChar.EnableNotifications(true);
                Console.WriteLine("---> NotifyOk");
                await CurrentBle.RequestMtu(128);
                Console.WriteLine("---> MtuOk");
                await currentChar.Write(UTF8Encoding.UTF8.GetBytes("OPEN " + App.DataRowUtente["CodiceFiscale"].ToString()));
                Console.WriteLine("---> WriteOk");

                currentChar.WhenNotificationReceived().Subscribe(n => {
                    Console.WriteLine("---> ReadOk: " + UTF8Encoding.UTF8.GetString(n.Data));
                });
                await Task.Delay(15000);
                CurrentBle.CancelConnection();
                Device.BeginInvokeOnMainThread(()=> { 
                    Navigation.PopAsync(); 
                });
            } catch (Exception err) {
                var z = 0;
                CurrentBle.CancelConnection();
            }
        }

        private void BtnOk_Clicked(object sender, EventArgs e) {
            CurrentBle.CancelConnection();
            Navigation.PopAsync();
        }
    }
}