using ScooterApp.Services.NinebotESX;
using ScooterApp.ViewModels;
using ScooterApp.Views;
using Xamarin.Forms;

namespace ScooterApp
{
    public partial class App : Application
    {
        public static NinebotBaseService ScooterService = null;
        public static DeviceViewModel CurrentDevice = null;
        public static bool ReloadBasicInfo = false;
        public static bool ReloadBMS1Info = false;
        public static bool ReloadBMS2Info = false;

        public App()
        {
            InitializeComponent();
            MainPage = new ShellPage();
        }

        protected override void OnStart()
        {

        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
