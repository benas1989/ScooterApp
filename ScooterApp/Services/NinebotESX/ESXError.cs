namespace ScooterApp.Services.NinebotESX
{
    public class ESXError
    {
        public int Id { get; set; }
        public string Message { get; set; }

        public ESXError(int id, string message)
        {
            Id = id;
            Message = message;
        }

        public ESXError(int id)
        {
            Id = id;
            if (ESXErrors.ErrorCodes.ContainsKey(id))
            {
                Message = ESXErrors.ErrorCodes[id];
            }
            else
            {
                Message = "Uknown error.";
            }
        }
    }
}
