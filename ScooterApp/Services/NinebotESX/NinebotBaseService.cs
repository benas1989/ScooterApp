using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScooterApp.Services.ScooterService;

namespace ScooterApp.Services.NinebotESX
{
    public abstract class NinebotBaseService : ScooterBaseService
    {
        //New Packet
        protected List<byte> currentPacket;
        protected bool isPacketStart = false;
        protected int currentPacketLength = 0;
        protected int currentPayloadLength = 0;
        protected bool hasResponse = false;
        protected ESXPacket responsePacket;
        protected byte[] responseArray;

        //Authentication
        protected byte[] serialNumber;
        protected byte[] bleAuthKey;
        protected int maxAuthRetry = 5;
        protected bool authentificationStarted = false;
        protected bool buttonPressed = false;
        protected bool connectionConfirmed = false;
        protected int maxReadRetry = 5;
        protected int maxWriteRetry = 5;

        #region Private

        protected abstract Task<bool> AuthenticateNotEncrypted();

        protected abstract Task<bool> AuthenticateEncrypted();

        protected void ProcessPacket(byte[] data)
        {
            ESXPacket packet = new ESXPacket();
            packet.Header1 = data[0];
            packet.Header2 = data[1];
            packet.PayloadLength = data[2];
            packet.Source = (ESXLocation)data[3];
            packet.Destination = (ESXLocation)data[4];
            packet.Command = (ESXCommand)data[5];
            packet.CommandData = data[6];
            packet.Payload = null;
            if (IsEncrypted)
            {
                if (data.Length > packet.PayloadLength + 7)
                {
                    packet.CheckSum = new byte[3];
                    packet.CheckSum[0] = data[data.Length - 1];
                    packet.CheckSum[1] = data[data.Length - 2];
                    packet.CheckSum[2] = data[data.Length - 3];
                }
            }
            else
            {
                packet.CheckSum = new byte[2];
                packet.CheckSum[0] = data[data.Length - 2];
                packet.CheckSum[1] = data[data.Length - 1];
            }
            packet.Payload = new byte[packet.PayloadLength];
            Buffer.BlockCopy(data, 7, packet.Payload, 0, packet.PayloadLength);
            if (packet.Command == ESXCommand.START_AUTHENTIFICATION && packet.PayloadLength == 30)
            {
                SaveAuthData(packet.Payload);
                authentificationStarted = true;
            }
            if (packet.Command == ESXCommand.ASK_PRESS_BUTTON)
            {
                if (packet.CommandData == 1)
                {
                    buttonPressed = true;
                }
                else
                {
                    buttonPressed = false;
                }
            }
            if (packet.Command == ESXCommand.CONFIRM_AUTHENTIFICATION)
            {
                if (packet.CommandData == 1)
                {
                    connectionConfirmed = true;
                }
                else
                {
                    connectionConfirmed = false;
                }
            }
            if (packet.Command == ESXCommand.RESPONSE_TO_READ_REGISTER || packet.Command == ESXCommand.RESPONSE_TO_WRITE_REGISTER)
            {
                responsePacket = packet;
                responseArray = data;
                hasResponse = true;
            }
            OnNewESXPacket?.Invoke(this, packet);
            OnNewPacketEvent(this, data);
            if (IsEncrypted)
            {
                OnStatusEvent(this, StatusType.COMMUNICATION, "Receive enc.", currentPacket.ToArray());
            }
            OnStatusEvent(this, StatusType.COMMUNICATION, "Receive", data);
        }

        private void SaveAuthData(byte[] payload)
        {
            bleAuthKey = new byte[16];
            serialNumber = new byte[14];
            Buffer.BlockCopy(payload, 0, bleAuthKey, 0, 16);
            Buffer.BlockCopy(payload, 16, serialNumber, 0, 14);
        }

        private byte[] GetPayload(byte[] data)
        {
            byte[] payload = new byte[data[2]];
            Buffer.BlockCopy(data, 7, payload, 0, payload.Length);
            return payload;
        }

        private float GetSpeed(byte byte1, byte byte0)
        {
            var total = Utils.BytesToInt(byte1, byte0);
            if (total > 10000)
            {
                total = 0xFFFF - total;
            }
            return total * 0.1f;
        }

        #endregion

        public event EventHandler<ESXPacket> OnNewESXPacket;

        public NinebotBaseService(string scooterName, bool isEncrypted = false, ICryptoService cryptoService = null)
        {
            ScooterName = scooterName;
            IsEncrypted = isEncrypted;
            CryptoService = cryptoService;
        }

        public override byte[] GetPacket(byte source, byte destination, byte command, byte commandData, byte[] payload = null)
        {
            return GetPacket((ESXLocation)source, (ESXLocation)destination, (ESXCommand)command, commandData, payload);
        }

