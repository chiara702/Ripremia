
using Plugin.FirebasePushNotification;
using System;
using System.Data;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp {
    public partial class App : Application {
        //public static DataRow RowUtente;
        public static DataRow DataRowUtente;
        public static DataRow DataRowComune;
        public static DataRow DataRowSuperUser;

        public App() {
            InitializeComponent();
            
            Device.SetFlags(new string[] { "Expander_Experimental" });

            //MainPage = new MainPage();
            //MainPage = new Pagemap();
            //return;

            if (Xamarin.Essentials.Preferences.Get("Loggato", false) == false) {
                MainPage = new PagePresentazione();
            } else {
                
                MainPage = new PageLoading();
            }

            PushApi.Inizializza();

        }
        public static void InizializzaDatiApp() {
            var Email = Xamarin.Essentials.Preferences.Get("Email", "");
            if ( Email == "") return;
            var Parchetto = new ClassApiParco();
            DataRowUtente = Parchetto.EseguiQueryRow("Utente", "Email='" + Funzioni.AntiAp(Email) + "'");
            if (DataRowUtente == null) return;
            DataRowComune = Parchetto.EseguiQueryRow("Comune", "Id=" + DataRowUtente["IdComune"].ToString());
            if (DataRowComune == null) return;
            DataRowSuperUser = Parchetto.EseguiQueryRow("SuperUser", "Codice='" + DataRowComune["CodiceSuperUser"].ToString() + "'");
            Task t1=Task.Run(() => UtenteDatiMemoria.Inizializza());

        }

        protected override void OnStart() {
        }

        protected override void OnSleep() {
        }

        protected override void OnResume() {
        }

        public static DateTime DataLetturaNotifiche{
            get{ return Xamarin.Essentials.Preferences.Get("DataLetturaNotifiche", DateTime.MinValue); }
            set{ Xamarin.Essentials.Preferences.Set("DataLetturaNotifiche", value); }
        }

        
    }

    public static class UtenteDatiMemoria {
        //public static int TotaliLitriErogati = 0;
        public static int UtentePetRaccolto = 0;
        public static Double UtenteOilRaccolto = 0;
        public static Double UtenteKgCO2Risparmiato = 0;
        public static Double UtenteBariliPetrolioRisparmiato = 0;
        public static int TotaliPetRaccolto = 0;
        public static int TotaliCO2Risparmiato = 0;
        public static int TotaliPetrolioRisparmiato = 0;
        private static TimeSpan TimeUpdateOnline = new TimeSpan(24, 0, 0);


        public static void Inizializza() {
            if (DateTime.Now > Preferences.Get("UpdateUtenteDatiMemoria", DateTime.MinValue).Add(TimeUpdateOnline)){
                CaricaDatiUtente();
                Preferences.Set("UpdateUtenteDatiMemoria", DateTime.Now);
            } else {
                Leggi();
            }
        }
        public static void AzzeraTimeUpdate() {
            Preferences.Set("UpdateUtenteDatiMemoria", DateTime.MinValue);
        }

       
        private static void Salva() {
            //Preferences.Set("DatiMemoriaTotaliLitriErogati", TotaliLitriErogati);
            Preferences.Set("PetRaccolto", UtentePetRaccolto);
            Preferences.Set("OilRaccolto", UtenteOilRaccolto);

        }
        private static void Leggi() {
            //TotaliLitriErogati=Preferences.Get("DatiMemoriaTotaliLitriErogati",0);
            UtentePetRaccolto = Convert.ToInt32(Preferences.Get("PetRaccolto", 0));
            UtenteOilRaccolto = Convert.ToDouble(Preferences.Get("OilRaccolto", 0.0));
            CalcoloStat();
        }
        private static void CalcoloStat() {
            Double RisparmioLPetrolio1Pet = 0.04542; //litri di petrolio risparmiati per ogni pet
            int BarilePetrolioL = 159;
            Double KgCo2x1PET = 0.108; //kg Co2
            Double KgCo2x1KgOlio = 2.5; //kg co2 per 1 litro di olio conferito trasfromato
            //UtentePetRaccolto = CountPet;
            //UtenteOilRaccolto = (Double)KgOlio;
            UtenteBariliPetrolioRisparmiato = (UtentePetRaccolto * RisparmioLPetrolio1Pet) / BarilePetrolioL; //Mettere una precisione di 3 numeri dopo la virgola
            UtenteKgCO2Risparmiato = (UtenteOilRaccolto * KgCo2x1KgOlio) + (UtentePetRaccolto * KgCo2x1PET); //tenendo conto che 1 tonnellata PET prodotta da zero (non da riciclo) immette nell'ambiente 2,7 tonnellate di CO2 1 bottiglia di pet da 1,5 l pesa 40 g quindi una t di PET sono 25000 pezzi
                                                                                                    

        }


        private static void CaricaDatiUtente() {
            var Api = new ClassApiEcoControl();
            UtentePetRaccolto = int.Parse(Api.EseguiCommand("Select Sum(CountPet) From RegistroUtenti Where (Telefono='" + App.DataRowUtente["CodiceFiscale"].ToString() + "' Or Telefono='" + App.DataRowUtente["CodiceMonetaVirtuale"].ToString() + "') And Data>'2020-01-01'").ToString());
            UtenteOilRaccolto = Double.Parse(Api.EseguiCommand("Select Sum(KgOlio) From RegistroUtenti Where (Telefono='" + App.DataRowUtente["CodiceFiscale"].ToString() + "' Or Telefono='" + App.DataRowUtente["CodiceMonetaVirtuale"].ToString() + "') And Data>'2020-01-01'").ToString());
            CalcoloStat();
            Salva();
        }
    }

    public static class PushApi {
        public static void Inizializza() {
            //Handle notification when app is open
            CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) => {
                var Messaggio = p.Data["body"].ToString();
                Boolean Flash = true;
                if (p.Data.ContainsKey("flash")==true) Flash=Boolean.Parse(p.Data["flash"].ToString());
                if (Flash == true) {
                    Device.BeginInvokeOnMainThread(async () => {
                        try {
                            await Application.Current.MainPage.DisplayAlert("", Messaggio + " Flash:" + Flash.ToString(), "OK");
                        } catch (Exception) { }
                    });
                } else {
                    Device.BeginInvokeOnMainThread(() => {
                        App.Current.MainPage = new PageNotifiche();
                    });
                }
            };
            /*CrossFirebasePushNotification.Current.OnTokenRefresh += (s, p) => {
                CrossFirebasePushNotification.Current.Subscribe("SUPERADMIN");
            };*/
            CrossFirebasePushNotification.Current.OnNotificationOpened += (s, p) =>{
                var Messaggio = p.Data["body"].ToString();
                
            };

        }

        public static void ResetTopics() {
            CrossFirebasePushNotification.Current.UnsubscribeAll();
            CrossFirebasePushNotification.Current.Subscribe("SUPERADMIN");
            if (App.DataRowUtente != null) {
                var SuperUserCodice = App.DataRowUtente["AdminSuperuserCode"].ToString();
                CrossFirebasePushNotification.Current.Subscribe("SUPERUSER_" + SuperUserCodice);
                var ComuneId = App.DataRowUtente["IdComune"].ToString();
                CrossFirebasePushNotification.Current.Subscribe("COMUNE_" + ComuneId);
                var MV = App.DataRowUtente["CodiceMonetaVirtuale"].ToString();
                CrossFirebasePushNotification.Current.Subscribe(MV);
                var UtenteId = App.DataRowUtente["Id"].ToString();
                CrossFirebasePushNotification.Current.Subscribe("UTENTE_" + UtenteId);
                var CF = App.DataRowUtente["CodiceFiscale"].ToString();
                CrossFirebasePushNotification.Current.Subscribe(CF);
            }
        }

       
    }


    
}
