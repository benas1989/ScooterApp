using ScooterApp.ViewModels;
using Xamarin.Forms;

namespace ScooterApp.Views
{
    public partial class InternalBatteryPage : ContentPage
    {
        public InternalBatteryPageViewModel VM { get { return BindingContext as InternalBatteryPageViewModel; } set { BindingContext = value; } }

        public InternalBatteryPage()
        {
            InitializeComponent();
            VM = new InternalBatteryPageViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (VM.CheckScooterStatus() && App.ReloadBMS1Info)
            {
                App.ReloadBMS1Info = false;
                VM.LoadDataCommand.Execute(null);
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }
    }
}
