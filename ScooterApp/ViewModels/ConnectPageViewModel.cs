using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ScooterApp.Services;
using ScooterApp.Services.NinebotESX;
using ScooterApp.Services.ScooterService;
using Xamarin.Forms;
using Plugin.BluetoothLE;
using System.Threading;

namespace ScooterApp.ViewModels
{
    public class ConnectPageViewModel : ViewModelBase
    {
        private bool isBusy = false;
        private string busyMessage = "";
        private bool isScanning = false;
        private string bleStatus = Styles.Icons.BLUETOOTH_OFF;
        private string statusMessage = "";
        private string buttonText = "Scan";
        private IAdapter BLE;
        private CancellationTokenSource scanTokenSource;
        private bool canDisconnect = false;
        private bool canScan = true;

        public ObservableCollection<DeviceViewModel> Devices { get; set; } = new ObservableCollection<DeviceViewModel>();
        public bool IsScanning { get { return isScanning; } set { SetValue(ref isScanning, value); } }
        public string BLEStatus { get { return bleStatus; } set { SetValue(ref bleStatus, value); } }
        public string BusyMessage { get => busyMessage; set => SetValue(ref busyMessage, value); }
        public bool IsBusy { get => isBusy; set => SetValue(ref isBusy, value); }
        public string StatusMessage { get { return statusMessage; } set { SetValue(ref statusMessage, value); } }
        public string ButtonText { get { return buttonText; } set { SetValue(ref buttonText, value); } }
        public bool CanScan { get => canScan; set { SetValue(ref canScan, value); } }
        public bool CanDisconnect { get => canDisconnect; set { SetValue(ref canDisconnect, value); } }


        public Command ScanBLECommand { get; set; }
        public Command RequestGPSAccessCommand { get; set; }
        public Command SelectScooterCommand { get; set; }
        public Command DisconnectBLECommand { get; set; }

        public ConnectPageViewModel()
        {
            ScanBLECommand = new Command(async () => { await onScanBLECommand(); });
            RequestGPSAccessCommand = new Command(async () => { await onRquestGPSAccessCommand(); });
            SelectScooterCommand = new Command<DeviceViewModel>(async (scooter) => { await onSelectScooterCommand(scooter); });
            DisconnectBLECommand = new Command(() => DisconnectFromScooter());
            CrossBleAdapter.Current.WhenStatusChanged().Subscribe((status) => { onBLEStatusChanged(status); });
            BLE = CrossBleAdapter.Current;   
        }

        private void onBLEStatusChanged(AdapterStatus status)
        {
            switch (status)
            {
                case AdapterStatus.PoweredOff:
                    BLEStatus = Styles.Icons.BLUETOOTH_OFF;
                    StatusMessage = "Bluetooth status: Off";
                    break;
                case AdapterStatus.PoweredOn:
                    BLEStatus = Styles.Icons.BLUETOOTH_ON;
                    StatusMessage = "Bluetooth status: On";
                    break;
                case AdapterStatus.Resetting:
                    BLEStatus = Styles.Icons.BLUETOOTH_OFF;
                    StatusMessage = "Bluetooth status: Resetting...";
                    break;
                case AdapterStatus.Unauthorized:
                    BLEStatus = Styles.Icons.BLUETOOTH_OFF;
                    StatusMessage = "Bluetooth status: Unauthorized.";
                    break;
                case AdapterStatus.Unknown:
                    BLEStatus = Styles.Icons.BLUETOOTH_OFF;
                    StatusMessage = "Bluetooth status: Uknown.";
                    break;
                case AdapterStatus.Unsupported:
                    BLEStatus = Styles.Icons.BLUETOOTH_OFF;
                    StatusMessage = "Bluetooth status: Not supported.";
                    break;
                default:
                    break;
            }
        }

        private async Task onScanBLECommand()
        {
            try
            {
                if (await onRquestGPSAccessCommand())
                {
                    if (BLE != null && BLE.Status == AdapterStatus.PoweredOn)
                    {
                        if (!BLE.IsScanning)
                        {
                            ButtonText = "Stop";
                            IsScanning = true;
                            Devices.Clear();
                            scanTokenSource = new CancellationTokenSource();
                            BLE.Scan().Subscribe(onDeviceFound, scanTokenSource.Token);
                        }
                        else
                        {
                            StopBLEScan();
                        }
                    }
                    else
                    {
                        NotificationService.Instance.DisplayAlert("Error", "Bluetooth is not availible. Status: " + BLE.Status, "Ok");
                    }
                }
                else
                {
                    NotificationService.Instance.DisplayAlert("Error", "We don't have access to gps.", "Ok");
                }
            }
            catch (Exception ex)
            {
                IsScanning = false;
                NotificationService.Instance.DisplayAlert("Exception: ", ex.Message, "Ok");
            }
        }

        private void DisconnectFromScooter()
        {
            if (App.ScooterService != null)
            {
                App.ScooterService.Disconnect();
            }
        }

