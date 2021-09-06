using System;
using System.Threading.Tasks;
using ScooterApp.Services;
using Xamarin.Forms;

namespace ScooterApp.ViewModels
{
    public class InternalBatteryPageViewModel : ViewModelBase
    {
        private DeviceViewModel scooter;
        private bool isBusy = true;
        private string busyMessage = "Please connect to scooter";

        public string BusyMessage { get => busyMessage; set => SetValue(ref busyMessage, value); }
        public bool IsBusy { get => isBusy; set => SetValue(ref isBusy, value); }
        public Command LoadDataCommand { get; set; }
        public DeviceViewModel Scooter { get => scooter; set { SetValue(ref scooter, value); } }

        public InternalBatteryPageViewModel()
        {
            LoadDataCommand = new Command(async () => { await onLoadDataCommand(); });
        }

        public bool CheckScooterStatus()
        {
            if (App.ScooterService != null && App.ScooterService.IsConnected && App.ScooterService.IsAuthenticated)
            {
                IsBusy = false;
                Scooter = App.CurrentDevice;
                return true;
            }
            else
            {
                BusyMessage = "Please connect to scooter";
                IsBusy = true;
                return false;
            }
        }

        private async Task onLoadDataCommand()
        {
            if (CheckScooterStatus())
            {
                try
                {
                    IsBusy = true;
                    BusyMessage = "Loading data";

                }
                catch (Exception ex)
                {
                    NotificationService.Instance.DisplayAlert("Error", ex.Message, "Ok");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }


    }
}
