
using Plugin.FirebasePushNotification;
using System;
using System.Data;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcoServiceApp {
    public interface IMultiPlatform {
        public Task<int> RequestTrackingAuth();
    }
    public partial class App : Application {
        //public static DataRow RowUtente;
        public static DataRow DataRowUtente;
        public static DataRow DataRowComune;
        public static DataRow DataRowSuperUser;
        public static DataRow DataRowCommerciante;
        //internal static object DataRowCliente;

        public static IMultiPlatform multiPlatform = DependencyService.Get<IMultiPlatform>();
        

        public App() {
            
            InitializeComponent();
            //Application.Current.UserAppTheme = OSAppTheme.Light;
            
            Device.SetFlags(new string[] { "Expander_Experimental" });

            //MainPage = new MainPage();
            //MainPage = new Pagemap();
            //return;

            

            if (Xamarin.Essentials.Preferences.Get("Loggato", false) == false) {
                MainPage = new NavigationPage(new PagePresentazione());
                
                //MainPage = new PagePresentazione();
            } else {
                //MainPage = new PageLoading();
                MainPage = new NavigationPage(new PageLoading());
                
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
            //Task t1=Task.Run(() => UtenteDatiMemoria.Inizializza());
            DataRowCommerciante = Parchetto.EseguiQueryRow("AttivitaCommerciali", (int)App.DataRowUtente["AttivitaCommercialiId"]);
        }
        public static void UpdateRowUtente() {
            var Parchetto = new ClassApiParco();
            var Email = Xamarin.Essentials.Preferences.Get("Email", "");
            var TmpRowUtente = Parchetto.EseguiQueryRow("Utente", "Email='" + Funzioni.AntiAp(Email) + "'");
            if (TmpRowUtente is null == false) DataRowUtente = TmpRowUtente;
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

    public static class CalcoloStatistiche {
        public static double UtenteKgCO2Risparmiato() {
            Double RisparmioLPetrolio1Pet = 0.04542; //litri di petrolio risparmiati per ogni pet
            Double RisparmioBariliPetrolio1BottVetro = 0.0006853; //barili di petrolio risparmiati per ogni bottiglia vetro in media 600g (di solito da 300g a 900g)
            int BarilePetrolioL = 159;
            Double KgCo2x1PET = 0.108; //kg Co2
            Double KgCo2x1BottVetro = 0.6; //kg Co2 per 1 bottiglia vetro
            Double KgCo2x1KgOlio = 2.5; //kg co2 per 1 litro di olio conferito trasfromato
            int CountVetro = Convert.ToInt32(App.DataRowUtente["CountVetro"]);
            int CountPet = Convert.ToInt32(App.DataRowUtente["CountPet"]);
            double KgOlio = Convert.ToDouble(App.DataRowUtente["KgOlio"]);
            Double UtenteBariliPetrolioRisparmiato = (CountVetro * RisparmioBariliPetrolio1BottVetro) + ((CountPet * RisparmioLPetrolio1Pet) / BarilePetrolioL); //Mettere una precisione di 3 numeri dopo la virgola
            Double UtenteKgCO2Risparmiato = (CountVetro * KgCo2x1BottVetro) + (KgOlio * KgCo2x1KgOlio) + (CountPet * KgCo2x1PET); //tenendo conto che 1 tonnellata PET prodotta da zero (non da riciclo) immette nell'ambiente 2,7 tonnellate di CO2 1 bottiglia di pet da 1,5 l pesa 40 g quindi una t di PET sono 25000 pezzi
            return UtenteKgCO2Risparmiato;
        }
        public static double UtenteBariliPetrolioRisparmiato() {
            Double RisparmioLPetrolio1Pet = 0.04542; //litri di petrolio risparmiati per ogni pet
            Double RisparmioBariliPetrolio1BottVetro = 0.0006853; //barili di petrolio risparmiati per ogni bottiglia vetro in media 600g (di solito da 300g a 900g)
            int BarilePetrolioL = 159;
            Double KgCo2x1PET = 0.108; //kg Co2
            Double KgCo2x1BottVetro = 0.6; //kg Co2 per 1 bottiglia vetro
            Double KgCo2x1KgOlio = 2.5; //kg co2 per 1 litro di olio conferito trasfromato
            int CountVetro = Convert.ToInt32(App.DataRowUtente["CountVetro"]);
            int CountPet = Convert.ToInt32(App.DataRowUtente["CountPet"]);
            double KgOlio = Convert.ToDouble(App.DataRowUtente["KgOlio"]);
            Double UtenteBariliPetrolioRisparmiato = (CountVetro * RisparmioBariliPetrolio1BottVetro) + ((CountPet * RisparmioLPetrolio1Pet) / BarilePetrolioL); //Mettere una precisione di 3 numeri dopo la virgola
            Double UtenteKgCO2Risparmiato = (CountVetro * KgCo2x1BottVetro) + (KgOlio * KgCo2x1KgOlio) + (CountPet * KgCo2x1PET); //tenendo conto che 1 tonnellata PET prodotta da zero (non da riciclo) immette nell'ambiente 2,7 tonnellate di CO2 1 bottiglia di pet da 1,5 l pesa 40 g quindi una t di PET sono 25000 pezzi
            return UtenteBariliPetrolioRisparmiato;
        }
    }

    /*public static class UtenteDatiMemoria {
        //public static int TotaliLitriErogati = 0;
        public static int UtentePetRaccolto = 0;
        public static int UtenteVetroRaccolto = 0;
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
            Preferences.Set("VetroRaccolto", UtenteVetroRaccolto);
            Preferences.Set("OilRaccolto", UtenteOilRaccolto);

        }
        private static void Leggi() {
            //TotaliLitriErogati=Preferences.Get("DatiMemoriaTotaliLitriErogati",0);
            UtentePetRaccolto = Convert.ToInt32(Preferences.Get("PetRaccolto", 0));
            UtenteVetroRaccolto = Convert.ToInt32(Preferences.Get("VetroRaccolto", 0));
            UtenteOilRaccolto = Convert.ToDouble(Preferences.Get("OilRaccolto", 0.0));
            CalcoloStat();
        }
        private static void CalcoloStat() {
            Double RisparmioLPetrolio1Pet = 0.04542; //litri di petrolio risparmiati per ogni pet
            Double RisparmioBariliPetrolio1BottVetro = 0.0006853; //barili di petrolio risparmiati per ogni bottiglia vetro in media 600g (di solito da 300g a 900g)
            int BarilePetrolioL = 159;
            Double KgCo2x1PET = 0.108; //kg Co2
            Double KgCo2x1BottVetro = 0.6; //kg Co2 per 1 bottiglia vetro
            Double KgCo2x1KgOlio = 2.5; //kg co2 per 1 litro di olio conferito trasfromato
            //UtentePetRaccolto = CountPet;
            //UtenteOilRaccolto = (Double)KgOlio;
            UtenteBariliPetrolioRisparmiato = (UtenteVetroRaccolto * RisparmioBariliPetrolio1BottVetro) + ((UtentePetRaccolto * RisparmioLPetrolio1Pet) / BarilePetrolioL); //Mettere una precisione di 3 numeri dopo la virgola
            UtenteKgCO2Risparmiato = (UtenteVetroRaccolto * KgCo2x1BottVetro) + (UtenteOilRaccolto * KgCo2x1KgOlio) + (UtentePetRaccolto * KgCo2x1PET); //tenendo conto che 1 tonnellata PET prodotta da zero (non da riciclo) immette nell'ambiente 2,7 tonnellate di CO2 1 bottiglia di pet da 1,5 l pesa 40 g quindi una t di PET sono 25000 pezzi
               
            //aggiunte statistiche vetro

        }


        private static void CaricaDatiUtente() {
            var Api = new ClassApiEcoControl();
            UtentePetRaccolto = int.Parse(Api.EseguiCommand("Select Sum(CountPet) From RegistroUtenti Where (Telefono='" + App.DataRowUtente["CodiceFiscale"].ToString() + "' Or Telefono='" + App.DataRowUtente["CodiceMonetaVirtuale"].ToString() + "') And Data>'2020-01-01'").ToString());
            UtenteVetroRaccolto = int.Parse(Api.EseguiCommand("Select Sum(CountVetro) From RegistroUtenti Where (Telefono='" + App.DataRowUtente["CodiceFiscale"].ToString() + "' Or Telefono='" + App.DataRowUtente["CodiceMonetaVirtuale"].ToString() + "') And Data>'2020-01-01'").ToString());
            UtenteOilRaccolto = Double.Parse(Api.EseguiCommand("Select Sum(KgOlio) From RegistroUtenti Where (Telefono='" + App.DataRowUtente["CodiceFiscale"].ToString() + "' Or Telefono='" + App.DataRowUtente["CodiceMonetaVirtuale"].ToString() + "') And Data>'2020-01-01'").ToString());
            CalcoloStat();
            Salva();
        }
    }*/

    public static class PushApi {
        public static void Inizializza() {
            //Handle notification when app is open
            //CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) => {
            //    var Messaggio = p.Data["body"].ToString();
            //    Boolean Flash = true;
            //    if (p.Data.ContainsKey("flash")==true) Flash=Boolean.Parse(p.Data["flash"].ToString());
            //    if (Flash == true) {
            //        Device.BeginInvokeOnMainThread(async () => {
            //            try {
            //                await Application.Current.MainPage.DisplayAlert("", Messaggio, "OK");
            //            } catch (Exception) { }
            //        });
            //    } else {
            //        Device.BeginInvokeOnMainThread(() => {
            //            App.Current.MainPage = new PageNotifiche();
            //        });
            //    }
            //};
            /*CrossFirebasePushNotification.Current.OnTokenRefresh += (s, p) => {
                CrossFirebasePushNotification.Current.Subscribe("SUPERADMIN");
            };*/
            //CrossFirebasePushNotification.Current.OnNotificationOpened += (s, p) =>{
            //    var Messaggio = p.Data["body"].ToString();
                
            //};

        }

        public static void ResetTopics() {
            return; //Da togliere

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
