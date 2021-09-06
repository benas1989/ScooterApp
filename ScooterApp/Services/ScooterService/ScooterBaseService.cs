using System;
using System.Threading.Tasks;

namespace ScooterApp.Services.ScooterService
{
    public abstract class ScooterBaseService
    {
        public event EventHandler<ActionEventArg> OnAction;
        public event EventHandler<ErrorEventArg> OnError;
        public event EventHandler<StatusEventArg> OnStatus;
        public event EventHandler<byte[]> OnNewPacket;


        public string ScooterName { get; protected set; }
        public bool IsConnected { get; protected set; }
        public bool IsAuthenticated { get; protected set; }
        public bool IsEncrypted { get; protected set; }
        public ICryptoService CryptoService { get; protected set; }
        public abstract Task Connect();
        public abstract Task Disconnect();
        public abstract Task<bool> Authenticate();
        public abstract byte[] GetPacket(byte source, byte destination, byte command, byte commandData, byte[] payload = null);
        public abstract Task<byte[]> ReadRegister(byte source, byte destination, byte register, byte length);
        public abstract Task<bool> WriteRegister(byte source, byte destination, byte register, byte[] data);
        public abstract Task<byte[]> WriteRegisterWR(byte source, byte destination, byte register, byte[] data);


        protected abstract Task<bool> Write(byte[] packet);
        protected void OnActionEvent(object sender, ActionType type, string message, byte[] data = null)
        {
            OnAction?.Invoke(sender, new ActionEventArg { Type = type, Message = message, Data = data });
        }

        protected void OnErrorEvent(object sender, ErrorType type, string message, byte[] data = null)
        {
            OnError?.Invoke(sender, new ErrorEventArg { Type = type, Message = message, Data = data });
        }

        protected void OnStatusEvent(object sender, StatusType type, string message, byte[] data = null)
        {
            OnStatus?.Invoke(sender, new StatusEventArg { Type = type, Message = message, Data = data });
        }

        protected void OnNewPacketEvent(object sender, byte[] data)
        {
            OnNewPacket?.Invoke(sender, data);
        }
    }
}
