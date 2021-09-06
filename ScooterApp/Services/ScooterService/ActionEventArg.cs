namespace ScooterApp.Services.ScooterService
{
    public class ActionEventArg
    {
        public ActionType Type { get; set; }
        public string Message { get; set; }
        public byte[] Data { get; set; }

        public ActionEventArg()
        {

        }

        public ActionEventArg(ActionType type, string message, byte[] data = null)
        {
            Type = type;
            Message = message;
            Data = data;
        }
    }
}