        private void onDeviceFound(IScanResult result)
        {
            if (result.AdvertisementData.ServiceUuids != null && result.AdvertisementData.ServiceUuids.Contains(Guid.Parse(AppSettings.UUID.NinebotSerialService)))
            {
                if (result.AdvertisementData.ManufacturerData != null)
                {
                    if (result.AdvertisementData.ManufacturerData.SequenceEqual(AppSettings.Constants.NinebotEncrypted))
                    {
                        AddToList(result, true);
                    }
                    else if(result.AdvertisementData.ManufacturerData.SequenceEqual(AppSettings.Constants.NinebotNotencrypted))
                    {
                        AddToList(result, false);
                    }
                    else
                    {
                        Console.WriteLine("Uknown Ninebot scooter: " + result.Device.Name);
                        return;
                    }
                }

            }
        }

        private void AddToList(IScanResult result, bool isEncrypted)
        {
            var device = Devices.FirstOrDefault((d) => { return d.UUID == result.Device.Uuid.ToString(); });
            if (device == null)
            {
                Devices.Add(new DeviceViewModel(result.Device, isEncrypted) { Name = result.Device.Name, UUID = result.Device.Uuid.ToString(), Rssi = result.Rssi });
            }
            else
            {
                device.Rssi = result.Rssi;
            }
        }

        private void StopBLEScan()
        {
            ButtonText = "Scan";
            IsScanning = false;
            if (scanTokenSource != null && scanTokenSource.Token.CanBeCanceled)
            {
                scanTokenSource.Cancel();
            }
            BLE.StopScan();
        }

        private async Task<bool> onRquestGPSAccessCommand()
        {
            return await PermissionManager.IsLocationPermissionsGrantedAsync();
        }

        private async Task onSelectScooterCommand(DeviceViewModel device)
        {
            try
            {
                StopBLEScan();
                Devices.Clear();
                BusyMessage = "Connecting";
                IsBusy = true;
                App.CurrentDevice = device;
                App.ScooterService = new NinebotBleService(device.Current, device.IsEncrypted, new NinebotCrypto(device.Name));
                App.ScooterService.OnAction += ScooterService_OnAction;
                App.ScooterService.OnError += ScooterService_OnError;
                App.ScooterService.OnStatus += ScooterService_OnStatus;
                await App.ScooterService.Connect();
            }
            catch (Exception)
            {
                IsBusy = false;
                NotificationService.Instance.DisplayAlert("Error", "Failed connect to " + App.ScooterService.ScooterName, "Ok");
            }
        }

        private async Task Authentificate()
        {
            if (App.ScooterService != null && App.ScooterService.IsAuthenticated == false)
            {
                try
                {
                    await App.ScooterService.Authenticate();
                }
                catch (Exception ex)
                {
                    IsBusy = false;
                    NotificationService.Instance.DisplayAlert("Error", ex.Message);
                }
            }
        }

        private async void ScooterService_OnStatus(object sender, StatusEventArg status)
        {
            switch (status.Type)
            {
                case StatusType.SCOOTER_CONNECTED:
                    IsBusy = false;
                    App.CurrentDevice.IsConnected = true;
                    IsBusy = true;
                    BusyMessage = "Connecting";
                    StatusMessage = "Connected: " + App.CurrentDevice.Name;
                    ButtonText = "Disconnect";
                    await Authentificate();
                    break;
                case StatusType.SCOOTER_DISCONNECTED:
                    ButtonText = "Scan";
                    StatusMessage = "Disconnected from: " + App.CurrentDevice.Name;
                    App.CurrentDevice.IsConnected = false;
                    IsBusy = false;
                    CanScan = true;
                    CanDisconnect = false;
                    break;
                case StatusType.AUTHENTICATED:
                    CanDisconnect = true;
                    CanScan = false;
                    App.CurrentDevice.IsAuthenticated = true;
                    Devices.Clear();
                    App.ReloadBasicInfo = true;
                    App.ReloadBMS1Info = true;
                    App.ReloadBMS2Info = true;
                    NavigationService.Instance.Goto(AppSettings.Routes.BasicInfoPageRoute);
                    break;
                case StatusType.COMMUNICATION:
                    //Utils.PrintHex(status.Message, status.Data);
                    break;
                default:
                    break;
            }
        }

        private void ScooterService_OnError(object sender, ErrorEventArg error)
        {
            NotificationService.Instance.DisplayAlert(error.Type.ToString(), error.Message);
        }

        private void ScooterService_OnAction(object sender, ActionEventArg action)
        {
            BusyMessage = action.Message;
            switch (action.Type)
            {
                case ActionType.ACTION_REQUIRED:
                    IsBusy = true;
                    break;
                case ActionType.ACTTION_COMPLETED:
                    IsBusy = false;
                    break;
                case ActionType.ACTION_FAILED:
                    IsBusy = false;
                    break;
            }
        }
    }
}
