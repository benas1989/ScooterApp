using ScooterApp.ViewModels;
using Xamarin.Forms;

namespace ScooterApp.Views
{
    public partial class BasicInfoPage : ContentPage
    {
        public BasicInfoPageViewModel VM { get { return BindingContext as BasicInfoPageViewModel; } set { BindingContext = value; } }

        public BasicInfoPage()
        {
            InitializeComponent();
            VM = new BasicInfoPageViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (VM.CheckScooterStatus() && App.ReloadBasicInfo)
            {
                App.ReloadBasicInfo = false;
                VM.LoadDataCommand.Execute(null);
            }
        }

        protected override void OnDisappearing()
        {
            
            base.OnDisappearing();
        }
    }
}
