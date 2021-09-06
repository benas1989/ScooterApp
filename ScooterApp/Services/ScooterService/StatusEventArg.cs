namespace ScooterApp.Services.ScooterService
{
    public class StatusEventArg
    {
        public StatusType Type { get; set; }
        public string Message { get; set; }
        public byte[] Data { get; set; }

        public StatusEventArg()
        {

        }

        public StatusEventArg(StatusType type, string message, byte[] data = null)
        {
            Type = type;
            Message = message;
            Data = data;
        }
    }
}
