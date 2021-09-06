using Plugin.BluetoothLE;
using ScooterApp.Services.NinebotESX;

namespace ScooterApp.ViewModels
{
    public class DeviceViewModel : ViewModelBase
    {
        //from BLE
        private string name;
        private int rssi;
        private string uuid;
        private bool isEncrypted;
        private bool isConnected = false;
        private bool isAuthenticated = false;

        //from esc
        private string escSerialNumber = "";
        private string blePassword = "";
        private string escVersion = "";
        private int errorCode;
        private string errorMessage = "";
        private int alarmCode;
        private bool hasSpeedLimit;
        private bool isLocked;
        private bool isBuzzerActive;
        private bool hasExternalBattery;
        private bool isESCActivated;
        private int currentOperatingSystem;
        private ESXOperatingMode currentOperatingMode;
        private byte bmsPercentage;
        private int actualRemainingMileage;
        private int predictedRemainingMileage;
        private float currentSpeed;
        private int totalMileage;
        private int currentMileage;
        private int totalRuntime;
        private int totalRidingTime;
        private int currentRuntime;
        private int currentRidingTime;
        private float scooterTemperature;
        private float escVoltage;
        private int bmsVoltage;
        private int bmsCurrent;
        private int motorPhaseCurrent;
        private float averageSpeed;
        private string externalBMSFirmwareVersionFromESC = "";
        private string internalBMSFirmwareVersionFromESC = "";
        private string bleFirmwareVersion = "";
        private float speedLimitSportMode;
        private float speedLimitNormalMode;
        private float speedLimitEcoMode;
        private ESXEngineStatus engineStatus;
        private ESXKERSLevel kersLevel;
        private ESXCruiseControlStatus cruiseControl;
        private bool taillightAlwaysOn;
        private ESXSpeedUnit speedUnits;
        private int power;
        private float range;

        //from internal bms
        private string internalBmsSerialNumber = "";
        private string internalBMSFirmwareVersion = "";
        private int internalBMSDesingCapacity;
        private int internalBMSActualCapacity;
        private int internalBMSDesignVoltage;
        private int internalBMSChargeFullCycles;
        private int internalBMSChargeCount;
        private string internalBMSManufactureDate = "";
        public bool internalBMSCheckCodeRight;
        public bool internalBMSActivated;
        public bool internalBMSInChargingProtect;
        public bool internalBMSChargingMosPipeIsOpen;
        public bool internalBMSWriteLockOn;
        public bool internalBMSIsDischarging;
        public bool internalBMSIsCharging;
        public bool internalBMSIsChargerInserted;
        public bool internalBMSIsUndervoltage;
        public bool internalBMSIsOvervoltage;
        public bool internalBMSIsOvertemperature;
        public bool internalBMSIsTestModeOn;
        private int internalBMSRemainingCapacity;
        private byte internalBMSRemainingCapacityPercent;
        private int internalBMSCurrent;
        private int internalBMSVoltage;
        private int internalBMSTemperature1;
        private int internalBMSTemperature2;
        private int internalBMSBalanceStatus;
        private int internalBMSUndervoltageStatus;
        private int internalBMSOvervoltageStatus;
        private int internalBMSCoulomberCapacity;
        private int internalBMSCapacityByVoltage;
        private int internalBMSHealthy;
        private byte[] internalBMSCellVoltages = new byte[10];
        private int internalBMSCell1Voltage;
        private int internalBMSCell2Voltage;
        private int internalBMSCell3Voltage;
        private int internalBMSCell4Voltage;
        private int internalBMSCell5Voltage;
        private int internalBMSCell6Voltage;
        private int internalBMSCell7Voltage;
        private int internalBMSCell8Voltage;
        private int internalBMSCell9Voltage;
        private int internalBMSCell10Voltage;

