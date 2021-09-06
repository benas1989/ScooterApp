namespace ScooterApp.Services.NinebotESX
{
    public class ESXQuickStatus
    {
        public ESXError ErrorCode { get; set; }
        public byte AlarmCode { get; set; }
        public ESCStatus ESCStatus { get; set; }
        public byte BMS1Percentage { get; set; }
        public byte BMS2Percentage { get; set; }
        public byte BMSPercentage { get; set; }
        public float CurrentSpeed { get; set; }
        public float AverageSpeed { get; set; }
        public int TotalMileage { get; set; }
        public int CurrentMileage { get; set; }
        public int CurrentRuntime { get; set; }
        public float ESXTemperature { get; set; }
        public ESXOperatingMode OperatingMode { get; set; }
        public float Range { get; set; }
        public int Power { get; set; }
        public byte AlarmCodeForDelayReset { get; set; }
        public int PredictedRemainingMileage { get; set; }
    }
}
