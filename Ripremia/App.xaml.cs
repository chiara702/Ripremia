
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
            Task.Run(() => UtenteDatiMemoria.Inizializza());
            
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
        public static int UtenteOilRaccolto = 0;
        public static int UtenteKgCO2Risparmiato = 0;
        public static int UtenteBariliPetrolioRisparmiato = 0;
        public static int TotaliPetRaccolto = 0;
        public static int TotaliCO2Risparmiato = 0;
        public static int TotaliPetrolioRisparmiato = 0;
        private static TimeSpan TimeUpdateOnline = new TimeSpan(24, 0, 0);


        public static void Inizializza() {
            if (DateTime.Now > Preferences.Get("UpdateUtenteDatiMemoria", DateTime.MinValue).Add(TimeUpdateOnline)){
                CaricaDatiUtente();
                Preferences.Set("UpdateUtenteDatiMemoria", DateTime.Now);
                Salva();
            } else {
                Leggi();
            }
        }
        public static void AzzeraTimeUpdate() {
            Preferences.Set("UpdateUtenteDatiMemoria", DateTime.MinValue);
        }

       
        private static void Salva() {
            //Preferences.Set("DatiMemoriaTotaliLitriErogati", TotaliLitriErogati);
            Preferences.Set("DatiMemoriaUtentePetRaccolto", UtentePetRaccolto);
            Preferences.Set("DatiMemoriaUtenteOilRaccolto", UtenteOilRaccolto);
            Preferences.Set("DatiMemoriaUtenteKgCo2Risparmiato", UtenteKgCO2Risparmiato);
            Preferences.Set("DatiMemoriaUtenteBariliPetrolioRisparmiato", UtenteBariliPetrolioRisparmiato);
            Preferences.Set("DatiMemoriaTotaliPetRaccolto", TotaliPetRaccolto);
            Preferences.Set("DatiMemoriaTotaliCO2Risparmiato", TotaliCO2Risparmiato);
            Preferences.Set("DatiMemoriaTotaliPetrolioRisparmiato", TotaliPetrolioRisparmiato);
        }
        private static void Leggi() {
            //TotaliLitriErogati=Preferences.Get("DatiMemoriaTotaliLitriErogati",0);
            UtentePetRaccolto=Preferences.Get("DatiMemoriaUtentePetRaccolto", 0);
            UtenteOilRaccolto=Preferences.Get("DatiMemoriaUtenteOilRaccolto", 0);
            UtenteKgCO2Risparmiato = Preferences.Get("DatiMemoriaUtenteKgCO2Risparmiato", 0);
            UtenteBariliPetrolioRisparmiato = Preferences.Get("DatiMemoriaUtenteBariliPetrolioRisparmiato", 0);
            TotaliPetRaccolto =Preferences.Get("DatiMemoriaTotaliPetRaccolto", 0);
            TotaliCO2Risparmiato=Preferences.Get("DatiMemoriaTotaliCO2Risparmiato", 0);
            TotaliPetrolioRisparmiato=Preferences.Get("DatiMemoriaTotaliPetrolioRisparmiato", 0);
        }


        private static void CaricaDatiUtente() {
            var Api = new ClassApiEcoControl();
            var CountPet = int.Parse(Api.EseguiCommand("Select Sum(CountPet) From RegistroUtenti Where (Telefono='" + App.DataRowUtente["CodiceFiscale"].ToString() + "' Or Telefono='" + App.DataRowUtente["CodiceMonetaVirtuale"].ToString() + "') And Data>'2020-01-01'").ToString());
            var KgOlio = decimal.Parse(Api.EseguiCommand("Select Sum(KgOlio) From RegistroUtenti Where (Telefono='" + App.DataRowUtente["CodiceFiscale"].ToString() + "' Or Telefono='" + App.DataRowUtente["CodiceMonetaVirtuale"].ToString() + "') And Data>'2020-01-01'").ToString());
            UtentePetRaccolto = CountPet;
            UtenteOilRaccolto = (int)KgOlio;
            UtenteBariliPetrolioRisparmiato = CountPet / 100; //Formula inventata
            UtenteKgCO2Risparmiato = CountPet / 100; //formula inventata

            
        }
    }

    public static class PushApi {
        public static void Inizializza() {
            //Handle notification when app is open
            CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) => {
                var Messaggio = p.Data["body"].ToString();
                Boolean Flash = Boolean.Parse(p.Data["flash"].ToString());
                if (Flash == true) {
                    Device.BeginInvokeOnMainThread(async () => {
                        await Application.Current.MainPage.DisplayAlert("", Messaggio + " Flash:" + Flash.ToString(), "OK");
                    });
                } else {
                    Device.BeginInvokeOnMainThread(() => {
                        App.Current.MainPage = new PageNotifiche();
                    });
                }
            };
            CrossFirebasePushNotification.Current.OnTokenRefresh += (s, p) => {
                CrossFirebasePushNotification.Current.Subscribe("SUPERADMIN");
            };
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