        public byte[] GetPacket(ESXLocation source, ESXLocation destination, ESXCommand command, byte commandData, byte[] payload = null)
        {
            var payloadLength = payload == null ? 0 : payload.Length;
            var packetLength = 7 + payloadLength;
            byte[] packet = new byte[packetLength];
            packet[0] = ESXConstants.HeaderByte1;
            packet[1] = ESXConstants.HeaderByte2;
            packet[2] = (byte)payloadLength;
            packet[3] = (byte)source;
            packet[4] = (byte)destination;
            packet[5] = (byte)command;
            packet[6] = commandData;
            if (payloadLength > 0)
            {
                Buffer.BlockCopy(payload, 0, packet, 7, payloadLength);
            }
            if (IsEncrypted)
            {
                var encrypted = CryptoService.Encrypt(packet);
                OnStatusEvent(this, StatusType.COMMUNICATION, "Send", packet);
                OnStatusEvent(this, StatusType.COMMUNICATION, "Send enc.", encrypted);
                return encrypted;
            }
            else
            {
                var crc = CryptoService.CalculateCRC(packet);
                byte[] notencrypted = new byte[packetLength + 2];
                Buffer.BlockCopy(packet, 0, notencrypted, 0, packet.Length);
                notencrypted[notencrypted.Length - 2] = crc[0];
                notencrypted[notencrypted.Length - 1] = crc[1];
                OnStatusEvent(this, StatusType.COMMUNICATION, "Send", notencrypted);
                return notencrypted;
            }
        }

        public override async Task<byte[]> ReadRegister(byte source, byte destination, byte register, byte length)
        {
            for (int i = 0; i < maxReadRetry; i++)
            {
                responseArray = null;
                responsePacket = null;
                hasResponse = false;
                var packet = GetPacket(source, destination, (byte)ESXCommand.READ_REGISTER, register, new byte[] { length, 0x00 });
                await Write(packet);
                for (int j = 0; j < 50; j++)
                {
                    if (hasResponse)
                    {
                        break;
                    }
                    await Task.Delay(10);
                }
                if (hasResponse)
                {
                    break;
                }
                OnStatusEvent(this,StatusType.COMMUNICATION, "Repeat read register " + register );
            }
            if (hasResponse)
            {
                return responseArray;
            }
            else
            {
                var device = (ESXLocation)destination;
                switch (device)
                {
                    case ESXLocation.ESC:
                        throw new Exception("Register " + (ESCRegister)register + " read failed. Failed to get response from " + device);
                    case ESXLocation.BMS1:
                    case ESXLocation.BMS2:
                        throw new Exception("Register " + (BMSRegister)register + " read failed. Failed to get response from " + device);
                    default:
                        throw new Exception("Register " + (BMSRegister)register + " read failed. Failed to get response.");
                }
            }
        }

        public Task<byte[]> ReadESCRegister(ESXLocation source, ESCRegister register, byte length)
        {
            return ReadRegister((byte)source, (byte)ESXLocation.ESC, (byte)register, length);
        }

        public Task<byte[]> ReadBMS1Register(ESXLocation source, BMSRegister register, byte length)
        {
            return ReadRegister((byte)source, (byte)ESXLocation.BMS1, (byte)register, length);
        }

        public Task<byte[]> ReadBMS2Register(ESXLocation source, BMSRegister register, byte length)
        {
            return ReadRegister((byte)source, (byte)ESXLocation.BMS2, (byte)register, length);
        }

        public override async Task<bool> WriteRegister(byte source, byte destination, byte register, byte[] data)
        {
            var packet = GetPacket(source, destination, (byte)ESXCommand.WRITE_REGISTER, register, data);
            return await Write(packet);
        }

        public Task<bool> WriteESCRegister(ESXLocation source, ESCRegister register, byte[] data)
        {
            return WriteRegister((byte)source, (byte)ESXLocation.ESC, (byte)register, data);
        }

        public Task<bool> WriteBMS1Register(ESXLocation source, BMSRegister register, byte[] data)
        {
            return WriteRegister((byte)source, (byte)ESXLocation.BMS1, (byte)register, data);
        }

        public Task<bool> WriteBMS2Register(ESXLocation source, BMSRegister register, byte[] data)
        {
            return WriteRegister((byte)source, (byte)ESXLocation.BMS2, (byte)register, data);
        }

        public override async Task<byte[]> WriteRegisterWR(byte source, byte destination, byte register, byte[] data)
        {
            for (int i = 0; i < maxWriteRetry; i++)
            {
                responseArray = null;
                responsePacket = null;
                hasResponse = false;
                var packet = GetPacket(source, destination, (byte)ESXCommand.WRITE_REGISTER_WR, register, data);
                await Write(packet);
                for (int j = 0; j < 50; i++)
                {
                    if (hasResponse)
                    {
                        break;
                    }
                    await Task.Delay(10);
                }
                if (hasResponse)
                {
                    break;
                }
                OnStatusEvent(this, StatusType.COMMUNICATION, "Repeat write to register " + register );
            }
            if (hasResponse)
            {
                ESXWriteRegisterStatus status = (ESXWriteRegisterStatus)responseArray[6];
                switch (status)
                {
                    case ESXWriteRegisterStatus.SUCCESSFUL:
                        return responseArray;
                    default:
                        ESXLocation location = (ESXLocation)destination;
                        switch (location)
                        {
                            case ESXLocation.BMS1:
                            case ESXLocation.BMS2:
                                throw new Exception("Register " + (BMSRegister)register + " write failed Error: " + status );
                            case ESXLocation.ESC:
                                throw new Exception("Register " + (ESCRegister)register + " write failed Error: " + status);
                            default:
                                throw new Exception("Register " + register + " write failed Error: " + status);
                        }
                }
            }
            else
            {
                ESXLocation location = (ESXLocation)destination;
                switch (location)
                {
                    case ESXLocation.BMS1:
                    case ESXLocation.BMS2:
                        throw new Exception("Register " + (BMSRegister)register + " write failed. Failed to get response from " + (ESXLocation)destination + ".");
                    case ESXLocation.ESC:
                        throw new Exception("Register " + (ESCRegister)register + " write failed. Failed to get response from " + (ESXLocation)destination + ".");
                    default:
                        throw new Exception((ESXLocation)destination + " register " + register + " write failed. Failed to get response.");
                }

            }
        }