        //from external bms
        private string externalBmsSerialNumber = "";
        private string externalBMSFirmwareVersion = "";
        private int externalBMSDesingCapacity = 0;
        private int externalBMSActualCapacity = 0;
        private int externalBMSDesignVoltage = 0;
        private int externalBMSChargeFullCycles = 0;
        private int externalBMSChargeCount = 0;
        private string externalBMSManufactureDate = "";
        public bool externalBMSCheckCodeRight;
        public bool externalBMSActivated;
        public bool externalBMSInChargingProtect;
        public bool externalBMSChargingMosPipeIsOpen;
        public bool externalBMSWriteLockOn;
        public bool externalBMSIsDischarging;
        public bool externalBMSIsCharging;
        public bool externalBMSIsChargerInserted;
        public bool externalBMSIsUndervoltage;
        public bool externalBMSIsOvervoltage;
        public bool externalBMSIsOvertemperature;
        public bool externalBMSIsTestModeOn;
        private int externalBMSRemainingCapacity;
        private byte externalBMSRemainingCapacityPercent;
        private int externalBMSCurrent;
        private int externalBMSVoltage;
        private int externalBMSTemperature1;
        private int externalBMSTemperature2;
        private int externalBMSBalanceStatus;
        private int externalBMSUndervoltageStatus;
        private int externalBMSOvervoltageStatus;
        private int externalBMSCoulomberCapacity;
        private int externalBMSCapacityByVoltage;
        private int externalBMSHealthy;
        private byte[] externalBMSCellVoltages = new byte[10];
        private int externalBMSCell1Voltage;
        private int externalBMSCell2Voltage;
        private int externalBMSCell3Voltage;
        private int externalBMSCell4Voltage;
        private int externalBMSCell5Voltage;
        private int externalBMSCell6Voltage;
        private int externalBMSCell7Voltage;
        private int externalBMSCell8Voltage;
        private int externalBMSCell9Voltage;
        private int externalBMSCell10Voltage;


        //from BLE
        public string Name { get { return name; } set { SetValue(ref name, value); } }
        public int Rssi { get { return rssi; } set { SetValue(ref rssi, value); } }
        public string UUID { get { return uuid; } set { SetValue(ref uuid, value); } }
        public bool IsEncrypted { get { return isEncrypted; } set { SetValue(ref isEncrypted, value); } }
        public IDevice Current { get; set; }
        public bool IsConnected { get => isConnected; set => SetValue(ref isConnected, value); }
        public bool IsAuthenticated { get => isAuthenticated; set => SetValue(ref isAuthenticated, value); }

