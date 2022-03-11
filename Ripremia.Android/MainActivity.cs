using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Plugin.FirebasePushNotification;
using Android.Content;
using Java.Lang;
using Android.Content.Res;
using Plugin.Permissions;
using System.Threading.Tasks;

namespace EcoServiceApp.Droid
{
    [Activity(Label = "Ripremia", Icon = "@drawable/RipremiaLogoApp", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Xamarin.Forms.DependencyService.Register<MultiPlatform>();
            //TabLayoutResource = Resource.Layout.Tabbar;
            //ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
            ZXing.Net.Mobile.Forms.Android.Platform.Init(); //zxing init

            //Firebase Messaging
            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O) {
                //Change for your default notification channel id here
                FirebasePushNotificationManager.DefaultNotificationChannelId = "FirebasePushNotificationChannel";
                //Change for your default notification channel name here
                FirebasePushNotificationManager.DefaultNotificationChannelName = "General";
            }
            FirebasePushNotificationManager.Initialize(this, false);

            //get max heap available to app
            var activityManager = (ActivityManager)GetSystemService(Context.ActivityService);
            int maxHeap = activityManager.MemoryClass * 1024; //KB

            //get current heap used by app
            Runtime runtime = Runtime.GetRuntime();
            int usedHeap = (int)((runtime.TotalMemory() - runtime.FreeMemory()) / 1024.0f); //KB

            //get amount of free heap remaining for app
            int availableHeap = maxHeap - usedHeap;


        }

        //aggiunto per i cellulari che hanno i font ingranditi
        protected override void AttachBaseContext(Context @base) {
            var configuration = new Configuration(@base.Resources.Configuration);

            configuration.FontScale = 1.1f;
            var config = Application.Context.CreateConfigurationContext(configuration);

            base.AttachBaseContext(config);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {

            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
    public class MultiPlatform : IMultiPlatform {

        public Task<int> RequestTrackingAuth() {
            var t=Task.FromResult(1);
            
            return t;
        }
    }
}