        public Task<byte[]> WriteESCRegisterWR(ESXLocation source, ESCRegister register, byte[] data)
        {
            return WriteRegisterWR((byte)source, (byte)ESXLocation.ESC, (byte)register, data);
        }

        public Task<byte[]> WriteBMS1RegisterWR(ESXLocation source, BMSRegister register, byte[] data)
        {
            return WriteRegisterWR((byte)source, (byte)ESXLocation.BMS1, (byte)register, data);
        }

        public Task<byte[]> WriteBMS2RegisterWR(ESXLocation source, BMSRegister register, byte[] data)
        {
            return WriteRegisterWR((byte)source, (byte)ESXLocation.BMS2, (byte)register, data);
        }

        public override Task<bool> Authenticate()
        {
            if (IsEncrypted)
            {
                return AuthenticateEncrypted();
            }
            else
            {
               return AuthenticateNotEncrypted();
            }
        }        

        #region READ REGISTERS

        //PHASE_A_CURRENT 0x0D
        //PHASE_B_CURRENT 0x0E
        //PHASE_C_CURRENT 0x0F

        public async Task<string> ReadSerialNumber(ESXLocation source, ESXLocation destination)
        {
            var response = new byte[0];
            switch (destination)
            {
                case ESXLocation.BMS1:
                    response = await ReadBMS1Register(source, BMSRegister.SERIAL_NUMBER, 14);
                    break;
                case ESXLocation.BMS2:
                    response = await ReadBMS2Register(source, BMSRegister.SERIAL_NUMBER, 14);
                    break;
                case ESXLocation.ESC:
                    response = await ReadESCRegister(source, ESCRegister.SERIAL_NUMBER, 14);
                    break;
                default:
                    return "";
            }
            return Utils.ArrayToASCI(GetPayload(response));
        }

        public async Task<string> ReadFirmwareVersion(ESXLocation source, ESXLocation destination)
        {
            var response = new byte[0];
            switch (destination)
            {
                case ESXLocation.BMS1:
                    response = await ReadBMS1Register(source, BMSRegister.FIRMWARE_VERSION, 2);
                    break;
                case ESXLocation.BMS2:
                    response = await ReadBMS2Register(source, BMSRegister.FIRMWARE_VERSION, 2);
                    break;
                case ESXLocation.ESC:
                    response = await ReadESCRegister(source, ESCRegister.ESC_FIRMWARE_VERSION, 2);
                    break;
                default:
                    return "";
            }
            string version = Utils.ArrayToNinebotVersion(GetPayload(response));
            return version;
        }

        public async Task<string> ReadBLEPassword(ESXLocation source)
        {
            var response = await ReadESCRegister(source, ESCRegister.BLE_PASSWORD, 6);
            return Utils.ArrayToASCI(GetPayload(response));
        }

        public async Task<string> ReadESCFirmwareVersion(ESXLocation source)
        {
            var response = await ReadESCRegister(source, ESCRegister.ESC_FIRMWARE_VERSION, 2);
            string version = Utils.ArrayToNinebotVersion(GetPayload(response));
            return version;
        }

        public async Task<ESXError> ReadErrorCode(ESXLocation source)
        {
            var response = await ReadESCRegister(source, ESCRegister.ERROR_CODE, 2);
            var error = GetPayload(response)[0];
            return new ESXError(error);
        }

        public async Task<ESXAlarmCode> ReadAlarmCode(ESXLocation source)
        {
            var response = await ReadESCRegister(source, ESCRegister.ALARM_CODE, 2);
            return (ESXAlarmCode)GetPayload(response)[0];
        }

        public async Task<ESCStatus> ReadESCStatus(ESXLocation source)
        {
            var response = await ReadESCRegister(source, ESCRegister.ESC_STATUS, 2);
            byte[] payload = GetPayload(response);
            return new ESCStatus(payload[1], payload[0]);
        }

        public async Task<byte> ReadESCCurrentOperationSystem(ESXLocation source)
        {
            var response = await ReadESCRegister(source, ESCRegister.CURRENT_OPERATION_SYSTEM, 2);
            return GetPayload(response)[0];
        }

        //CURRENT_OPERATION_MODE 0x1F. Not working. Use 0x75.
        //VOLUME_OF_BMS1 0x20
        //VOLUME_OF_BMS2 0x21

        public async Task<int> ReadBMSPercentageFromESC(ESXLocation source)
        {
            var response = await ReadESCRegister(source, ESCRegister.BMS_PERCENTAGE, 2);
            return GetPayload(response)[0];
        }

