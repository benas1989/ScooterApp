namespace ScooterApp.Services.NinebotESX
{
    public class ESCRentalFunctions
    {
        public bool HeadLightAlwaysOn { get; private set; }
        public bool HeadLightFlash { get; private set; }
        public bool TaillightAlwaysOn { get; private set; }
        public bool TaillightFlash { get; private set; }
        public bool BMS2LightAlwaysOn { get; private set; }
        public bool NoAlarmWhenLocked { get; private set; }
        public bool UseMphUnits { get; private set; }
        public bool DisplayUnit { get; private set; }
        public bool DisplaySpeed { get; private set; }
        public bool BLEDisplayOn { get; private set; }
        public bool DisplayBluetoothStatus { get; private set; }
        public bool BluetoothIconAlwaysOn { get; private set; }
        public bool BluetoothIconFlash { get; private set; }
        public bool DisplayErrorIcon { get; private set; }
        public bool DisplayTemperatureErrorIcon { get; private set; }
        public bool DisplayBatteryLevel { get; private set; }
        public bool ButtonCanChangeMode { get; private set; }
        public bool DisplaySpeedMode { get; private set; }



        public ESCRentalFunctions(byte functionsH, byte functionsL)
        {
            //TODO set variables
        }
    }
}