        //from esc
        public string ESCSerialNumber { get => escSerialNumber; set => SetValue(ref escSerialNumber, value); }
        public string BLEPassword { get => blePassword; set { SetValue(ref blePassword, value); } }
        public string ESCVersion { get => escVersion; set => SetValue(ref escVersion, value); }
        public int ErrorCode { get => errorCode; set { SetValue(ref errorCode, value); } }
        public string ErrorMessage { get => errorMessage; set { SetValue(ref errorMessage, value); } }
        public int AlarmCode { get => alarmCode; set { SetValue(ref alarmCode, value); } }
        public bool HasSpeedLimit { get => hasSpeedLimit; set { SetValue(ref hasSpeedLimit, value); } }
        public bool IsLocked { get => isLocked; set { SetValue(ref isLocked, value); } }
        public bool IsBuzzerActive { get => isBuzzerActive; set { SetValue(ref isBuzzerActive, value); } }
        public bool HasExternalBattery { get => hasExternalBattery; set { SetValue(ref hasExternalBattery, value); } }
        public bool IsESCActivated { get => isESCActivated; set { SetValue(ref isESCActivated, value); } }
        public int CurrentOperatingSystem { get => currentOperatingSystem; set { SetValue(ref currentOperatingSystem, value); } }
        public ESXOperatingMode CurrentOperatingMode { get => currentOperatingMode; set { SetValue(ref currentOperatingMode, value); } }
        public byte BMSPercentage { get => bmsPercentage; set { SetValue(ref bmsPercentage, value); } }
        public int ActualRemainingMileage { get => actualRemainingMileage; set { SetValue(ref actualRemainingMileage, value); } }
        public int PredictedRemainingMileage { get => predictedRemainingMileage; set { SetValue(ref predictedRemainingMileage, value); } }
        public float CurrentSpeed { get => currentSpeed; set { SetValue(ref currentSpeed, value); } }
        public int TotalMileage { get => totalMileage; set { SetValue(ref totalMileage, value); } }
        public int CurrentMileage { get => currentMileage; set { SetValue(ref currentMileage, value); ; } }
        public int TotalRuntime { get => totalRuntime; set { SetValue(ref totalRuntime, value); } }
        public int TotalRidingTime { get => totalRidingTime; set { SetValue(ref totalRidingTime, value); } }
        public int CurrentRuntime { get => currentRuntime; set { SetValue(ref currentRuntime, value); } }
        public int CurrentRidingTime { get => currentRidingTime; set { SetValue(ref currentRidingTime, value); } }
        public float ScooterTemperature { get => scooterTemperature; set { SetValue(ref scooterTemperature, value); } }
        public float ESCVoltage { get => escVoltage; set { SetValue(ref escVoltage, value); } }
        public int BMSVoltage { get => bmsVoltage; set { SetValue(ref bmsVoltage, value); } }
        public int BMSCurrent { get => bmsCurrent; set { SetValue(ref bmsCurrent, value); } }
        public int MotorPhaseCurrent { get => motorPhaseCurrent; set { SetValue(ref motorPhaseCurrent, value); } }
        public float AverageSpeed { get => averageSpeed; set { SetValue(ref averageSpeed, value); } }
        public string ExternalBMSFirmwareVersionFromESC { get => externalBMSFirmwareVersionFromESC; set { SetValue(ref externalBMSFirmwareVersionFromESC, value); } }
        public string InternalBMSFirmwareVersionFromESC { get => internalBMSFirmwareVersionFromESC; set { SetValue(ref internalBMSFirmwareVersionFromESC, value); } }
        public string BLEFirmwareVersion { get => bleFirmwareVersion; set { SetValue(ref bleFirmwareVersion, value); } }
        public float SpeedLimitSportMode { get => speedLimitSportMode; set { SetValue(ref speedLimitSportMode, value); } }
        public float SpeedLimitNormalMode { get => speedLimitNormalMode; set { SetValue(ref speedLimitNormalMode, value); } }
        public float SpeedLimitEcoMode { get => speedLimitEcoMode; set { SetValue(ref speedLimitEcoMode, value); } }
        public ESXEngineStatus EngineStatus { get => engineStatus; set { SetValue(ref engineStatus, value); } }
        public ESXKERSLevel KersLevel { get => kersLevel; set { SetValue(ref kersLevel, value); } }
        public ESXCruiseControlStatus CruiseControl { get => cruiseControl; set { SetValue(ref cruiseControl, value); } }
        public bool TaillightAlwaysOn { get => taillightAlwaysOn; set { SetValue(ref taillightAlwaysOn, value); } }
        public ESXSpeedUnit SpeedUnits { get => speedUnits; set { SetValue(ref speedUnits, value); } }
        public int Power { get => power; set { SetValue(ref power, value); } }
        public float Range { get => range; set { SetValue(ref range, value); } }