        public async Task<int> ReadActualRemainingMileage(ESXLocation source)
        {
            var response = await ReadESCRegister(source, ESCRegister.ACTUAL_REMAINING_MILEAGE, 2);
            byte[] payload = GetPayload(response);
            return Utils.BytesToInt(payload[1],payload[0]) * 10;
        }

        public async Task<int> ReadPredictedRemainingMileage(ESXLocation source)
        {
            var response = await ReadESCRegister(source, ESCRegister.PREDICTED_REMAINING_MILEAGE, 2);
            byte[] payload = GetPayload(response);
            return Utils.BytesToInt(payload[1], payload[0]) * 10;
        }

        public async Task<float> ReadCurrentSpeed(ESXLocation source)
        {
            var response = await ReadESCRegister(source, ESCRegister.CURRENT_SPEED, 2);
            byte[] payload = GetPayload(response);
            return GetSpeed(payload[1], payload[0]);
        }

        public async Task<int> ReadTotalMileage(ESXLocation source)
        {
            var response = await ReadESCRegister(source, ESCRegister.TOTAL_MILEAGE_L, 4);
            byte[] payload = GetPayload(response);
            return Utils.BytesToInt(payload[3], payload[2], payload[1],payload[0]);
        }

        public async Task<int> ReadCurrentMileage(ESXLocation source)
        {
            var response = await ReadESCRegister(source, ESCRegister.CURRENT_MILEAGE, 2);
            byte[] payload = GetPayload(response);
            return Utils.BytesToInt(payload[1], payload[0]) * 10;
        }

        public async Task<int> ReadTotalRuntime(ESXLocation source)
        {
            var response = await ReadESCRegister(source, ESCRegister.TOTAL_RUNTIME_L, 4);
            byte[] payload = GetPayload(response);
            return Utils.BytesToInt(payload[3],payload[2], payload[1], payload[0]);
        }

        public async Task<int> ReadTotalRidingTime(ESXLocation source)
        {
            var response = await ReadESCRegister(source, ESCRegister.TOTAL_RIDING_TIME_L, 4);
            byte[] payload = GetPayload(response);
            return Utils.BytesToInt(payload[3], payload[2], payload[1], payload[0]);
        }

        public async Task<int> ReadCurrentRuntime(ESXLocation source)
        {
            var response = await ReadESCRegister(source, ESCRegister.CURRENT_RUNTIME, 2);
            byte[] payload = GetPayload(response);
            return Utils.BytesToInt(payload[1], payload[0]);
        }

        public async Task<int> ReadCurrentRidingTime(ESXLocation source)
        {
            var response = await ReadESCRegister(source, ESCRegister.CURRENT_RIDING_TIME, 2);
            byte[] payload = GetPayload(response);
            return Utils.BytesToInt(payload[1], payload[0]);
        }

        public async Task<float> ReadESXTemperature(ESXLocation source)
        {
            var response = await ReadESCRegister(source, ESCRegister.SCOOTER_TEMPERATURE, 2);
            byte[] payload = GetPayload(response);
            return Utils.BytesToInt(payload[1], payload[0]) * 0.1f;
        }

        //BMS1_TEMPERATURE 0x3F
        //BMS2_TEMPERATURE 0x40
        //MOS_PIPE_TEMPERATURE 0x42

        public async Task<float> ReadESCVoltage(ESXLocation source)
        {
            var response = await ReadESCRegister(source, ESCRegister.ESC_VOLTAGE, 2);
            byte[] payload = GetPayload(response);
            return Utils.BytesToInt(payload[1], payload[0]) * 0.01f;
        }

        public async Task<float> ReadBMSVoltageFromESC(ESXLocation source)
        {
            var response = await ReadESCRegister(source, ESCRegister.BMS_VOLTAGE, 2);
            byte[] payload = GetPayload(response);
            return Utils.BytesToInt(payload[1], payload[0]) * 0.01f;
        }

        public async Task<float> ReadBMSCurrentFromESC(ESXLocation source)
        {
            var response = await ReadESCRegister(source, ESCRegister.BMS_CURRENT, 2);
            byte[] payload = GetPayload(response);
            return Utils.BytesToInt(payload[1], payload[0]) * 0.001f;
        }

        private int lastCurrentResult = 0;
        public async Task<float> ReadMotorPhaseCurrent(ESXLocation source)
        {
            var response = await ReadESCRegister(source, ESCRegister.MOTOR_PHASE_CURRENT, 2);
            byte[] payload = GetPayload(response);
            var result = Utils.BytesToInt(payload[1], payload[0]);
            if (result > 100)
            {
                result = lastCurrentResult;
            }
            else
            {
                lastCurrentResult = result;
            }
            return result * 0.01f;
        }

        public async Task<float> ReadAverageSpeed(ESXLocation source)
        {
            var response = await ReadESCRegister(source, ESCRegister.AVERAGE_SPEED, 2);
            byte[] payload = GetPayload(response);
            return GetSpeed(payload[1], payload[0]);
        }



        public async Task<string> ReadBMS2FirmwareVersionFromESC(ESXLocation source)
        {
            var response = await ReadESCRegister(source, ESCRegister.BMS2_FIRMWARE_VERSION, 2);
            string version = Utils.ArrayToNinebotVersion(GetPayload(response));
            return version;
        }

