namespace ScooterApp.Services.NinebotESX
{
    public class ESXFunctions
    {
        public bool TaillightAlwaysOn { get; private set; }
        public bool UseMphUnits { get; private set; }
        public ESXSpeedUnit Units { get; set; }

        public ESXFunctions(byte byte0)
        {
            TaillightAlwaysOn = (byte0 & 0x02) == 0x02;
            UseMphUnits = (byte0 & 0x10) == 0x10;
            if (UseMphUnits)
            {
                Units = ESXSpeedUnit.MPH;
            }
            else
            {
                Units = ESXSpeedUnit.KMH;
            }
        }

        public ESXFunctions(bool taillightAlwaysOn, bool useMphUnits)
        {
            TaillightAlwaysOn = taillightAlwaysOn;
            UseMphUnits = useMphUnits;
            if (useMphUnits)
            {
                Units = ESXSpeedUnit.MPH;
            }
            else
            {
                Units = ESXSpeedUnit.KMH;
            }
        }

        public override string ToString()
        {
            return "TaillightAlwaysOn: " + TaillightAlwaysOn + " UseMphUnits: " + UseMphUnits;
        }

        public byte GetByte()
        {
            var taillightAlwaysOn = TaillightAlwaysOn ? 0x02 : 0x00;
            var useMphUnits = UseMphUnits ? 0x10 : 0x00;
            return (byte)(taillightAlwaysOn | useMphUnits);
        }
    }
}