        //from internal bms
        public string InternalBMSSerialNumber { get => internalBmsSerialNumber; set { SetValue(ref internalBmsSerialNumber, value); } }
        public string InternalBMSFirmwareVersion { get => internalBMSFirmwareVersion; set { SetValue(ref internalBMSFirmwareVersion, value); } }
        public int InternalBMSDesignCapacity { get => internalBMSDesingCapacity; set { SetValue(ref internalBMSDesingCapacity, value); } }
        public int InternalBMSActualCapacity { get => internalBMSActualCapacity; set { SetValue(ref internalBMSActualCapacity, value); } }
        public int InternalBMSDesignVoltage { get => internalBMSDesignVoltage; set { SetValue(ref internalBMSDesignVoltage, value); } }
        public int InternalBMSChargeFullCycles { get => internalBMSChargeFullCycles; set { SetValue(ref internalBMSChargeFullCycles, value); } }
        public int InternalBMSChargeCount { get => internalBMSChargeCount; set { SetValue(ref internalBMSChargeCount, value); } }
        public string InternalBMSManufactureDate { get => internalBMSManufactureDate; set { SetValue(ref internalBMSManufactureDate, value); } }
        public bool InternalBMSCheckCodeRight { get => internalBMSCheckCodeRight; set { SetValue(ref internalBMSCheckCodeRight, value); } }
        public bool InternalBMSActivated { get => internalBMSActivated; set { SetValue(ref internalBMSActivated, value); } }
        public bool InternalBMSInChargingProtect { get => internalBMSInChargingProtect; set { SetValue(ref internalBMSInChargingProtect, value); } }
        public bool InternalBMSChargingMosPipeIsOpen { get => internalBMSChargingMosPipeIsOpen; set { SetValue(ref internalBMSChargingMosPipeIsOpen, value); } }
        public bool InternalBMSWriteLockOn { get => internalBMSWriteLockOn; set { SetValue(ref internalBMSWriteLockOn, value); } }
        public bool InternalBMSIsDischarging { get => internalBMSIsDischarging; set { SetValue(ref internalBMSIsDischarging, value); } }
        public bool InternalBMSIsCharging { get => internalBMSIsCharging; set { SetValue(ref internalBMSIsCharging, value); } }
        public bool InternalBMSIsChargerInserted { get => internalBMSIsChargerInserted; set { SetValue(ref internalBMSIsChargerInserted, value); } }
        public bool InternalBMSIsUndervoltage { get => internalBMSIsUndervoltage; set { SetValue(ref internalBMSIsUndervoltage, value); } }
        public bool InternalBMSIsOvervoltage { get => internalBMSIsOvervoltage; set { SetValue(ref internalBMSIsOvervoltage, value); } }
        public bool InternalBMSIsOvertemperature { get => internalBMSIsOvertemperature; set { SetValue(ref internalBMSIsOvertemperature, value); } }
        public bool InternalBMSIsTestModeOn { get => internalBMSIsTestModeOn; set { SetValue(ref internalBMSIsTestModeOn, value); } }
        public int InternalBMSRemainingCapacity { get => internalBMSRemainingCapacity; set { SetValue(ref internalBMSRemainingCapacity, value); } }
        public byte InternalBMSRemainingCapacityPercent { get => internalBMSRemainingCapacityPercent; set { SetValue(ref internalBMSRemainingCapacityPercent, value); } }
        public int InternalBMSCurrent { get => internalBMSCurrent; set { SetValue(ref internalBMSCurrent, value); } }
        public int InternalBMSVoltage { get => internalBMSVoltage; set { SetValue(ref internalBMSVoltage, value); } }
        public int InternalBMSTemperature1 { get => internalBMSTemperature1; set { SetValue(ref internalBMSTemperature1, value); } }
        public int InternalBMSTemperature2 { get => internalBMSTemperature2; set { SetValue(ref internalBMSTemperature2, value); } }
        public int InternalBMSBalanceStatus { get => internalBMSBalanceStatus; set { SetValue(ref internalBMSBalanceStatus, value); } }
        public int InternalBMSUndervoltageStatus { get => internalBMSUndervoltageStatus; set { SetValue(ref internalBMSUndervoltageStatus, value); } }
        public int InternalBMSOvervoltageStatus { get => internalBMSOvervoltageStatus; set { SetValue(ref internalBMSOvervoltageStatus, value); } }
        public int InternalBMSCoulomberCapacity { get => internalBMSCoulomberCapacity; set { SetValue(ref internalBMSCoulomberCapacity, value); } }
        public int InternalBMSCapacityByVoltage { get => internalBMSCapacityByVoltage; set { SetValue(ref internalBMSCapacityByVoltage, value); } }
        public int InternalBMSHealthy { get => internalBMSHealthy; set { SetValue(ref internalBMSHealthy, value); } }
        public byte[] InternalBMSCellVoltages { get => internalBMSCellVoltages; set { SetValue(ref internalBMSCellVoltages, value); } }
        public int InternalBMSCell1Voltage { get => internalBMSCell1Voltage; set { SetValue(ref internalBMSCell1Voltage, value); } }
        public int InternalBMSCell2Voltage { get => internalBMSCell2Voltage; set { SetValue(ref internalBMSCell2Voltage, value); } }
        public int InternalBMSCell3Voltage { get => internalBMSCell3Voltage; set { SetValue(ref internalBMSCell3Voltage, value); } }
        public int InternalBMSCell4Voltage { get => internalBMSCell4Voltage; set { SetValue(ref internalBMSCell4Voltage, value); } }
        public int InternalBMSCell5Voltage { get => internalBMSCell5Voltage; set { SetValue(ref internalBMSCell5Voltage, value); } }
        public int InternalBMSCell6Voltage { get => internalBMSCell6Voltage; set { SetValue(ref internalBMSCell6Voltage, value); } }
        public int InternalBMSCell7Voltage { get => internalBMSCell7Voltage; set { SetValue(ref internalBMSCell7Voltage, value); } }
        public int InternalBMSCell8Voltage { get => internalBMSCell8Voltage; set { SetValue(ref internalBMSCell8Voltage, value); } }
        public int InternalBMSCell9Voltage { get => internalBMSCell9Voltage; set { SetValue(ref internalBMSCell9Voltage, value); } }
        public int InternalBMSCell10Voltage { get => internalBMSCell10Voltage; set { SetValue(ref internalBMSCell10Voltage, value); } }