        public async Task<string> ReadBMS1FirmwareVersionFromESC(ESXLocation source)
        {
            var response = await ReadESCRegister(source, ESCRegister.BMS1_FIRMWARE_VERSION, 2);
            string version = Utils.ArrayToNinebotVersion(GetPayload(response));
            return version;
        }

        public async Task<string> ReadBLEFirmwareVersionFromESC(ESXLocation source)
        {
            var response = await ReadESCRegister(source, ESCRegister.BLE_FIRMWARE_VERSION, 2);
            string version = Utils.ArrayToNinebotVersion(GetPayload(response));
            return version;
        }

        public async Task<float> ReadSpeedLimitSportMode(ESXLocation source)
        {
            var response = await ReadESCRegister(source, ESCRegister.SPEED_LIMIT_SPORT_MODE, 2);
            var payload = GetPayload(response);
            return payload[0];
        }

        public async Task<float> ReadSpeedLimitNormalMode(ESXLocation source)
        {
            var response = await ReadESCRegister(source, ESCRegister.SPEED_LIMIT_NORMAL_MODE, 2);
            var payload = GetPayload(response);
            return (payload[1] * 255 + payload[0]) * 0.1f;
        }

        public async Task<float> ReadSpeedLimitEcoMode(ESXLocation source)
        {
            var response = await ReadESCRegister(source, ESCRegister.SPEED_LIMIT_ECO_MODE, 2);
            var payload = GetPayload(response);
            return (payload[1] * 255 + payload[0]) * 0.1f;
        }

        public async Task<ESXOperatingMode> ReadCurrentOperatingMode(ESXLocation source)
        {
            var response = await ReadESCRegister(source, ESCRegister.OPERATING_MODE, 2);
            return (ESXOperatingMode)GetPayload(response)[0];
        }

        public async Task<ESXEngineStatus> ReadEngineStatus(ESXLocation source)
        {
            var response = await ReadESCRegister(source, ESCRegister.ENGINE_STATUS, 2);
            return (ESXEngineStatus)GetPayload(response)[0];
        }

        public async Task<ESXKERSLevel> ReadKERSLevel(ESXLocation source)
        {
            var response = await ReadESCRegister(source, ESCRegister.KERS_LEVEL, 2);
            return (ESXKERSLevel)GetPayload(response)[0];
        }

        public async Task<ESXCruiseControlStatus> ReadCruiseControlStatus(ESXLocation source)
        {
            var response = await ReadESCRegister(source, ESCRegister.CRUISE_CONTROL, 2);
            return (ESXCruiseControlStatus)GetPayload(response)[0];
        }

        public async Task<ESXFunctions> ReadFunctions(ESXLocation source)
        {
            var response = await ReadESCRegister(source, ESCRegister.FUNCTION_SETUP, 2);
            return new ESXFunctions(GetPayload(response)[0]);
        }

        public async Task<ESXQuickStatus> ReadQuickStatus(ESXLocation source)
        {
            var response = await ReadESCRegister(source, ESCRegister.ERROR_CODE_QUICK, 32);
            var payload = GetPayload(response);
            var status = new ESXQuickStatus();
            status.ErrorCode = new ESXError(payload[0]);
            status.AlarmCode = payload[2];
            status.ESCStatus = new ESCStatus(payload[5], payload[4]);
            status.BMS1Percentage = payload[6];
            status.BMS2Percentage = payload[7];
            status.BMSPercentage = payload[8];
            status.CurrentSpeed = GetSpeed(payload[11], payload[10]);
            status.AverageSpeed = GetSpeed(payload[13], payload[12]);
            status.TotalMileage = Utils.BytesToInt(payload[17], payload[16], payload[15], payload[14]);
            status.CurrentMileage = Utils.BytesToInt(payload[19], payload[18]) * 10;
            status.CurrentRuntime = Utils.BytesToInt(payload[21], payload[20]);
            status.ESXTemperature = Utils.BytesToInt(payload[23], payload[22]) * 0.1f;
            status.OperatingMode = (ESXOperatingMode)payload[24];
            status.Range = payload[25] * 0.1f;
            status.Power = Utils.BytesToInt(payload[27], payload[26]);
            status.AlarmCodeForDelayReset = payload[28];
            status.PredictedRemainingMileage = Utils.BytesToInt(payload[31], payload[30]) * 10;
            return status;
        }

        public async Task<ESXLedStripEffect> ReadLEDEffect(ESXLocation source)
        {
            var response = await ReadESCRegister(source, ESCRegister.DISPLAY_MODE_OF_CHASSIS_LAMP, 2);
            return (ESXLedStripEffect)GetPayload(response)[0];
        }

        //--------------------------------------------------------------------------------------------------------------
        //---------------------------------------BMS--------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------

        public async Task<int> ReadBMSDesignCapacity(ESXLocation source, BMS bms)
        {
            var response = new byte[2];
            switch (bms)
            {
                case BMS.INTERNAL:
                    response = await ReadBMS1Register(source,BMSRegister.DESIGN_CAPACITY, 2);
                    break;
                case BMS.EXTERNAL:
                    response = await ReadBMS2Register(source, BMSRegister.DESIGN_CAPACITY, 2);
                    break;
                default:
                    break;
            }
            var payload = GetPayload(response);
            return Utils.BytesToInt(payload[1], payload[0]);
        }

