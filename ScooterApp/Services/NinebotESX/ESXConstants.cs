namespace ScooterApp.Services.NinebotESX
{
    public class ESXConstants
    {
        public static byte HeaderByte1 = 0x5A;
        public static byte HeaderByte2 = 0xA5;
        public static int BLEPacketLength = 20;
        public static readonly int MinimumPacketLength = 9;
        public static readonly int MinimumEncryptedPacketLength = 13;
        public static readonly byte[] Default_BLE_Pin = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
    }
}