        //from external bms
        public string ExternalBMSSerialNumber { get => externalBmsSerialNumber; set { SetValue(ref externalBmsSerialNumber, value); } }
        public string ExternalBMSFirmwareVersion { get => externalBMSFirmwareVersion; set { SetValue(ref externalBMSFirmwareVersion, value); } }
        public int ExternalBMSDesignCapacity { get => externalBMSDesingCapacity; set { SetValue(ref externalBMSDesingCapacity, value); } }
        public int ExternalBMSActualCapacity { get => externalBMSActualCapacity; set { SetValue(ref externalBMSActualCapacity, value); } }
        public int ExternalBMSDesignVoltage { get => externalBMSDesignVoltage; set { SetValue(ref externalBMSDesignVoltage, value); } }
        public int ExternalBMSChargeFullCycles { get => externalBMSChargeFullCycles; set { SetValue(ref externalBMSChargeFullCycles, value); } }
        public int ExternalBMSChargeCount { get => externalBMSChargeCount; set { SetValue(ref externalBMSChargeCount, value); } }
        public string ExternalBMSManufactureDate { get => externalBMSManufactureDate; set { SetValue(ref externalBMSManufactureDate, value); } }
        public bool ExternalBMSCheckCodeRight { get => externalBMSCheckCodeRight; set { SetValue(ref externalBMSCheckCodeRight, value); } }
        public bool ExternalBMSActivated { get => externalBMSActivated; set { SetValue(ref externalBMSActivated, value); } }
        public bool ExternalBMSInChargingProtect { get => externalBMSInChargingProtect; set { SetValue(ref externalBMSInChargingProtect, value); } }
        public bool ExternalBMSChargingMosPipeIsOpen { get => externalBMSChargingMosPipeIsOpen; set { SetValue(ref externalBMSChargingMosPipeIsOpen, value); } }
        public bool ExternalBMSWriteLockOn { get => externalBMSWriteLockOn; set { SetValue(ref externalBMSWriteLockOn, value); } }
        public bool ExternalBMSIsDischarging { get => externalBMSIsDischarging; set { SetValue(ref externalBMSIsDischarging, value); } }
        public bool ExternalBMSIsCharging { get => externalBMSIsCharging; set { SetValue(ref externalBMSIsCharging, value); } }
        public bool ExternalBMSIsChargerInserted { get => externalBMSIsChargerInserted; set { SetValue(ref externalBMSIsChargerInserted, value); } }
        public bool ExternalBMSIsUndervoltage { get => externalBMSIsUndervoltage; set { SetValue(ref externalBMSIsUndervoltage, value); } }
        public bool ExternalBMSIsOvervoltage { get => externalBMSIsOvervoltage; set { SetValue(ref externalBMSIsOvervoltage, value); } }
        public bool ExternalBMSIsOvertemperature { get => externalBMSIsOvertemperature; set { SetValue(ref externalBMSIsOvertemperature, value); } }
        public bool ExternalBMSIsTestModeOn { get => externalBMSIsTestModeOn; set { SetValue(ref externalBMSIsTestModeOn, value); } }
        public int ExternalBMSRemainingCapacity { get => externalBMSRemainingCapacity; set { SetValue(ref externalBMSRemainingCapacity, value); } }
        public byte ExternalBMSRemainingCapacityPercent { get => externalBMSRemainingCapacityPercent; set { SetValue(ref externalBMSRemainingCapacityPercent, value); } }
        public int ExternalBMSCurrent { get => externalBMSCurrent; set { SetValue(ref externalBMSCurrent, value); } }
        public int ExternalBMSVoltage { get => externalBMSVoltage; set { SetValue(ref externalBMSVoltage, value); } }
        public int ExternalBMSTemperature1 { get => externalBMSTemperature1; set { SetValue(ref externalBMSTemperature1, value); } }
        public int ExternalBMSTemperature2 { get => externalBMSTemperature2; set { SetValue(ref externalBMSTemperature2, value); } }
        public int ExternalBMSBalanceStatus { get => externalBMSBalanceStatus; set { SetValue(ref externalBMSBalanceStatus, value); } }
        public int ExternalBMSUndervoltageStatus { get => externalBMSUndervoltageStatus; set { SetValue(ref externalBMSUndervoltageStatus, value); } }
        public int ExternalBMSOvervoltageStatus { get => externalBMSOvervoltageStatus; set { SetValue(ref externalBMSOvervoltageStatus, value); } }
        public int ExternalBMSCoulomberCapacity { get => externalBMSCoulomberCapacity; set { SetValue(ref externalBMSCoulomberCapacity, value); } }
        public int ExternalBMSCapacityByVoltage { get => externalBMSCapacityByVoltage; set { SetValue(ref externalBMSCapacityByVoltage, value); } }
        public int ExternalBMSHealthy { get => externalBMSHealthy; set { SetValue(ref externalBMSHealthy, value); } }
        public byte[] ExternalBMSCellVoltages { get => externalBMSCellVoltages; set { SetValue(ref externalBMSCellVoltages, value); } }
        public int ExternalBMSCell1Voltage { get => externalBMSCell1Voltage; set { SetValue(ref externalBMSCell1Voltage, value); } }
        public int ExternalBMSCell2Voltage { get => externalBMSCell2Voltage; set { SetValue(ref externalBMSCell2Voltage, value); } }
        public int ExternalBMSCell3Voltage { get => externalBMSCell3Voltage; set { SetValue(ref externalBMSCell3Voltage, value); } }
        public int ExternalBMSCell4Voltage { get => externalBMSCell4Voltage; set { SetValue(ref externalBMSCell4Voltage, value); } }
        public int ExternalBMSCell5Voltage { get => externalBMSCell5Voltage; set { SetValue(ref externalBMSCell5Voltage, value); } }
        public int ExternalBMSCell6Voltage { get => externalBMSCell6Voltage; set { SetValue(ref externalBMSCell6Voltage, value); } }
        public int ExternalBMSCell7Voltage { get => externalBMSCell7Voltage; set { SetValue(ref externalBMSCell7Voltage, value); } }
        public int ExternalBMSCell8Voltage { get => externalBMSCell8Voltage; set { SetValue(ref externalBMSCell8Voltage, value); } }
        public int ExternalBMSCell9Voltage { get => externalBMSCell9Voltage; set { SetValue(ref externalBMSCell9Voltage, value); } }
        public int ExternalBMSCell10Voltage { get => externalBMSCell10Voltage; set { SetValue(ref externalBMSCell10Voltage, value); } }


        public DeviceViewModel(IDevice device, bool isEncrypted)
        {
            name = string.IsNullOrWhiteSpace(device.Name) ? "Uknown name" : device.Name;
            Current = device;
            this.isEncrypted = isEncrypted;
        }
    }
}
