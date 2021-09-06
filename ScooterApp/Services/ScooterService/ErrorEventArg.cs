namespace ScooterApp.Services.ScooterService
{
    public class ErrorEventArg
    {
        public ErrorType Type { get; set; }
        public string Message { get; set; }
        public byte[] Data { get; set; }

        public ErrorEventArg()
        {

        }

        public ErrorEventArg(ErrorType type, string message, byte[] data = null)
        {
            Type = type;
            Message = message;
            Data = data;
        }
    }
}
