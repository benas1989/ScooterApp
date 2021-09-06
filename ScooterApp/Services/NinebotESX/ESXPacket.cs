namespace ScooterApp.Services.NinebotESX
{
    public class ESXPacket
    {
        public byte Header1 { get; set; }
        public byte Header2 { get; set; }
        public byte PayloadLength { get; set; }
        public ESXLocation Source { get; set; }
        public ESXLocation Destination { get; set; }
        public ESXCommand Command { get; set; }
        public byte CommandData { get; set; }
        public byte[] Payload { get; set; }
        public byte[] CheckSum { get; set; }
    }
}
