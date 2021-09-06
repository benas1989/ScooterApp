using ScooterApp.ViewModels;
using Xamarin.Forms;

namespace ScooterApp.Views
{
    public partial class ExternalBatteryPage : ContentPage
    {
        public ExternalBatteryPageViewModel VM { get { return BindingContext as ExternalBatteryPageViewModel; } set { BindingContext = value; } }

        public ExternalBatteryPage()
        {
            InitializeComponent();
            VM = new ExternalBatteryPageViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (VM.CheckScooterStatus() && App.ReloadBMS2Info)
            {
                App.ReloadBMS2Info = false;
                VM.LoadDataCommand.Execute(null);
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }
    }
}