        public async Task<int> ReadBMSActualCapacity(ESXLocation source, BMS bms)
        {
            var response = new byte[2];
            switch (bms)
            {
                case BMS.INTERNAL:
                    response = await ReadBMS1Register(source, BMSRegister.ACTUAL_CAPACITY, 2);
                    break;
                case BMS.EXTERNAL:
                    response = await ReadBMS2Register(source, BMSRegister.ACTUAL_CAPACITY, 2);
                    break;
                default:
                    break;
            }
            var payload = GetPayload(response);
            return Utils.BytesToInt(payload[1], payload[0]);
        }

        public async Task<int> ReadBMSDesignVoltage(ESXLocation source, BMS bms)
        {
            var response = new byte[2];
            switch (bms)
            {
                case BMS.INTERNAL:
                    response = await ReadBMS1Register(source, BMSRegister.DESIGN_VOLTAGE, 2);
                    break;
                case BMS.EXTERNAL:
                    response = await ReadBMS2Register(source, BMSRegister.DESIGN_VOLTAGE, 2);
                    break;
                default:
                    break;
            }
            var payload = GetPayload(response);
            return Utils.BytesToInt(payload[1], payload[0]);
        }

        public async Task<int> ReadBMSChargeFullCycles (ESXLocation source, BMS bms)
        {
            var response = new byte[2];
            switch (bms)
            {
                case BMS.INTERNAL:
                    response = await ReadBMS1Register(source, BMSRegister.CHARGE_FULL_CYCLES, 2);
                    break;
                case BMS.EXTERNAL:
                    response = await ReadBMS2Register(source, BMSRegister.CHARGE_FULL_CYCLES, 2);
                    break;
                default:
                    break;
            }
            var payload = GetPayload(response);
            return Utils.BytesToInt(payload[1], payload[0]);
        }

        public async Task<int> ReadBMSChargeTimes(ESXLocation source, BMS bms)
        {
            var response = new byte[2];
            switch (bms)
            {
                case BMS.INTERNAL:
                    response = await ReadBMS1Register(source, BMSRegister.CHARGE_COUNT, 2);
                    break;
                case BMS.EXTERNAL:
                    response = await ReadBMS2Register(source, BMSRegister.CHARGE_COUNT, 2);
                    break;
                default:
                    break;
            }
            var payload = GetPayload(response);
            return Utils.BytesToInt(payload[1], payload[0]);
        }

        //CHARGE_CAPACITY_L = 0x1D,//Not working
        //CHARGE_CAPACITY_H = 0x1E,//Not working
        //OVER_DISCHARGE_TIMES = 0x1F,//Not working

        public async Task<string> ReadBMSManufactureDate(ESXLocation source, BMS bms)
        {
            var response = new byte[2];
            switch (bms)
            {
                case BMS.INTERNAL:
                    response = await ReadBMS1Register(source, BMSRegister.MANUFACTURE_DATE, 2);
                    break;
                case BMS.EXTERNAL:
                    response = await ReadBMS2Register(source, BMSRegister.MANUFACTURE_DATE, 2);
                    break;
                default:
                    break;
            }
            var payload = GetPayload(response);
            int dateInt =  Utils.BytesToInt(payload[1], payload[0]);
            int year = dateInt >> 9;
            int month = (dateInt >> 5) & 0x0F;
            int day = dateInt & 0x1F;
            return "20" + year + "-" + month + "-" + day;
        }

        public async Task<BMSStatus> ReadBMSStatus(ESXLocation source, BMS bms)
        {
            var response = new byte[2];
            switch (bms)
            {
                case BMS.INTERNAL:
                    response = await ReadBMS1Register(source, BMSRegister.STATUS, 2);
                    break;
                case BMS.EXTERNAL:
                    response = await ReadBMS2Register(source, BMSRegister.STATUS, 2);
                    break;
                default:
                    break;
            }
            var payload = GetPayload(response);
            return new BMSStatus(payload[1], payload[0]);
        }

        public async Task<int> ReadBMSRemainingCapacity(ESXLocation source, BMS bms)
        {
            var response = new byte[2];
            switch (bms)
            {
                case BMS.INTERNAL:
                    response = await ReadBMS1Register(source, BMSRegister.REMAINING_CAPACITY_mAh, 2);
                    break;
                case BMS.EXTERNAL:
                    response = await ReadBMS2Register(source, BMSRegister.REMAINING_CAPACITY_mAh, 2);
                    break;
                default:
                    break;
            }
            var payload = GetPayload(response);
            return Utils.BytesToInt(payload[1], payload[0]);
        }

        public async Task<int> ReadBMSRemainingPercent(ESXLocation source, BMS bms)
        {
            var response = new byte[2];
            switch (bms)
            {
                case BMS.INTERNAL:
                    response = await ReadBMS1Register(source, BMSRegister.REMAINING_CAPACITY_PERCENT, 2);
                    break;
                case BMS.EXTERNAL:
                    response = await ReadBMS2Register(source, BMSRegister.REMAINING_CAPACITY_PERCENT, 2);
                    break;
                default:
                    break;
            }
            var payload = GetPayload(response);
            return Utils.BytesToInt(payload[1], payload[0]);
        }

