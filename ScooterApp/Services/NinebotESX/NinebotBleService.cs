using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Plugin.BluetoothLE;
using ScooterApp.Services.ScooterService;

namespace ScooterApp.Services.NinebotESX
{
    public class NinebotBleService : NinebotBaseService
    {
        //Ninebot App key
        private readonly byte[] Key = { 0x4A, 0xEE, 0xBD, 0x73, 0xE2, 0x16, 0x1C, 0x11, 0x2D, 0x06, 0x5A, 0x49, 0xCC, 0x6E, 0x8B, 0xB7 };
        private CancellationTokenSource deviceTokenSource;


        //BLE
        private IDevice currentDevice;
        private IGattCharacteristic writeCharacteristic;
        private IGattCharacteristic readCharacteristic;


        #region PRIVATE Methods

        protected override Task<bool> AuthenticateNotEncrypted()
        {
            OnStatusEvent(this, StatusType.AUTHENTICATED, "Authentificated to " + currentDevice.Name);
            OnActionEvent(this, ActionType.ACTTION_COMPLETED, "Authentificated to " + currentDevice.Name);
            IsAuthenticated = true;
            return Task.FromResult(true);
        }

        protected override async Task<bool> AuthenticateEncrypted()
        {
            authentificationStarted = false;
            buttonPressed = false;
            connectionConfirmed = false;
            byte[] data;
            for (int i = 0; i < maxAuthRetry; i++)
            {
                data = GetPacket(ESXLocation.PHONE, ESXLocation.BLE, ESXCommand.START_AUTHENTIFICATION, 0x00);
                await Write(data);
                for (int j = 0; j < 100; j++)
                {
                    if (authentificationStarted)
                    {
                        break;
                    }
                    await Task.Delay(10);
                }
                if (authentificationStarted)
                {
                    break;
                }
            }
            if (authentificationStarted)
            {
                for (int i = 0; i < maxAuthRetry; i++)
                {
                    data = GetPacket(ESXLocation.PHONE, ESXLocation.BLE, ESXCommand.ASK_PRESS_BUTTON, 0x00, Key);
                    await Write(data);
                    OnActionEvent(this, ActionType.ACTION_REQUIRED, "Press button");
                    for (int j = 0; j < 100; j++)
                    {
                        if (buttonPressed)
                        {
                            break;
                        }
                        await Task.Delay(10);
                    }
                    if (buttonPressed)
                    {
                        break;
                    }
                }
                if (buttonPressed)
                {
                    for (int i = 0; i < maxAuthRetry; i++)
                    {
                        data = GetPacket(ESXLocation.PHONE, ESXLocation.BLE, ESXCommand.CONFIRM_AUTHENTIFICATION, 0x00, serialNumber);
                        await Write(data);
                        OnActionEvent(this, ActionType.ACTION_STATUS, "Waiting confirmation");
                        for (int j = 0; j < 100; j++)
                        {
                            if (connectionConfirmed)
                            {
                                break;
                            }
                            await Task.Delay(10);
                        }
                        await Task.Delay(1000);
                        if (connectionConfirmed)
                        {
                            break;
                        }
                    }
                    if (connectionConfirmed)
                    {
                        IsAuthenticated = true;
                        OnActionEvent(this, ActionType.ACTTION_COMPLETED, "Authenticated to " + currentDevice.Name);
                        OnStatusEvent(this, StatusType.AUTHENTICATED, "Authenticated to " + currentDevice.Name);
                        return true;
                    }
                    else
                    {
                        IsAuthenticated = false;
                        OnActionEvent(this, ActionType.ACTION_FAILED, "Failed to authentificate. Connection not confirmed.");
                        OnErrorEvent(this, ErrorType.AUTHENTICATION_FAILED, "Failed to authentificate. Connection not confirmed.");
                    }
                }
                else
                {
                    IsAuthenticated = false;
                    OnActionEvent(this, ActionType.ACTION_FAILED, "Failed to authentificate. Button not pressed.");
                    OnErrorEvent(this, ErrorType.AUTHENTICATION_FAILED, "Failed to authentificate. Button not pressed.");
                }
            }
            else
            {
                IsAuthenticated = false;
                OnActionEvent(this, ActionType.ACTION_FAILED, "Failed to authentificate. Don't have response from scooter.");
                OnErrorEvent(this, ErrorType.AUTHENTICATION_FAILED, "Failed to authentificate. Don't have response from scooter.");
            }
            await Disconnect();
            return false;
        }

        private void ProcessNotEncryptedData(byte[] data)
        {
            if (!isPacketStart)
            {
                if (data.Length >= ESXConstants.MinimumPacketLength && data[0] == ESXConstants.HeaderByte1 && data[1] == ESXConstants.HeaderByte2)
                {
                    currentPacket = new List<byte>();
                    isPacketStart = true;
                    currentPayloadLength = data[2];
                    currentPacketLength = currentPayloadLength + ESXConstants.MinimumPacketLength;
                    foreach (var value in data)
                    {
                        currentPacket.Add(value);
                        if (currentPacket.Count >= currentPacketLength)
                        {
                            isPacketStart = false;
                            ProcessPacket(currentPacket.ToArray());
                            return;
                        }
                    }
                }
                else
                {
                    OnErrorEvent(this, ErrorType.INVALID_PACKET, "Invalid start data received", data);
                }
            }
            else
            {
                if (currentPacket.Count + data.Length < currentPacketLength && data.Length < ESXConstants.BLEPacketLength)
                {
                    OnErrorEvent(this, ErrorType.INVALID_PACKET, "Invalid packet data received", data);
                }
                else
                {
                    foreach (var value in data)
                    {
                        currentPacket.Add(value);
                        if (currentPacket.Count >= currentPacketLength)
                        {
                            isPacketStart = false;
                            ProcessPacket(currentPacket.ToArray());
                            return;
                        }
                    }
                }
            }
        }

