namespace ScooterApp.Services.NinebotESX
{
    public class ESCStatus
    {
        public bool HasSpeedLimit { get; private set; }
        public bool IsLocked { get; private set; }
        public bool IsBuzzerActive { get; private set; }
        public bool HasExternalBattery { get; private set; }
        public bool IsActivated { get; private set; }

        public ESCStatus(byte statusH, byte statusL)
        {
            HasSpeedLimit = (statusL & 0x01) == 1;
            IsLocked =  (statusL & 0x02) == 2;
            IsBuzzerActive = (statusL & 0x04) == 4;
            HasExternalBattery = (statusH & 0x02) == 2;
            IsActivated = (statusH & 0x08) == 8;
        }
    }
}