        public async Task<int> ReadBMSCurrent(ESXLocation source, BMS bms)
        {
            var response = new byte[2];
            switch (bms)
            {
                case BMS.INTERNAL:
                    response = await ReadBMS1Register(source, BMSRegister.CURRENT, 2);
                    break;
                case BMS.EXTERNAL:
                    response = await ReadBMS2Register(source, BMSRegister.CURRENT, 2);
                    break;
                default:
                    break;
            }
            var payload = GetPayload(response);
            var current = Utils.BytesToInt(payload[1], payload[0]);
            if (current > 10000)
            {
                return (0xFFFF - current) * 10;
            }
            else
            {
                return current * -10;
            }
        }

        public async Task<int> ReadBMSVoltage(ESXLocation source, BMS bms)
        {
            var response = new byte[2];
            switch (bms)
            {
                case BMS.INTERNAL:
                    response = await ReadBMS1Register(source, BMSRegister.VOLTAGE, 2);
                    break;
                case BMS.EXTERNAL:
                    response = await ReadBMS2Register(source, BMSRegister.VOLTAGE, 2);
                    break;
                default:
                    break;
            }
            var payload = GetPayload(response);
            return Utils.BytesToInt(payload[1], payload[0]) * 10;
        }

        public async Task<BMSTemperature> ReadBMSTemperature(ESXLocation source, BMS bms)
        {
            var response = new byte[2];
            switch (bms)
            {
                case BMS.INTERNAL:
                    response = await ReadBMS1Register(source, BMSRegister.TEMPERATURE, 2);
                    break;
                case BMS.EXTERNAL:
                    response = await ReadBMS2Register(source, BMSRegister.TEMPERATURE, 2);
                    break;
                default:
                    break;
            }
            var payload = GetPayload(response);
            int temperature1 = payload[0] - 20;
            int temperature2 = payload[1] - 20;
            return new BMSTemperature(temperature1, temperature2);
        }

        public async Task<int> ReadBMSBalanceStatus(ESXLocation source, BMS bms)
        {
            var response = new byte[2];
            switch (bms)
            {
                case BMS.INTERNAL:
                    response = await ReadBMS1Register(source, BMSRegister.BALANCE_STATUS, 2);
                    break;
                case BMS.EXTERNAL:
                    response = await ReadBMS2Register(source, BMSRegister.BALANCE_STATUS, 2);
                    break;
                default:
                    break;
            }
            var payload = GetPayload(response);
            return Utils.BytesToInt(payload[1], payload[0]);
        }

        public async Task<int> ReadBMSUndervoltageStatus(ESXLocation source, BMS bms)
        {
            var response = new byte[2];
            switch (bms)
            {
                case BMS.INTERNAL:
                    response = await ReadBMS1Register(source, BMSRegister.UNDERVOLTAGE_STATUS, 2);
                    break;
                case BMS.EXTERNAL:
                    response = await ReadBMS2Register(source, BMSRegister.UNDERVOLTAGE_STATUS, 2);
                    break;
                default:
                    break;
            }
            var payload = GetPayload(response);
            return Utils.BytesToInt(payload[1], payload[0]);
        }

        public async Task<int> ReadBMSOvervoltageStatus(ESXLocation source, BMS bms)
        {
            var response = new byte[2];
            switch (bms)
            {
                case BMS.INTERNAL:
                    response = await ReadBMS1Register(source, BMSRegister.OVERVOLTAGE_STATUS, 2);
                    break;
                case BMS.EXTERNAL:
                    response = await ReadBMS2Register(source, BMSRegister.OVERVOLTAGE_STATUS, 2);
                    break;
                default:
                    break;
            }
            var payload = GetPayload(response);
            return Utils.BytesToInt(payload[1], payload[0]);
        }

        public async Task<int> ReadBMSCoulombmeterCapacity(ESXLocation source, BMS bms)
        {
            var response = new byte[2];
            switch (bms)
            {
                case BMS.INTERNAL:
                    response = await ReadBMS1Register(source, BMSRegister.COULOMBMETER_CAPACITY, 2);
                    break;
                case BMS.EXTERNAL:
                    response = await ReadBMS2Register(source, BMSRegister.COULOMBMETER_CAPACITY, 2);
                    break;
                default:
                    break;
            }
            var payload = GetPayload(response);
            return Utils.BytesToInt(payload[1], payload[0]);
        }

        public async Task<int> ReadBMSCapacityByVoltage(ESXLocation source, BMS bms)
        {
            var response = new byte[2];
            switch (bms)
            {
                case BMS.INTERNAL:
                    response = await ReadBMS1Register(source, BMSRegister.CAPACITY_BY_VOLTAGE, 2);
                    break;
                case BMS.EXTERNAL:
                    response = await ReadBMS2Register(source, BMSRegister.CAPACITY_BY_VOLTAGE, 2);
                    break;
                default:
                    break;
            }
            var payload = GetPayload(response);
            return Utils.BytesToInt(payload[1], payload[0]);
        }

        public async Task<int> ReadBMSHealth(ESXLocation source, BMS bms)
        {
            var response = new byte[2];
            switch (bms)
            {
                case BMS.INTERNAL:
                    response = await ReadBMS1Register(source, BMSRegister.HEALTH, 2);
                    break;
                case BMS.EXTERNAL:
                    response = await ReadBMS2Register(source, BMSRegister.HEALTH, 2);
                    break;
                default:
                    break;
            }
            var payload = GetPayload(response);
            return Utils.BytesToInt(payload[1], payload[0]);
        }