        private void ProcessEncryptedData(byte[] data)
        {
            if (!isPacketStart)
            {
                if (data.Length >= ESXConstants.MinimumEncryptedPacketLength && data[0] == ESXConstants.HeaderByte1 && data[1] == ESXConstants.HeaderByte2)
                {
                    currentPacket = new List<byte>();
                    isPacketStart = true;
                    currentPayloadLength = data[2];
                    currentPacketLength = currentPayloadLength + ESXConstants.MinimumEncryptedPacketLength;
                    foreach (var value in data)
                    {
                        currentPacket.Add(value);
                        if (currentPacket.Count >= currentPacketLength)
                        {
                            isPacketStart = false;
                            var result = CryptoService.Decrypt(currentPacket.ToArray());
                            ProcessPacket(result);
                        }
                    }
                }
                else
                {
                    OnErrorEvent(this, ErrorType.INVALID_PACKET, "Invalid start data received", data);
                }
            }
            else
            {
                if (currentPacket.Count + data.Length < currentPacketLength && data.Length < ESXConstants.BLEPacketLength)
                {
                    OnErrorEvent(this, ErrorType.INVALID_PACKET, "Invalid packet data received", data);
                }
                else
                {
                    foreach (var value in data)
                    {
                        currentPacket.Add(value);
                        if (currentPacket.Count >= currentPacketLength)
                        {
                            isPacketStart = false;
                            var result = CryptoService.Decrypt(currentPacket.ToArray());
                            ProcessPacket(result);
                            return;
                        }
                    }
                }
            }
        }

        private void BLE_DeviceDisconnected(IDevice device)
        {
            if (IsConnected)
            {
                Disconnect();
            }
        }

        private async void BLE_DeviceConnected(IDevice device)
        {
            try
            {
                IsConnected = true;
                readCharacteristic = await device.GetKnownCharacteristics(Guid.Parse(AppSettings.UUID.NinebotSerialService), new Guid[] { Guid.Parse(AppSettings.UUID.NinebotReadCharacteristic)});
                await readCharacteristic.EnableNotifications();
                readCharacteristic.WhenNotificationReceived().Subscribe(response => ReadCharacteristic_ValueUpdated(response), deviceTokenSource.Token);
                writeCharacteristic = await device.GetKnownCharacteristics(Guid.Parse(AppSettings.UUID.NinebotSerialService), new Guid[] { Guid.Parse(AppSettings.UUID.NinebotWriteCharacteristic) });
                OnStatusEvent(this, StatusType.SCOOTER_CONNECTED, "Connected to " + currentDevice.Name);
            }
            catch (Exception ex)
            {
                await Disconnect();
                OnErrorEvent(this, ErrorType.COMMUNICATION_INTERFACE_ERROR, ex.Message);
            }
        }

        private void BLE_DeviceError(BleException error)
        {
            OnErrorEvent(this, ErrorType.COMMUNICATION_INTERFACE_ERROR, error.Message);
        }

        private void ReadCharacteristic_ValueUpdated(CharacteristicGattResult result)
        {
            if (result.Data == null || result.Data.Length == 0)
            {
                return;
            }
            if (IsEncrypted)
            {
                ProcessEncryptedData(result.Data);
            }
            else
            {
                ProcessNotEncryptedData(result.Data);
            }
        }

        protected override async Task<bool> Write(byte[] packet)
        {
            if (packet.Length <= ESXConstants.BLEPacketLength)
            {
                await writeCharacteristic.Write(packet);
                return true;
            }
            int position = 0;
            int length = ESXConstants.BLEPacketLength;
            while (position < packet.Length)
            {
                byte[] data = new byte[length];
                Buffer.BlockCopy(packet, position, data, 0, length);
                await writeCharacteristic.Write(data);
                position += ESXConstants.BLEPacketLength;
                length = position + ESXConstants.BLEPacketLength < packet.Length ? ESXConstants.BLEPacketLength : packet.Length - position;
            }
            return true;
        }

        #endregion

        public NinebotBleService(IDevice device, bool isEncrypted = false, ICryptoService cryptoService = null) : base(device.Name, isEncrypted, cryptoService)
        {
            currentDevice = device;
        }

        public override Task Connect()
        {
            deviceTokenSource = new CancellationTokenSource();
            currentDevice.WhenConnected().Subscribe((device) => BLE_DeviceConnected(device), deviceTokenSource.Token);
            currentDevice.WhenConnectionFailed().Subscribe((error) => BLE_DeviceError(error), deviceTokenSource.Token);
            currentDevice.WhenDisconnected().Subscribe((device) => BLE_DeviceDisconnected(device), deviceTokenSource.Token);
            currentDevice.Connect();
            return Task.FromResult(true);
        }

        public override Task Disconnect()
        {
            IsConnected = false;
            IsAuthenticated = false;
            if (currentDevice != null)
            {
                currentDevice.CancelConnection();
            }
            if (deviceTokenSource != null && deviceTokenSource.Token.CanBeCanceled)
            {
                deviceTokenSource.Cancel();
            }
            OnStatusEvent(this, StatusType.SCOOTER_DISCONNECTED, "Disconnected from " + currentDevice.Name);
            return Task.FromResult(true);
        }
        
    }
}
