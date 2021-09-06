namespace ScooterApp.Services.NinebotESX
{
    public class BMSStatus
    {
        public bool BatteryCheckCodeRight { get; set; }
        public bool BatteryActivated { get; set; }
        public bool BatteryInChargingProtect { get; set; }
        public bool ChargingMosPipeIsOpen { get; set; }
        public bool WriteLockOn { get; set; }
        public bool IsDischarging { get; set; }
        public bool IsCharging { get; set; }
        public bool IsChargerInserted { get; set; }
        public bool IsUndervoltage { get; set; }
        public bool IsOvervoltage { get; set; }
        public bool IsOvertemperature { get; set; }
        public bool IsTestModeOn { get; set; }

        public BMSStatus(byte statusH, byte statusL)
        {
            BatteryCheckCodeRight = (statusL & 0x01) == 0x01;
            BatteryActivated = (statusL & 0x02) == 0x02;
            BatteryInChargingProtect = (statusL & 0x04) == 0x04;
            ChargingMosPipeIsOpen = (statusL & 0x08) == 0x08;
            WriteLockOn = (statusL & 0x10) == 0x10;
            IsDischarging = (statusL & 0x20) == 0x20;
            IsCharging = (statusL & 0x40) == 0x40;
            IsChargerInserted = (statusL & 0x80) == 0x80;

            IsUndervoltage = (statusH & 0x01) == 0x01;
            IsOvervoltage = (statusH & 0x02) == 0x02;
            IsOvertemperature = (statusH & 0x04) == 0x04;
            IsTestModeOn = (statusH & 0x08) == 0x08;
        }
    }
}
