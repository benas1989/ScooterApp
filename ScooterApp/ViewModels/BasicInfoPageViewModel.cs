using System;
using System.Threading.Tasks;
using ScooterApp.Services;
using ScooterApp.Services.NinebotESX;
using Xamarin.Forms;

namespace ScooterApp.ViewModels
{
    public class BasicInfoPageViewModel : ViewModelBase
    {
        private DeviceViewModel scooter;
        private bool isBusy = true;
        private string busyMessage = "Please connect to scooter";

        public string BusyMessage { get => busyMessage; set => SetValue(ref busyMessage, value); }
        public bool IsBusy { get => isBusy; set => SetValue(ref isBusy, value); }
        public Command LoadDataCommand { get; set; }
        public DeviceViewModel Scooter { get => scooter; set { SetValue(ref scooter, value); } }


        public BasicInfoPageViewModel()
        {
            LoadDataCommand = new Command(async() => { await onLoadDataCommand(); });
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
                BusyMessage =  "Please connect to scooter";
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

                    Scooter.ESCSerialNumber = await App.ScooterService.ReadSerialNumber(ESXLocation.PHONE, ESXLocation.ESC);
                    Scooter.ESCVersion = await App.ScooterService.ReadESCFirmwareVersion(ESXLocation.PHONE);
                    Scooter.BLEFirmwareVersion = await App.ScooterService.ReadBLEFirmwareVersionFromESC(ESXLocation.PHONE);
                    Scooter.InternalBMSSerialNumber = await App.ScooterService.ReadSerialNumber(ESXLocation.PHONE, ESXLocation.BMS1);
                    Scooter.InternalBMSFirmwareVersionFromESC = await App.ScooterService.ReadBMS1FirmwareVersionFromESC(ESXLocation.PHONE);
                    Scooter.ExternalBMSFirmwareVersionFromESC = await App.ScooterService.ReadBMS2FirmwareVersionFromESC(ESXLocation.PHONE);

                    var status = await App.ScooterService.ReadQuickStatus(ESXLocation.PHONE);
                    Scooter.AlarmCode = status.AlarmCode;
                    Scooter.AverageSpeed = status.AverageSpeed;
                    Scooter.InternalBMSRemainingCapacityPercent = status.BMS1Percentage;
                    Scooter.ExternalBMSRemainingCapacityPercent = status.BMS2Percentage;
                    Scooter.BMSPercentage = status.BMSPercentage;
                    Scooter.CurrentMileage = status.CurrentMileage;
                    Scooter.CurrentRuntime = status.CurrentRuntime;
                    Scooter.CurrentSpeed = status.CurrentSpeed;
                    Scooter.ErrorCode = status.ErrorCode.Id;
                    Scooter.ErrorMessage = status.ErrorCode.Message;
                    Scooter.HasExternalBattery = status.ESCStatus.HasExternalBattery;
                    Scooter.HasSpeedLimit = status.ESCStatus.HasSpeedLimit;
                    Scooter.IsESCActivated = status.ESCStatus.IsActivated;
                    Scooter.IsBuzzerActive = status.ESCStatus.IsBuzzerActive;
                    Scooter.IsLocked = status.ESCStatus.IsLocked;
                    Scooter.ScooterTemperature = status.ESXTemperature;
                    Scooter.CurrentOperatingMode = status.OperatingMode;
                    Scooter.Power = status.Power;
                    Scooter.PredictedRemainingMileage = status.PredictedRemainingMileage;
                    Scooter.Range = status.Range;
                    Scooter.TotalMileage = status.TotalMileage;

                    if (Scooter.HasExternalBattery)
                    {
                        Scooter.ExternalBMSSerialNumber = await App.ScooterService.ReadSerialNumber(ESXLocation.PHONE, ESXLocation.BMS2);
                    }
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