        public async Task<int[]> ReadBMSCellVoltages(ESXLocation source, BMS bms)
        {
            var response = new byte[0];
            switch (bms)
            {
                case BMS.INTERNAL:
                    response = await ReadBMS1Register(source, BMSRegister.CELL_1_VOLTAGE, 20);;
                    break;
                case BMS.EXTERNAL:
                    response = await ReadBMS2Register(source, BMSRegister.CELL_1_VOLTAGE, 20);
                    break;
                default:
                    return new int[10];
            }
            var payload = GetPayload(response);
            int[] voltages = new int[10];
            voltages[0] = Utils.BytesToInt(payload[1], payload[0]);
            voltages[1] = Utils.BytesToInt(payload[3], payload[2]);
            voltages[2] = Utils.BytesToInt(payload[5], payload[4]);
            voltages[3] = Utils.BytesToInt(payload[7], payload[6]);
            voltages[4] = Utils.BytesToInt(payload[9], payload[8]);
            voltages[5] = Utils.BytesToInt(payload[11], payload[10]);
            voltages[6] = Utils.BytesToInt(payload[13], payload[12]);
            voltages[7] = Utils.BytesToInt(payload[15], payload[14]);
            voltages[8] = Utils.BytesToInt(payload[17], payload[16]);
            voltages[9] = Utils.BytesToInt(payload[19], payload[18]);
            return voltages;
        }

        #endregion




        #region WRITE REGISTER

        //BLE_PASSWORD 0x17
        //REINITIALIZE_BLE 0x4D



        public async Task<bool> UnlockScooter(ESXLocation source)
        {
            await WriteESCRegisterWR(source, ESCRegister.UNLOCK_SCOOTER, new byte[] { 0x01, 0x00 });
            return true;
        }

        public async Task<bool> LockScooter(ESXLocation source)
        {
            await WriteESCRegisterWR(source, ESCRegister.LOCK_SCOOTER, new byte[] { 0x01, 0x00});
            return true;
        }

        public async Task<bool> WriteEngineStatus(ESXLocation source, ESXEngineStatus status)
        {
            await WriteESCRegisterWR(source, ESCRegister.ENGINE_STATUS, new byte[] { (byte)status, 0x00 });
            return true;
        }

        public async Task<bool> RestartScooter(ESXLocation source)
        {
            await WriteESCRegister(source, ESCRegister.RESTART, new byte[] { (byte)0x01, 0x00 });
            return true;
        }

        public async Task<bool> ShutDownScooter(ESXLocation source)
        {
            await WriteESCRegister(source, ESCRegister.SHUTDOWN, new byte[] { (byte)0x01, 0x00 });
            return true;
        }

        public async Task<bool> WriteSpeedLimitSportMode(ESXLocation source, short speed)
        {
            await WriteESCRegister(source, ESCRegister.SPEED_LIMIT_SPORT_MODE, new byte[] { (byte)(speed & 0xFF), 0 });
            return true;
        }

        public async Task<bool> WriteSpeedLimitNormalMode(ESXLocation source, short speed)
        {
            int value = (speed * 10);
            var high = value / 255;
            var low = value % 255;
            await WriteESCRegister(source, ESCRegister.SPEED_LIMIT_NORMAL_MODE, new byte[] { (byte)(low), (byte)(high) });
            return true;
        }

        public async Task<bool> WriteSpeedLimitEcoMode(ESXLocation source, short speed)
        {
            int value = speed * 10;
            var high = value / 255;
            var low = value % 255;
            await WriteESCRegister(source, ESCRegister.SPEED_LIMIT_ECO_MODE, new byte[] { (byte)(low), (byte)(high) });
            return true;
        }

        public async Task<bool> WriteOperatingMode(ESXLocation source, ESXOperatingMode mode)
        {
            await WriteESCRegister(source, ESCRegister.OPERATING_MODE, new byte[] { (byte)mode, 0x00 });
            return true;
        }

        public async Task<bool> WriteKERSLevel(ESXLocation source, ESXKERSLevel level)
        {
            await WriteESCRegister(source, ESCRegister.KERS_LEVEL, new byte[] { (byte)level, 0x00 });
            return true;
        }

        public async Task<bool> WriteCruiseControlStatus(ESXLocation source, ESXCruiseControlStatus status)
        {
            await WriteESCRegister(source, ESCRegister.CRUISE_CONTROL, new byte[] { (byte)status, 0x00 });
            return true;
        }

        public async Task<bool> WriteFunctions(ESXLocation source, ESXFunctions functions)
        {
            await WriteESCRegisterWR(source, ESCRegister.FUNCTION_SETUP, new byte[] { functions.GetByte(), 0x00 });
            return true;
        }

        public async Task<bool> WriteLEDEffect(ESXLocation source, ESXLedStripEffect effect)
        {
            await WriteESCRegister(source, ESCRegister.DISPLAY_MODE_OF_CHASSIS_LAMP, new byte[] { (byte)effect, 0x00 });
            return true;
        }





        #endregion


    }
}
